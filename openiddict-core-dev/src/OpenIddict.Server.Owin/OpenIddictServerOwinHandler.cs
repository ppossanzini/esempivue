﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.OpenIddictServerEvents;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace OpenIddict.Server.Owin
{
    /// <summary>
    /// Provides the entry point necessary to register the OpenIddict server in an OWIN pipeline.
    /// </summary>
    public class OpenIddictServerOwinHandler : AuthenticationHandler<OpenIddictServerOwinOptions>
    {
        private readonly IOpenIddictServerDispatcher _dispatcher;
        private readonly IOpenIddictServerFactory _factory;

        /// <summary>
        /// Creates a new instance of the <see cref="OpenIddictServerOwinHandler"/> class.
        /// </summary>
        /// <param name="dispatcher">The OpenIddict server dispatcher used by this instance.</param>
        /// <param name="factory">The OpenIddict server factory used by this instance.</param>
        public OpenIddictServerOwinHandler(
            IOpenIddictServerDispatcher dispatcher,
            IOpenIddictServerFactory factory)
        {
            _dispatcher = dispatcher;
            _factory = factory;
        }

        /// <inheritdoc/>
        protected override async Task InitializeCoreAsync()
        {
            // Note: the transaction may be already attached when replaying an OWIN request
            // (e.g when using a status code pages middleware re-invoking the OWIN pipeline).
            var transaction = Context.Get<OpenIddictServerTransaction>(typeof(OpenIddictServerTransaction).FullName);
            if (transaction is null)
            {
                // Create a new transaction and attach the OWIN request to make it available to the OWIN handlers.
                transaction = await _factory.CreateTransactionAsync();
                transaction.Properties[typeof(IOwinRequest).FullName!] = new WeakReference<IOwinRequest>(Request);

                // Attach the OpenIddict server transaction to the OWIN shared dictionary
                // so that it can retrieved while performing sign-in/sign-out operations.
                Context.Set(typeof(OpenIddictServerTransaction).FullName, transaction);
            }

            var context = new ProcessRequestContext(transaction);
            await _dispatcher.DispatchAsync(context);

            // Store the context in the transaction so that it can be retrieved from InvokeAsync().
            transaction.SetProperty(typeof(ProcessRequestContext).FullName!, context);
        }

        /// <inheritdoc/>
        public override async Task<bool> InvokeAsync()
        {
            // Note: due to internal differences between ASP.NET Core and Katana, the request MUST start being processed
            // in InitializeCoreAsync() to ensure the request context is available from AuthenticateCoreAsync() when
            // active authentication is used, as AuthenticateCoreAsync() is always called before InvokeAsync() in this case.

            var transaction = Context.Get<OpenIddictServerTransaction>(typeof(OpenIddictServerTransaction).FullName) ??
                throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));

            var context = transaction.GetProperty<ProcessRequestContext>(typeof(ProcessRequestContext).FullName!) ??
                throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));

            if (context.IsRequestHandled)
            {
                return true;
            }

            else if (context.IsRequestSkipped)
            {
                return false;
            }

            else if (context.IsRejected)
            {
                var notification = new ProcessErrorContext(transaction)
                {
                    Response = new OpenIddictResponse
                    {
                        Error = context.Error ?? Errors.InvalidRequest,
                        ErrorDescription = context.ErrorDescription,
                        ErrorUri = context.ErrorUri
                    }
                };

                await _dispatcher.DispatchAsync(notification);

                if (notification.IsRequestHandled)
                {
                    return true;
                }

                else if (notification.IsRequestSkipped)
                {
                    return false;
                }
                throw new InvalidOperationException(SR.GetResourceString(SR.ID0111));

            }

            return false;
        }

        /// <inheritdoc/>
        protected override async Task<AuthenticationTicket?> AuthenticateCoreAsync()
        {
            var transaction = Context.Get<OpenIddictServerTransaction>(typeof(OpenIddictServerTransaction).FullName);
            if (transaction is null)
            {
                throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));
            }

            // Note: in many cases, the authentication token was already validated by the time this action is called
            // (generally later in the pipeline, when using the pass-through mode). To avoid having to re-validate it,
            // the authentication context is resolved from the transaction. If it's not available, a new one is created.
            var context = transaction.GetProperty<ProcessAuthenticationContext>(typeof(ProcessAuthenticationContext).FullName!);
            if (context is null)
            {
                context = new ProcessAuthenticationContext(transaction);
                await _dispatcher.DispatchAsync(context);

                // Store the context object in the transaction so it can be later retrieved by handlers
                // that want to access the authentication result without triggering a new authentication flow.
                transaction.SetProperty(typeof(ProcessAuthenticationContext).FullName!, context);
            }

            if (context.IsRequestHandled || context.IsRequestSkipped)
            {
                return null;
            }

            else if (context.IsRejected)
            {
                // Note: the missing_token error is special-cased to indicate to Katana
                // that no authentication result could be produced due to the lack of token.
                // This also helps reducing the logging noise when no token is specified.
                if (string.Equals(context.Error, Errors.MissingToken, StringComparison.Ordinal))
                {
                    return null;
                }

                var properties = new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerOwinConstants.Properties.Error] = context.Error,
                    [OpenIddictServerOwinConstants.Properties.ErrorDescription] = context.ErrorDescription,
                    [OpenIddictServerOwinConstants.Properties.ErrorUri] = context.ErrorUri
                });

                return new AuthenticationTicket(null, properties);
            }

            else
            {
                Debug.Assert(context.Principal is not null, SR.GetResourceString(SR.ID4006));
                Debug.Assert(!string.IsNullOrEmpty(context.Principal.GetTokenType()), SR.GetResourceString(SR.ID4009));
                Debug.Assert(!string.IsNullOrEmpty(context.Token), SR.GetResourceString(SR.ID4010));

                // Store the token to allow any OWIN/Katana component (e.g a controller)
                // to retrieve it (e.g to make an API request to another application).
                var properties = new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [context.Principal.GetTokenType()!] = context.Token
                });

                return new AuthenticationTicket((ClaimsIdentity) context.Principal.Identity, properties);
            }
        }

        /// <inheritdoc/>
        protected override async Task TeardownCoreAsync()
        {
            // Note: OWIN authentication handlers cannot reliabily write to the response stream
            // from ApplyResponseGrantAsync or ApplyResponseChallengeAsync because these methods
            // are susceptible to be invoked from AuthenticationHandler.OnSendingHeaderCallback,
            // where calling Write or WriteAsync on the response stream may result in a deadlock
            // on hosts using streamed responses. To work around this limitation, this handler
            // doesn't implement ApplyResponseGrantAsync but TeardownCoreAsync, which is never called
            // by AuthenticationHandler.OnSendingHeaderCallback. In theory, this would prevent
            // OpenIddictServerOwinMiddleware from both applying the response grant and allowing
            // the next middleware in the pipeline to alter the response stream but in practice,
            // OpenIddictServerOwinMiddleware is assumed to be the only middleware allowed to write
            // to the response stream when a response grant (sign-in/out or challenge) was applied.

            // Note: unlike the ASP.NET Core host, the OWIN host MUST check whether the status code
            // corresponds to a challenge response, as LookupChallenge() will always return a non-null
            // value when active authentication is used, even if no challenge was actually triggered.
            var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);
            if (challenge is not null && (Response.StatusCode == 401 || Response.StatusCode == 403))
            {
                var transaction = Context.Get<OpenIddictServerTransaction>(typeof(OpenIddictServerTransaction).FullName) ??
                    throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));

                transaction.Properties[typeof(AuthenticationProperties).FullName!] = challenge.Properties ?? new AuthenticationProperties();

                var context = new ProcessChallengeContext(transaction)
                {
                    Response = new OpenIddictResponse()
                };

                await _dispatcher.DispatchAsync(context);

                if (context.IsRequestHandled || context.IsRequestSkipped)
                {
                    return;
                }

                else if (context.IsRejected)
                {
                    var notification = new ProcessErrorContext(transaction)
                    {
                        Response = new OpenIddictResponse
                        {
                            Error = context.Error ?? Errors.InvalidRequest,
                            ErrorDescription = context.ErrorDescription,
                            ErrorUri = context.ErrorUri
                        }
                    };

                    await _dispatcher.DispatchAsync(notification);

                    if (notification.IsRequestHandled || context.IsRequestSkipped)
                    {
                        return;
                    }

                    throw new InvalidOperationException(SR.GetResourceString(SR.ID0111));
                }
            }

            var signin = Helper.LookupSignIn(Options.AuthenticationType);
            if (signin is not null)
            {
                var transaction = Context.Get<OpenIddictServerTransaction>(typeof(OpenIddictServerTransaction).FullName) ??
                    throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));

                transaction.Properties[typeof(AuthenticationProperties).FullName!] = signin.Properties ?? new AuthenticationProperties();

                var context = new ProcessSignInContext(transaction)
                {
                    Principal = signin.Principal,
                    Response = new OpenIddictResponse()
                };

                await _dispatcher.DispatchAsync(context);

                if (context.IsRequestHandled || context.IsRequestSkipped)
                {
                    return;
                }

                else if (context.IsRejected)
                {
                    var notification = new ProcessErrorContext(transaction)
                    {
                        Response = new OpenIddictResponse
                        {
                            Error = context.Error ?? Errors.InvalidRequest,
                            ErrorDescription = context.ErrorDescription,
                            ErrorUri = context.ErrorUri
                        }
                    };

                    await _dispatcher.DispatchAsync(notification);

                    if (notification.IsRequestHandled || context.IsRequestSkipped)
                    {
                        return;
                    }

                    throw new InvalidOperationException(SR.GetResourceString(SR.ID0111));
                }
            }

            var signout = Helper.LookupSignOut(Options.AuthenticationType, Options.AuthenticationMode);
            if (signout is not null)
            {
                var transaction = Context.Get<OpenIddictServerTransaction>(typeof(OpenIddictServerTransaction).FullName) ??
                    throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));

                transaction.Properties[typeof(AuthenticationProperties).FullName!] = signout.Properties ?? new AuthenticationProperties();

                var context = new ProcessSignOutContext(transaction)
                {
                    Response = new OpenIddictResponse()
                };

                await _dispatcher.DispatchAsync(context);

                if (context.IsRequestHandled || context.IsRequestSkipped)
                {
                    return;
                }

                else if (context.IsRejected)
                {
                    var notification = new ProcessErrorContext(transaction)
                    {
                        Response = new OpenIddictResponse
                        {
                            Error = context.Error ?? Errors.InvalidRequest,
                            ErrorDescription = context.ErrorDescription,
                            ErrorUri = context.ErrorUri
                        }
                    };

                    await _dispatcher.DispatchAsync(notification);

                    if (notification.IsRequestHandled || context.IsRequestSkipped)
                    {
                        return;
                    }

                    throw new InvalidOperationException(SR.GetResourceString(SR.ID0111));
                }
            }
        }
    }
}
