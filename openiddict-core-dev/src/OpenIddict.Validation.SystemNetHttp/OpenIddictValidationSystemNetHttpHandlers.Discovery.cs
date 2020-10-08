﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Collections.Immutable;
using static OpenIddict.Validation.OpenIddictValidationEvents;

namespace OpenIddict.Validation.SystemNetHttp
{
    public static partial class OpenIddictValidationSystemNetHttpHandlers
    {
        public static class Discovery
        {
            public static ImmutableArray<OpenIddictValidationHandlerDescriptor> DefaultHandlers { get; } = ImmutableArray.Create(
                /*
                 * Configuration request processing:
                 */
                PrepareGetHttpRequest<PrepareConfigurationRequestContext>.Descriptor,
                AttachQueryStringParameters<PrepareConfigurationRequestContext>.Descriptor,
                SendHttpRequest<ApplyConfigurationRequestContext>.Descriptor,

                /*
                 * Configuration response processing:
                 */
                ExtractJsonHttpResponse<ExtractConfigurationResponseContext>.Descriptor,

                /*
                 * Cryptography request processing:
                 */
                PrepareGetHttpRequest<PrepareCryptographyRequestContext>.Descriptor,
                AttachQueryStringParameters<PrepareCryptographyRequestContext>.Descriptor,
                SendHttpRequest<ApplyCryptographyRequestContext>.Descriptor,

                /*
                 * Configuration response processing:
                 */
                ExtractJsonHttpResponse<ExtractCryptographyResponseContext>.Descriptor);
        }
    }
}
