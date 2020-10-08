﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.OpenIddictServerEvents;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace OpenIddict.Server.AspNetCore
{
    /// <summary>
    /// Provides the logic necessary to extract, validate and handle OpenID Connect requests.
    /// </summary>
    public class OpenIddictServerAspNetCoreHandler : AuthenticationHandler<OpenIddictServerAspNetCoreOptions>,
        IAuthenticationRequestHandler,
        IAuthenticationSignInHandler,
        IAuthenticationSignOutHandler
    {
        private readonly IOpenIddictServerDispatcher _dispatcher;
        private readonly IOpenIddictServerFactory _factory;

        /// <summary>
        /// Creates a new instance of the <see cref="OpenIddictServerAspNetCoreHandler"/> class.
        /// </summary>
        public OpenIddictServerAspNetCoreHandler(
            IOpenIddictServerDispatcher dispatcher,
            IOpenIddictServerFactory factory,
            IOptionsMonitor<OpenIddictServerAspNetCoreOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _dispatcher = dispatcher;
            _factory = factory;
        }

        /// <inheritdoc/>
        public async Task<bool> HandleRequestAsync()
        {
            // Note: the transaction may be already attached when replaying an ASP.NET Core request
            // (e.g when using the built-in status code pages middleware with the re-execute mode).
            var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction;
            if (transaction is null)
            {
                // Create a new transaction and attach the HTTP request to make it available to the ASP.NET Core handlers.
                transaction = await _factory.CreateTransactionAsync();
                transaction.Properties[typeof(HttpRequest).FullName!] = new WeakReference<HttpRequest>(Request);

                // Attach the OpenIddict server transaction to the ASP.NET Core features
                // so that it can retrieved while performing sign-in/sign-out operations.
                Context.Features.Set(new OpenIddictServerAspNetCoreFeature { Transaction = transaction });
            }

            var context = new ProcessRequestContext(transaction);
            await _dispatcher.DispatchAsync(context);

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
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction ??
                throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));

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
                return AuthenticateResult.NoResult();
            }

            else if (context.IsRejected)
            {
                // Note: the missing_token error is special-cased to indicate to ASP.NET Core
                // that no authentication result could be produced due to the lack of token.
                // This also helps reducing the logging noise when no token is specified.
                if (string.Equals(context.Error, Errors.MissingToken, StringComparison.Ordinal))
                {
                    return AuthenticateResult.NoResult();
                }

                var properties = new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = context.Error,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = context.ErrorDescription,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorUri] = context.ErrorUri
                });

                return AuthenticateResult.Fail(SR.GetResourceString(SR.ID0113), properties);
            }

            else
            {
                Debug.Assert(context.Principal is not null, SR.GetResourceString(SR.ID4006));
                Debug.Assert(!string.IsNullOrEmpty(context.Principal.GetTokenType()), SR.GetResourceString(SR.ID4009));
                Debug.Assert(!string.IsNullOrEmpty(context.Token), SR.GetResourceString(SR.ID4010));

                // Store the token to allow any OWIN/Katana component (e.g a controller)
                // to retrieve it (e.g to make an API request to another application).
                var properties = new AuthenticationProperties();
                properties.StoreTokens(new[]
                {
                    new AuthenticationToken
                    {
                        Name = context.Principal.GetTokenType(),
                        Value = context.Token
                    }
                });

                return AuthenticateResult.Success(new AuthenticationTicket(
                    context.Principal, properties,
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
            }
        }

        /// <inheritdoc/>
        protected override async Task HandleChallengeAsync(AuthenticationProperties? properties)
        {
            var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction ??
                throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));

            transaction.Properties[typeof(AuthenticationProperties).FullName!] = properties ?? new AuthenticationProperties();

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

        /// <inheritdoc/>
        protected override Task HandleForbiddenAsync(AuthenticationProperties? properties)
            => HandleChallengeAsync(properties);

        /// <inheritdoc/>
        public async Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction ??
                throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));

            transaction.Properties[typeof(AuthenticationProperties).FullName!] = properties ?? new AuthenticationProperties();

            var context = new ProcessSignInContext(transaction)
            {
                Principal = user,
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

        /// <inheritdoc/>
        public async Task SignOutAsync(AuthenticationProperties? properties)
        {
            var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction ??
                throw new InvalidOperationException(SR.GetResourceString(SR.ID0112));

            var context = new ProcessSignOutContext(transaction)
            {
                Response = new OpenIddictResponse()
            };

            transaction.Properties[typeof(AuthenticationProperties).FullName!] = properties ?? new AuthenticationProperties();

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
