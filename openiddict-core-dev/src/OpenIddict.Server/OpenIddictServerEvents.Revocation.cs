﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;
using System.Security.Claims;
using OpenIddict.Abstractions;

namespace OpenIddict.Server
{
    public static partial class OpenIddictServerEvents
    {
        /// <summary>
        /// Represents an event called for each request to the revocation endpoint to give the user code
        /// a chance to manually extract the revocation request from the ambient HTTP context.
        /// </summary>
        public class ExtractRevocationRequestContext : BaseValidatingContext
        {
            /// <summary>
            /// Creates a new instance of the <see cref="ExtractRevocationRequestContext"/> class.
            /// </summary>
            public ExtractRevocationRequestContext(OpenIddictServerTransaction transaction)
                : base(transaction)
            {
            }

            /// <summary>
            /// Gets or sets the request, or <c>null</c> if it wasn't extracted yet.
            /// </summary>
            public OpenIddictRequest? Request
            {
                get => Transaction.Request;
                set => Transaction.Request = value;
            }
        }

        /// <summary>
        /// Represents an event called for each request to the revocation endpoint
        /// to determine if the request is valid and should continue to be processed.
        /// </summary>
        public class ValidateRevocationRequestContext : BaseValidatingClientContext
        {
            /// <summary>
            /// Creates a new instance of the <see cref="ValidateRevocationRequestContext"/> class.
            /// </summary>
            public ValidateRevocationRequestContext(OpenIddictServerTransaction transaction)
                : base(transaction)
            {
            }

            /// <summary>
            /// Gets or sets the request.
            /// </summary>
            public OpenIddictRequest Request
            {
                get => Transaction.Request!;
                set => Transaction.Request = value;
            }

            /// <summary>
            /// Gets the optional token_type_hint parameter extracted from the
            /// revocation request, or <c>null</c> if it cannot be found.
            /// </summary>
            public string? TokenTypeHint => Request.TokenTypeHint;

            /// <summary>
            /// Gets or sets the security principal extracted from the revoked token, if available.
            /// </summary>
            public ClaimsPrincipal? Principal { get; set; }
        }

        /// <summary>
        /// Represents an event called for each validated revocation request
        /// to allow the user code to decide how the request should be handled.
        /// </summary>
        public class HandleRevocationRequestContext : BaseValidatingContext
        {
            /// <summary>
            /// Creates a new instance of the <see cref="HandleRevocationRequestContext"/> class.
            /// </summary>
            public HandleRevocationRequestContext(OpenIddictServerTransaction transaction)
                : base(transaction)
            {
            }

            /// <summary>
            /// Gets or sets the request.
            /// </summary>
            public OpenIddictRequest Request
            {
                get => Transaction.Request!;
                set => Transaction.Request = value;
            }

            /// <summary>
            /// Gets or sets the security principal extracted from the revoked token.
            /// </summary>
            public ClaimsPrincipal? Principal { get; set; }

            /// <summary>
            /// Gets the authentication ticket.
            /// </summary>
            public IDictionary<string, object> Claims { get; }
                = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Represents an event called before the revocation response is returned to the caller.
        /// </summary>
        public class ApplyRevocationResponseContext : BaseRequestContext
        {
            /// <summary>
            /// Creates a new instance of the <see cref="ApplyRevocationResponseContext"/> class.
            /// </summary>
            public ApplyRevocationResponseContext(OpenIddictServerTransaction transaction)
                : base(transaction)
            {
            }

            /// <summary>
            /// Gets or sets the request, or <c>null</c> if it couldn't be extracted.
            /// </summary>
            public OpenIddictRequest? Request
            {
                get => Transaction.Request;
                set => Transaction.Request = value;
            }

            /// <summary>
            /// Gets or sets the response.
            /// </summary>
            public OpenIddictResponse Response
            {
                get => Transaction.Response!;
                set => Transaction.Response = value;
            }

            /// <summary>
            /// Gets the error code returned to the client application.
            /// When the response indicates a successful response,
            /// this property returns <c>null</c>.
            /// </summary>
            public string? Error => Response.Error;
        }
    }
}
