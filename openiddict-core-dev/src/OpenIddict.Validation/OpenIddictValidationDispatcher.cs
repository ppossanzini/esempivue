﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static OpenIddict.Validation.OpenIddictValidationEvents;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace OpenIddict.Validation
{
    public class OpenIddictValidationDispatcher : IOpenIddictValidationDispatcher
    {
        private readonly ILogger<OpenIddictValidationDispatcher> _logger;
        private readonly IOptionsMonitor<OpenIddictValidationOptions> _options;
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Creates a new instance of the <see cref="OpenIddictValidationDispatcher"/> class.
        /// </summary>
        public OpenIddictValidationDispatcher(
            ILogger<OpenIddictValidationDispatcher> logger,
            IOptionsMonitor<OpenIddictValidationOptions> options,
            IServiceProvider provider)
        {
            _logger = logger;
            _options = options;
            _provider = provider;
        }

        public async ValueTask DispatchAsync<TContext>(TContext context) where TContext : BaseContext
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            await foreach (var handler in GetHandlersAsync())
            {
                try
                {
                    await handler.HandleAsync(context);
                }

                catch (Exception exception) when (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(exception, SR.GetResourceString(SR.ID6132), handler.GetType().FullName, typeof(TContext).FullName);

                    throw;
                }

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(SR.GetResourceString(SR.ID6133), typeof(TContext).FullName, handler.GetType().FullName);
                }

                switch (context)
                {
                    case BaseRequestContext notification when notification.IsRequestHandled:
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug(SR.GetResourceString(SR.ID6134), typeof(TContext).FullName, handler.GetType().FullName);
                        }
                        return;

                    case BaseRequestContext notification when notification.IsRequestSkipped:
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug(SR.GetResourceString(SR.ID6135), typeof(TContext).FullName, handler.GetType().FullName);
                        }
                        return;

                    case BaseValidatingContext notification when notification.IsRejected:
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug(SR.GetResourceString(SR.ID6136), typeof(TContext).FullName, handler.GetType().FullName);
                        }
                        return;

                    default: continue;
                }
            }

            async IAsyncEnumerable<IOpenIddictValidationHandler<TContext>> GetHandlersAsync()
            {
                // Note: the descriptors collection is sorted during options initialization for performance reasons.
                var descriptors = _options.CurrentValue.Handlers;
                if (descriptors.Count == 0)
                {
                    yield break;
                }

                for (var index = 0; index < descriptors.Count; index++)
                {
                    var descriptor = descriptors[index];
                    if (descriptor.ContextType != typeof(TContext) || !await IsActiveAsync(descriptor))
                    {
                        continue;
                    }

                    var handler = descriptor.ServiceDescriptor.ImplementationInstance is not null ?
                        descriptor.ServiceDescriptor.ImplementationInstance as IOpenIddictValidationHandler<TContext> :
                        _provider.GetService(descriptor.ServiceDescriptor.ServiceType) as IOpenIddictValidationHandler<TContext>;

                    if (handler is null)
                    {
                        throw new InvalidOperationException(SR.FormatID0138(descriptor.ServiceDescriptor.ServiceType));
                    }

                    yield return handler;
                }
            }

            async ValueTask<bool> IsActiveAsync(OpenIddictValidationHandlerDescriptor descriptor)
            {
                for (var index = 0; index < descriptor.FilterTypes.Length; index++)
                {
                    if (!(_provider.GetService(descriptor.FilterTypes[index]) is IOpenIddictValidationHandlerFilter<TContext> filter))
                    {
                        throw new InvalidOperationException(SR.FormatID0099(descriptor.FilterTypes[index]));
                    }

                    if (!await filter.IsActiveAsync(context))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
