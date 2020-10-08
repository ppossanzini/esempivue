/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Xunit;
using static OpenIddict.Abstractions.OpenIddictConstants;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace OpenIddict.Abstractions.Tests.Primitives
{
    public class OpenIddictExtensionsTests
    {
        [Fact]
        public void GetAcrValues_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.GetAcrValues());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData("mod-pr", new[] { "mod-pr" })]
        [InlineData("mod-pr ", new[] { "mod-pr" })]
        [InlineData(" mod-pr ", new[] { "mod-pr" })]
        [InlineData("mod-pr mod-mf", new[] { "mod-pr", "mod-mf" })]
        [InlineData("mod-pr     mod-mf", new[] { "mod-pr", "mod-mf" })]
        [InlineData("mod-pr mod-mf ", new[] { "mod-pr", "mod-mf" })]
        [InlineData(" mod-pr mod-mf", new[] { "mod-pr", "mod-mf" })]
        [InlineData("mod-pr mod-pr mod-mf", new[] { "mod-pr", "mod-mf" })]
        [InlineData("mod-pr MOD-PR mod-mf", new[] { "mod-pr", "MOD-PR", "mod-mf" })]
        public void GetAcrValues_ReturnsExpectedAcrValues(string value, string[] values)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                AcrValues = value
            };

            // Act and assert
            Assert.Equal(values, request.GetAcrValues());
        }

        [Fact]
        public void GetPrompts_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => request.GetPrompts());

            // Assert
            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData("login", new[] { "login" })]
        [InlineData("login ", new[] { "login" })]
        [InlineData(" login ", new[] { "login" })]
        [InlineData("login consent", new[] { "login", "consent" })]
        [InlineData("login     consent", new[] { "login", "consent" })]
        [InlineData("login consent ", new[] { "login", "consent" })]
        [InlineData(" login consent", new[] { "login", "consent" })]
        [InlineData("login login consent", new[] { "login", "consent" })]
        [InlineData("login LOGIN consent", new[] { "login", "LOGIN", "consent" })]
        public void GetPrompts_ReturnsExpectedPrompts(string value, string[] values)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                Prompt = value
            };

            // Act and assert
            Assert.Equal(values, request.GetPrompts());
        }

        [Fact]
        public void GetResponseTypes_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => request.GetResponseTypes());

            // Assert
            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData("code", new[] { "code" })]
        [InlineData("code ", new[] { "code" })]
        [InlineData(" code ", new[] { "code" })]
        [InlineData("code id_token", new[] { "code", "id_token" })]
        [InlineData("code     id_token", new[] { "code", "id_token" })]
        [InlineData("code id_token ", new[] { "code", "id_token" })]
        [InlineData(" code id_token", new[] { "code", "id_token" })]
        [InlineData("code code id_token", new[] { "code", "id_token" })]
        [InlineData("code CODE id_token", new[] { "code", "CODE", "id_token" })]
        public void GetResponseTypes_ReturnsExpectedResponseTypes(string value, string[] values)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                ResponseType = value
            };

            // Act and assert
            Assert.Equal(values, request.GetResponseTypes());
        }

        [Fact]
        public void GetScopes_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.GetScopes());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData("openid", new[] { "openid" })]
        [InlineData("openid ", new[] { "openid" })]
        [InlineData(" openid ", new[] { "openid" })]
        [InlineData("openid profile", new[] { "openid", "profile" })]
        [InlineData("openid     profile", new[] { "openid", "profile" })]
        [InlineData("openid profile ", new[] { "openid", "profile" })]
        [InlineData(" openid profile", new[] { "openid", "profile" })]
        [InlineData("openid openid profile", new[] { "openid", "profile" })]
        [InlineData("openid OPENID profile", new[] { "openid", "OPENID", "profile" })]
        public void GetScopes_ReturnsExpectedScopes(string scope, string[] scopes)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                Scope = scope
            };

            // Act and assert
            Assert.Equal(scopes, request.GetScopes());
        }

        [Fact]
        public void HasAcrValue_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.HasAcrValue("mod-mf"));

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void HasAcrValue_ThrowsAnExceptionForNullOrEmptyAcrValue(string value)
        {
            // Arrange
            var request = new OpenIddictRequest();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => request.HasAcrValue(value));

            Assert.Equal("value", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0177), exception.Message);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("mod-mf", true)]
        [InlineData("mod-mf mod-pr", true)]
        [InlineData(" mod-mf mod-pr", true)]
        [InlineData("mod-pr mod-mf", true)]
        [InlineData("mod-pr mod-mf ", true)]
        [InlineData("mod-pr mod-mf mod-cstm", true)]
        [InlineData("mod-pr mod-mf mod-cstm ", true)]
        [InlineData("mod-pr    mod-mf   mod-cstm ", true)]
        [InlineData("mod-pr", false)]
        [InlineData("mod-pr mod-cstm", false)]
        [InlineData("MOD-MF", false)]
        [InlineData("MOD-MF MOD-PR", false)]
        [InlineData(" MOD-MF MOD-PR", false)]
        [InlineData("MOD-PR MOD-MF", false)]
        [InlineData("MOD-PR MOD-MF ", false)]
        [InlineData("MOD-PR MOD-MF MOD-CSTM", false)]
        [InlineData("MOD-PR MOD-MF MOD-CSTM ", false)]
        [InlineData("MOD-PR    MOD-MF   MOD-CSTM ", false)]
        [InlineData("MOD-PR", false)]
        [InlineData("MOD-PR MOD-CSTM", false)]
        public void HasAcrValue_ReturnsExpectedResult(string value, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                AcrValues = value
            };

            // Act and assert
            Assert.Equal(result, request.HasAcrValue("mod-mf"));
        }

        [Fact]
        public void HasPrompt_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                request.HasPrompt(Prompts.Consent);
            });

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void HasPrompt_ThrowsAnExceptionForNullOrEmptyPrompt(string prompt)
        {
            // Arrange
            var request = new OpenIddictRequest();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => request.HasPrompt(prompt));

            Assert.Equal("prompt", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0178), exception.Message);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("consent", true)]
        [InlineData("consent login", true)]
        [InlineData(" consent login", true)]
        [InlineData("login consent", true)]
        [InlineData("login consent ", true)]
        [InlineData("login consent select_account", true)]
        [InlineData("login consent select_account ", true)]
        [InlineData("login    consent   select_account ", true)]
        [InlineData("login", false)]
        [InlineData("login select_account", false)]
        [InlineData("CONSENT", false)]
        [InlineData("CONSENT LOGIN", false)]
        [InlineData(" CONSENT LOGIN", false)]
        [InlineData("LOGIN CONSENT", false)]
        [InlineData("LOGIN CONSENT ", false)]
        [InlineData("LOGIN CONSENT SELECT_ACCOUNT", false)]
        [InlineData("LOGIN CONSENT SELECT_ACCOUNT ", false)]
        [InlineData("LOGIN    CONSENT   SELECT_ACCOUNT ", false)]
        [InlineData("LOGIN", false)]
        [InlineData("LOGIN SELECT_ACCOUNT", false)]
        public void HasPrompt_ReturnsExpectedResult(string prompt, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                Prompt = prompt
            };

            // Act and assert
            Assert.Equal(result, request.HasPrompt(Prompts.Consent));
        }

        [Fact]
        public void HasResponseType_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                request.HasResponseType(ResponseTypes.Code);
            });

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void HasResponseType_ThrowsAnExceptionForNullOrEmptyResponseType(string type)
        {
            // Arrange
            var request = new OpenIddictRequest();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => request.HasResponseType(type));

            Assert.Equal("type", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0179), exception.Message);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("code", true)]
        [InlineData("code id_token", true)]
        [InlineData(" code id_token", true)]
        [InlineData("id_token code", true)]
        [InlineData("id_token code ", true)]
        [InlineData("id_token code token", true)]
        [InlineData("id_token code token ", true)]
        [InlineData("id_token    code   token ", true)]
        [InlineData("id_token", false)]
        [InlineData("id_token token", false)]
        [InlineData("CODE", false)]
        [InlineData("CODE ID_TOKEN", false)]
        [InlineData(" CODE ID_TOKEN", false)]
        [InlineData("ID_TOKEN CODE", false)]
        [InlineData("ID_TOKEN CODE ", false)]
        [InlineData("ID_TOKEN CODE TOKEN", false)]
        [InlineData("ID_TOKEN CODE TOKEN ", false)]
        [InlineData("ID_TOKEN    CODE   TOKEN ", false)]
        [InlineData("ID_TOKEN", false)]
        [InlineData("ID_TOKEN TOKEN", false)]
        public void HasResponseType_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                ResponseType = type
            };

            // Act and assert
            Assert.Equal(result, request.HasResponseType(ResponseTypes.Code));
        }

        [Fact]
        public void HasScope_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                request.HasScope(Scopes.OpenId);
            });

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void HasScope_ThrowsAnExceptionForNullOrEmptyScope(string scope)
        {
            // Arrange
            var request = new OpenIddictRequest();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => request.HasScope(scope));

            Assert.Equal("scope", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0180), exception.Message);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("openid", true)]
        [InlineData("openid ", true)]
        [InlineData(" openid ", true)]
        [InlineData("openid profile", true)]
        [InlineData("openid     profile", true)]
        [InlineData("openid profile ", true)]
        [InlineData(" openid profile", true)]
        [InlineData("profile", false)]
        [InlineData("profile email", false)]
        [InlineData("OPENID", false)]
        [InlineData("OPENID ", false)]
        [InlineData(" OPENID ", false)]
        [InlineData("OPENID PROFILE", false)]
        [InlineData("OPENID     PROFILE", false)]
        [InlineData("OPENID PROFILE ", false)]
        [InlineData(" OPENID PROFILE", false)]
        [InlineData("PROFILE", false)]
        [InlineData("PROFILE EMAIL", false)]
        public void HasScope_ReturnsExpectedResult(string scope, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                Scope = scope
            };

            // Act and assert
            Assert.Equal(result, request.HasScope(Scopes.OpenId));
        }

        [Fact]
        public void IsNoneFlow_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsNoneFlow());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("none", true)]
        [InlineData("none ", true)]
        [InlineData(" none", true)]
        [InlineData("none id_token", false)]
        [InlineData(" none id_token", false)]
        [InlineData("none id_token ", false)]
        [InlineData(" none id_token ", false)]
        [InlineData("NONE", false)]
        [InlineData("NONE ", false)]
        [InlineData(" NONE", false)]
        [InlineData("NONE ID_TOKEN", false)]
        [InlineData(" NONE ID_TOKEN", false)]
        [InlineData("NONE ID_TOKEN ", false)]
        [InlineData(" NONE ID_TOKEN ", false)]
        public void IsNoneFlow_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                ResponseType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsNoneFlow());
        }

        [Fact]
        public void IsAuthorizationCodeFlow_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsAuthorizationCodeFlow());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("code", true)]
        [InlineData("code ", true)]
        [InlineData(" code", true)]
        [InlineData("code id_token", false)]
        [InlineData(" code id_token", false)]
        [InlineData("code id_token ", false)]
        [InlineData(" code id_token ", false)]
        [InlineData("CODE", false)]
        [InlineData("CODE ", false)]
        [InlineData(" CODE", false)]
        [InlineData("CODE ID_TOKEN", false)]
        [InlineData(" CODE ID_TOKEN", false)]
        [InlineData("CODE ID_TOKEN ", false)]
        [InlineData(" CODE ID_TOKEN ", false)]
        public void IsAuthorizationCodeFlow_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                ResponseType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsAuthorizationCodeFlow());
        }

        [Fact]
        public void IsImplicitFlow_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsImplicitFlow());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("id_token", true)]
        [InlineData("id_token ", true)]
        [InlineData(" id_token", true)]
        [InlineData("id_token token", true)]
        [InlineData(" id_token token", true)]
        [InlineData("id_token token ", true)]
        [InlineData(" id_token token ", true)]
        [InlineData("token", true)]
        [InlineData("token ", true)]
        [InlineData(" token", true)]
        [InlineData("code id_token", false)]
        [InlineData("code id_token token", false)]
        [InlineData("code token", false)]
        [InlineData("ID_TOKEN", false)]
        [InlineData("ID_TOKEN ", false)]
        [InlineData(" ID_TOKEN", false)]
        [InlineData("ID_TOKEN TOKEN", false)]
        [InlineData(" ID_TOKEN TOKEN", false)]
        [InlineData("ID_TOKEN TOKEN ", false)]
        [InlineData(" ID_TOKEN TOKEN ", false)]
        [InlineData("TOKEN", false)]
        [InlineData("TOKEN ", false)]
        [InlineData(" TOKEN", false)]
        [InlineData("CODE ID_TOKEN", false)]
        [InlineData("CODE ID_TOKEN TOKEN", false)]
        [InlineData("CODE TOKEN", false)]
        public void IsImplicitFlow_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                ResponseType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsImplicitFlow());
        }

        [Fact]
        public void IsHybridFlow_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsHybridFlow());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("code id_token", true)]
        [InlineData("code id_token ", true)]
        [InlineData(" code id_token", true)]
        [InlineData("code id_token token", true)]
        [InlineData(" code id_token token", true)]
        [InlineData("code id_token token ", true)]
        [InlineData(" code id_token token ", true)]
        [InlineData(" code  id_token  token ", true)]
        [InlineData("code token", true)]
        [InlineData("code token ", true)]
        [InlineData(" code token", true)]
        [InlineData("id_token", false)]
        [InlineData("id_token token", false)]
        [InlineData("token", false)]
        [InlineData("CODE ID_TOKEN", false)]
        [InlineData("CODE ID_TOKEN ", false)]
        [InlineData(" CODE ID_TOKEN", false)]
        [InlineData("CODE ID_TOKEN TOKEN", false)]
        [InlineData(" CODE ID_TOKEN TOKEN", false)]
        [InlineData("CODE ID_TOKEN TOKEN ", false)]
        [InlineData(" CODE ID_TOKEN TOKEN ", false)]
        [InlineData(" CODE  ID_TOKEN  TOKEN ", false)]
        [InlineData("CODE TOKEN", false)]
        [InlineData("CODE TOKEN ", false)]
        [InlineData(" CODE TOKEN", false)]
        [InlineData("ID_TOKEN", false)]
        [InlineData("ID_TOKEN TOKEN", false)]
        [InlineData("TOKEN", false)]
        public void IsHybridFlow_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                ResponseType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsHybridFlow());
        }

        [Fact]
        public void IsFragmentResponseMode_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsFragmentResponseMode());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("unknown", null, false)]
        [InlineData("query", null, false)]
        [InlineData("form_post", null, false)]
        [InlineData("fragment", null, true)]
        [InlineData("fragment ", null, false)]
        [InlineData(" fragment", null, false)]
        [InlineData(" fragment ", null, false)]
        [InlineData(null, "code", false)]
        [InlineData(null, "code id_token", true)]
        [InlineData(null, "code id_token token", true)]
        [InlineData(null, "code token", true)]
        [InlineData(null, "id_token", true)]
        [InlineData(null, "id_token token", true)]
        [InlineData(null, "token", true)]
        [InlineData("QUERY", null, false)]
        [InlineData("FRAGMENT", null, false)]
        [InlineData("FORM_POST", null, false)]
        [InlineData(null, "CODE", false)]
        [InlineData(null, "CODE ID_TOKEN", false)]
        [InlineData(null, "CODE ID_TOKEN TOKEN", false)]
        [InlineData(null, "CODE TOKEN", false)]
        [InlineData(null, "ID_TOKEN", false)]
        [InlineData(null, "ID_TOKEN TOKEN", false)]
        [InlineData(null, "TOKEN", false)]
        public void IsFragmentResponseMode_ReturnsExpectedResult(string mode, string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                ResponseMode = mode,
                ResponseType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsFragmentResponseMode());
        }

        [Fact]
        public void IsQueryResponseMode_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsQueryResponseMode());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("unknown", null, false)]
        [InlineData("query", null, true)]
        [InlineData("query ", null, false)]
        [InlineData(" query", null, false)]
        [InlineData(" query ", null, false)]
        [InlineData("fragment", null, false)]
        [InlineData("form_post", null, false)]
        [InlineData(null, "none", true)]
        [InlineData(null, "code", true)]
        [InlineData(null, "code id_token token", false)]
        [InlineData(null, "code token", false)]
        [InlineData(null, "id_token", false)]
        [InlineData(null, "id_token token", false)]
        [InlineData(null, "token", false)]
        [InlineData("QUERY", null, false)]
        [InlineData("FRAGMENT", null, false)]
        [InlineData("FORM_POST", null, false)]
        [InlineData(null, "CODE", false)]
        [InlineData(null, "CODE ID_TOKEN", false)]
        [InlineData(null, "CODE ID_TOKEN TOKEN", false)]
        [InlineData(null, "CODE TOKEN", false)]
        [InlineData(null, "ID_TOKEN", false)]
        [InlineData(null, "ID_TOKEN TOKEN", false)]
        [InlineData(null, "TOKEN", false)]
        public void IsQueryResponseMode_ReturnsExpectedResult(string mode, string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                ResponseMode = mode,
                ResponseType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsQueryResponseMode());
        }

        [Fact]
        public void IsFormPostResponseMode_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsFormPostResponseMode());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("query", false)]
        [InlineData("fragment", false)]
        [InlineData("form_post", true)]
        [InlineData("form_post ", false)]
        [InlineData(" form_post", false)]
        [InlineData(" form_post ", false)]
        [InlineData("QUERY", false)]
        [InlineData("FRAGMENT", false)]
        [InlineData("FORM_POST", false)]
        public void IsFormPostResponseMode_ReturnsExpectedResult(string mode, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                ResponseMode = mode
            };

            // Act and assert
            Assert.Equal(result, request.IsFormPostResponseMode());
        }

        [Fact]
        public void IsAuthorizationCodeGrantType_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsAuthorizationCodeGrantType());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("authorization_code", true)]
        [InlineData("authorization_code ", false)]
        [InlineData(" authorization_code", false)]
        [InlineData(" authorization_code ", false)]
        [InlineData("client_credentials", false)]
        [InlineData("password", false)]
        [InlineData("refresh_token", false)]
        [InlineData("AUTHORIZATION_CODE", false)]
        [InlineData("CLIENT_CREDENTIALS", false)]
        [InlineData("PASSWORD", false)]
        [InlineData("REFRESH_TOKEN", false)]
        [InlineData("urn:ietf:params:oauth:grant-type:device_code", false)]
        public void IsAuthorizationCodeGrantType_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                GrantType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsAuthorizationCodeGrantType());
        }

        [Fact]
        public void IsClientCredentialsGrantType_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsClientCredentialsGrantType());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("authorization_code", false)]
        [InlineData("client_credentials", true)]
        [InlineData("client_credentials ", false)]
        [InlineData(" client_credentials", false)]
        [InlineData(" client_credentials ", false)]
        [InlineData("password", false)]
        [InlineData("refresh_token", false)]
        [InlineData("AUTHORIZATION_CODE", false)]
        [InlineData("CLIENT_CREDENTIALS", false)]
        [InlineData("PASSWORD", false)]
        [InlineData("REFRESH_TOKEN", false)]
        [InlineData("urn:ietf:params:oauth:grant-type:device_code", false)]
        public void IsClientCredentialsGrantType_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                GrantType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsClientCredentialsGrantType());
        }

        [Fact]
        public void IsDeviceCodeGrantType_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsDeviceCodeGrantType());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("authorization_code", false)]
        [InlineData("client_credentials", false)]
        [InlineData("password", false)]
        [InlineData("refresh_token", false)]
        [InlineData("AUTHORIZATION_CODE", false)]
        [InlineData("CLIENT_CREDENTIALS", false)]
        [InlineData("PASSWORD", false)]
        [InlineData("REFRESH_TOKEN", false)]
        [InlineData("urn:ietf:params:oauth:grant-type:device_code", true)]
        [InlineData("urn:ietf:params:oauth:grant-type:device_code ", false)]
        [InlineData(" urn:ietf:params:oauth:grant-type:device_code", false)]
        [InlineData(" urn:ietf:params:oauth:grant-type:device_code ", false)]
        public void IsDeviceCodeGrantType_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                GrantType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsDeviceCodeGrantType());
        }

        [Fact]
        public void IsPasswordGrantType_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsPasswordGrantType());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("authorization_code", false)]
        [InlineData("client_credentials", false)]
        [InlineData("password", true)]
        [InlineData("password ", false)]
        [InlineData(" password", false)]
        [InlineData(" password ", false)]
        [InlineData("refresh_token", false)]
        [InlineData("AUTHORIZATION_CODE", false)]
        [InlineData("CLIENT_CREDENTIALS", false)]
        [InlineData("PASSWORD", false)]
        [InlineData("REFRESH_TOKEN", false)]
        [InlineData("urn:ietf:params:oauth:grant-type:device_code", false)]
        public void IsPasswordGrantType_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                GrantType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsPasswordGrantType());
        }

        [Fact]
        public void IsRefreshTokenGrantType_ThrowsAnExceptionForNullRequest()
        {
            // Arrange
            var request = (OpenIddictRequest) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => request.IsRefreshTokenGrantType());

            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("unknown", false)]
        [InlineData("authorization_code", false)]
        [InlineData("client_credentials", false)]
        [InlineData("password", false)]
        [InlineData("refresh_token", true)]
        [InlineData("refresh_token ", false)]
        [InlineData(" refresh_token", false)]
        [InlineData(" refresh_token ", false)]
        [InlineData("AUTHORIZATION_CODE", false)]
        [InlineData("CLIENT_CREDENTIALS", false)]
        [InlineData("PASSWORD", false)]
        [InlineData("REFRESH_TOKEN", false)]
        [InlineData("urn:ietf:params:oauth:grant-type:device_code", false)]
        public void IsRefreshTokenGrantType_ReturnsExpectedResult(string type, bool result)
        {
            // Arrange
            var request = new OpenIddictRequest
            {
                GrantType = type
            };

            // Act and assert
            Assert.Equal(result, request.IsRefreshTokenGrantType());
        }

        [Fact]
        public void Claim_GetDestinations_ThrowsAnExceptionForNullClaim()
        {
            // Arrange
            var claim = (Claim) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => claim.GetDestinations());

            Assert.Equal("claim", exception.ParamName);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData("", new string[0])]
        [InlineData("[]", new string[0])]
        [InlineData(@"[""id_token""]", new[] { "id_token" })]
        [InlineData(@"[""access_token"",""id_token""]", new[] { "access_token", "id_token" })]
        [InlineData(@"[""access_token"",""access_token"",""id_token""]", new[] { "access_token", "id_token" })]
        [InlineData(@"[""access_token"",""ACCESS_TOKEN"",""id_token""]", new[] { "access_token", "id_token" })]
        public void Claim_GetDestinations_ReturnsExpectedDestinations(string destination, string[] destinations)
        {
            // Arrange
            var claim = new Claim(Claims.Name, "Bob le Bricoleur");
            claim.Properties[Properties.Destinations] = destination;

            // Act and assert
            Assert.Equal(destinations, claim.GetDestinations());
        }

        [Fact]
        public void Claim_HasDestination_ThrowsAnExceptionForNullClaim()
        {
            // Arrange
            var claim = (Claim) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => claim.HasDestination("destination"));

            Assert.Equal("claim", exception.ParamName);
        }

        [Fact]
        public void Claim_HasDestination_ThrowsAnExceptionForNullOrEmptyDestination()
        {
            // Arrange
            var claim = new Claim(Claims.Name, "Bob le Bricoleur");

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => claim.HasDestination(null));

            Assert.Equal("destination", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0181), exception.Message);
        }

        [Fact]
        public void Claim_HasDestination_ReturnFalseForNullOrEmptyDestinations()
        {
            // Arrange
            var claim = new Claim(Claims.Name, "Bob le Bricoleur");

            // Act
            var hasDestination = claim.HasDestination("destination");

            // Assert
            Assert.False(hasDestination);
        }

        [Fact]
        public void Claim_HasDestination_ReturnTrueForExistingDestination()
        {
            // Arrange
            var claim = new Claim(Claims.Name, "Bob le Bricoleur");
            claim.SetDestinations(new[] { "destination1", "destination2", "destination3" });

            // Act
            var hasDestination = claim.HasDestination("destination2");

            // Assert
            Assert.True(hasDestination);
        }


        [Fact]
        public void Claim_SetDestinations_ThrowsAnExceptionForNullClaim()
        {
            // Arrange
            var claim = (Claim) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => claim.SetDestinations());

            Assert.Equal("claim", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new object[] { new string[0] })]
        public void Claim_SetDestinations_RemovesPropertyForEmptyArray(string[] destinations)
        {
            // Arrange
            var claim = new Claim(Claims.Name, "Bob le Bricoleur");

            // Act
            claim.SetDestinations(destinations);

            // Assert
            Assert.Equal(0, claim.Properties.Count);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Claim_SetDestinations_ThrowsAnExceptionForNullOrEmptyDestinations(string destination)
        {
            // Arrange
            var claim = new Claim(Claims.Name, "Bob le Bricoleur");

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => claim.SetDestinations(destination));

            Assert.Equal("destinations", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0182), exception.Message);
        }

        [Theory]
        [InlineData(new[] { "access_token" }, @"[""access_token""]")]
        [InlineData(new[] { "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "ACCESS_TOKEN", "id_token" }, @"[""access_token"",""id_token""]")]
        public void Claim_SetDestinations_SetsAppropriateDestinations(string[] destinations, string destination)
        {
            // Arrange
            var claim = new Claim(Claims.Name, "Bob le Bricoleur");

            // Act
            claim.SetDestinations(destinations);

            // Assert
            Assert.Equal(destination, claim.Properties[Properties.Destinations]);
        }

        [Fact]
        public void ClaimsPrincipal_GetDestinations_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetDestinations());

            Assert.Equal("principal", exception.ParamName);
        }

        [Fact]
        public void ClaimsPrincipal_GetDestinations_ReturnsExpectedDestinations()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(Claims.Name, "Bob le Bricoleur")
                {
                    Properties =
                    {
                        [Properties.Destinations] = @"[""access_token"",""id_token""]"
                    }
                },
                new Claim(Claims.Email, "bob@bricoleur.com")
                {
                    Properties =
                    {
                        [Properties.Destinations] = @"[""id_token""]"
                    }
                },
                new Claim(Claims.Nonce, "OkjjKJkjkHJJHhgFsd")
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var destinations = principal.GetDestinations();

            // Assert
            Assert.Equal(2, destinations.Count);
            Assert.Equal(new[] { Destinations.AccessToken, Destinations.IdentityToken }, destinations[Claims.Name]);
            Assert.Equal(new[] { Destinations.IdentityToken }, destinations[Claims.Email]);
        }

        [Fact]
        public void ClaimsPrincipal_SetDestinations_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetDestinations(destinations: null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Fact]
        public void ClaimsPrincipal_SetDestinations_ThrowsAnExceptionForNullDestinations()
        {
            // Arrange
            var principal = new ClaimsPrincipal(new ClaimsIdentity());
            var destinations = (ImmutableDictionary<string, string[]>) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetDestinations(destinations));

            Assert.Equal("destinations", exception.ParamName);
        }

        [Fact]
        public void ClaimsPrincipal_SetDestinations_SetsAppropriateDestinations()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(Claims.Name, "Bob le Bricoleur"),
                new Claim(Claims.Email, "bob@bricoleur.com"),
                new Claim(Claims.Nonce, "OkjjKJkjkHJJHhgFsd")
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var destinations = ImmutableDictionary.CreateBuilder<string, string[]>(StringComparer.Ordinal);
            destinations.Add(Claims.Name, new[] { Destinations.AccessToken, Destinations.IdentityToken });
            destinations.Add(Claims.Email, new[] { Destinations.IdentityToken });
            destinations.Add(Claims.Nonce, Array.Empty<string>());

            // Act
            principal.SetDestinations(destinations.ToImmutable());

            // Assert
            Assert.Equal(@"[""access_token"",""id_token""]", principal.FindFirst(Claims.Name).Properties[Properties.Destinations]);
            Assert.Equal(@"[""id_token""]", principal.FindFirst(Claims.Email).Properties[Properties.Destinations]);
            Assert.DoesNotContain(Properties.Destinations, principal.FindFirst(Claims.Nonce).Properties);
        }

        [Theory]
        [InlineData(new[] { "access_token" }, @"[""access_token""]")]
        [InlineData(new[] { "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "ACCESS_TOKEN", "id_token" }, @"[""access_token"",""id_token""]")]
        public void SetDestinations_IEnumerable_SetsAppropriateDestinations(string[] destinations, string destination)
        {
            // Arrange
            var claim = new Claim(Claims.Name, "Bob le Bricoleur");

            // Act
            claim.SetDestinations((IEnumerable<string>) destinations);

            // Assert
            Assert.Equal(destination, claim.Properties[Properties.Destinations]);
        }

        [Theory]
        [InlineData(new[] { "access_token" }, @"[""access_token""]")]
        [InlineData(new[] { "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "ACCESS_TOKEN", "id_token" }, @"[""access_token"",""id_token""]")]
        public void SetDestinations_ImmutableArray_SetsAppropriateDestinations(string[] destinations, string destination)
        {
            // Arrange
            var claim = new Claim(Claims.Name, "Bob le Bricoleur");

            // Act
            claim.SetDestinations(ImmutableArray.Create(destinations));

            // Assert
            Assert.Equal(destination, claim.Properties[Properties.Destinations]);
        }

        [Fact]
        public void ClaimsIdentity_Clone_ReturnsDifferentInstanceWithFilteredClaims()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(Claims.Name, "Bob le Bricoleur"));
            identity.AddClaim(new Claim(Claims.ClientId, "B56BF6CE-8D8C-4290-A0E7-A4F8EE0A9FC4"));

            // Act
            var clone = identity.Clone(claim => claim.Type == Claims.Name);
            clone.AddClaim(new Claim("clone_claim", "value"));

            // Assert
            Assert.NotSame(identity, clone);
            Assert.Null(identity.FindFirst("clone_claim"));
            Assert.NotNull(clone.FindFirst(Claims.Name));
            Assert.Null(clone.FindFirst(Claims.ClientId));
        }

        [Fact]
        public void ClaimsIdentity_Clone_ExcludesUnwantedClaims()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(Claims.Name, "Bob le Bricoleur"));
            identity.AddClaim(new Claim(Claims.Subject, "D8F1A010-BD46-4F8F-AD4E-05582307F8F4"));

            // Act
            var clone = identity.Clone(claim => claim.Type == Claims.Name);

            // Assert
            Assert.Single(clone.Claims);
            Assert.Null(clone.FindFirst(Claims.Subject));
            Assert.Equal("Bob le Bricoleur", clone.FindFirst(Claims.Name).Value);
        }

        [Fact]
        public void ClaimsIdentity_Clone_ExcludesUnwantedClaimsFromActor()
        {
            // Arrange
            var identity = new ClaimsIdentity
            {
                Actor = new ClaimsIdentity()
            };
            identity.Actor.AddClaim(new Claim(Claims.Name, "Bob le Bricoleur"));
            identity.Actor.AddClaim(new Claim(Claims.Subject, "D8F1A010-BD46-4F8F-AD4E-05582307F8F4"));

            // Act
            var clone = identity.Clone(claim => claim.Type == Claims.Name);

            // Assert
            Assert.Single(clone.Actor.Claims);
            Assert.Null(clone.Actor.FindFirst(Claims.Subject));
            Assert.Equal("Bob le Bricoleur", clone.Actor.FindFirst(Claims.Name).Value);
        }

        [Fact]
        public void ClaimsPrincipal_Clone_ExcludesUnwantedClaimsFromIdentities()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(Claims.Name, "Bob le Bricoleur"));
            identity.AddClaim(new Claim(Claims.Subject, "D8F1A010-BD46-4F8F-AD4E-05582307F8F4"));

            var principal = new ClaimsPrincipal(identity);

            // Act
            var clone = principal.Clone(claim => claim.Type == Claims.Name);

            // Assert
            Assert.Single(clone.Claims);
            Assert.Null(clone.FindFirst(Claims.Subject));
            Assert.Equal("Bob le Bricoleur", clone.FindFirst(Claims.Name).Value);
        }

        [Fact]
        public void AddClaim_ThrowsAnExceptionForNullIdentity()
        {
            // Arrange
            var identity = (ClaimsIdentity) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                identity.AddClaim(Claims.Name, "Bob le Bricoleur");
            });

            Assert.Equal("identity", exception.ParamName);
        }

        [Fact]
        public void AddClaim_SetsAppropriateClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();

            // Act
            identity.AddClaim(Claims.Name, "Bob le Bricoleur");

            // Assert
            Assert.Equal("Bob le Bricoleur", identity.FindFirst(Claims.Name).Value);
        }

        [Theory]
        [InlineData(new[] { "access_token" }, @"[""access_token""]")]
        [InlineData(new[] { "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "ACCESS_TOKEN", "id_token" }, @"[""access_token"",""id_token""]")]
        public void AddClaim_ImmutableArray_SetsAppropriateDestinations(string[] destinations, string destination)
        {
            // Arrange
            var identity = new ClaimsIdentity();

            // Act
            identity.AddClaim(Claims.Name, "Bob le Bricoleur", ImmutableArray.Create(destinations));

            var claim = identity.FindFirst(Claims.Name);

            // Assert
            Assert.Equal("Bob le Bricoleur", claim.Value);
            Assert.Equal(destination, claim.Properties[Properties.Destinations]);
        }

        [Theory]
        [InlineData(new[] { "access_token" }, @"[""access_token""]")]
        [InlineData(new[] { "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "access_token", "id_token" }, @"[""access_token"",""id_token""]")]
        [InlineData(new[] { "access_token", "ACCESS_TOKEN", "id_token" }, @"[""access_token"",""id_token""]")]
        public void AddClaim_SetsAppropriateDestinations(string[] destinations, string destination)
        {
            // Arrange
            var identity = new ClaimsIdentity();

            // Act
            identity.AddClaim(Claims.Name, "Bob le Bricoleur", destinations);

            var claim = identity.FindFirst(Claims.Name);

            // Assert
            Assert.Equal("Bob le Bricoleur", claim.Value);
            Assert.Equal(destination, claim.Properties[Properties.Destinations]);
        }

        [Fact]
        public void GetClaim_ThrowsAnExceptionForNullIdentity()
        {
            // Arrange
            var identity = (ClaimsIdentity) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                identity.GetClaim(Claims.Name);
            });

            Assert.Equal("identity", exception.ParamName);
        }

        [Fact]
        public void GetClaim_ReturnsNullForMissingClaims()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal();

            // Act and assert
            Assert.Null(identity.GetClaim(Claims.Name));
            Assert.Null(principal.GetClaim(Claims.Name));
        }

        [Fact]
        public void GetClaim_ReturnsAppropriateResult()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            identity.AddClaim(Claims.Name, "Bob le Bricoleur");

            // Act and assert
            Assert.Equal("Bob le Bricoleur", identity.GetClaim(Claims.Name));
            Assert.Equal("Bob le Bricoleur", principal.GetClaim(Claims.Name));
        }

        [Fact]
        public void ClaimsIdentity_Clone_ThrowsAnExceptionForNullIdentity()
        {
            // Arrange
            var identity = (ClaimsIdentity) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => identity.Clone(claim => true));

            Assert.Equal("identity", exception.ParamName);
        }

        [Fact]
        public void ClaimsIdentity_Clone_ReturnsIdenticalIdentity()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim("type", "value");

            // Act
            var copy = identity.Clone(claim => true);

            // Assert
            Assert.Equal("value", copy.GetClaim("type"));
            Assert.Equal(identity.Claims.Count(), copy.Claims.Count());
        }

        [Fact]
        public void ClaimsPrincipal_Clone_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.Clone(claim => true));

            Assert.Equal("principal", exception.ParamName);
        }

        [Fact]
        public void ClaimsPrincipal_Clone_ReturnsIdenticalPrincipal()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(Claims.Name, "Bob le Bricoleur"));

            var principal = new ClaimsPrincipal(identity);

            // Act
            var copy = principal.Clone(claim => true);

            // Assert
            Assert.Equal("Bob le Bricoleur", copy.GetClaim(Claims.Name));
            Assert.Equal(principal.Claims.Count(), copy.Claims.Count());
        }

        [Fact]
        public void ClaimsIdentity_Clone_ReturnsDifferentIdentityInstance()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim("type", "value");

            // Act
            var copy = identity.Clone(claim => true);
            copy.AddClaim("clone_type", "value");

            // Assert
            Assert.NotSame(identity, copy);
            Assert.Null(identity.FindFirst("clone_type"));
        }

        [Fact]
        public void ClaimsPrincipal_Clone_ReturnsDifferentPrincipalInstance()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(Claims.Name, "Bob le Bricoleur"));

            var principal = new ClaimsPrincipal(identity);

            // Act
            var copy = principal.Clone(claim => true);
            copy.SetClaim("clone_claim", "value");

            // Assert
            Assert.NotSame(principal, copy);
            Assert.Null(principal.FindFirst("clone_claim"));
        }

        [Fact]
        public void GetClaim_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetClaim("type"));

            Assert.Equal("principal", exception.ParamName);
        }

        [Fact]
        public void GetClaim_ReturnsNullForMissingClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act and assert
            Assert.Null(principal.GetClaim("type"));
        }

        [Fact]
        public void GetClaim_IsCaseInsensitive()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            principal.SetClaim("type", "value");

            // Act and assert
            Assert.Equal("value", principal.GetClaim("TYPE"));
        }

        [Fact]
        public void GetCreationDate_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetCreationDate());

            Assert.Equal("principal", exception.ParamName);
        }

        [Fact]
        public void GetCreationDate_ReturnsNullIfNoClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act and assert
            Assert.Null(principal.GetCreationDate());
        }

        [Fact]
        public void GetCreationDate_ReturnsCreationDate()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            principal.SetClaim(Claims.Private.CreationDate, "Wed, 01 Jan 2020 04:30:30 GMT");

            // Act
            var date = principal.GetCreationDate();

            // Assert
            Assert.Equal(new DateTimeOffset(2020, 01, 01, 05, 30, 30, TimeSpan.FromHours(1)), date);
        }

        [Fact]
        public void GetExpirationDate_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetExpirationDate());

            Assert.Equal("principal", exception.ParamName);
        }

        [Fact]
        public void GetExpirationDate_ReturnsNullIfNoClaim()
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act and assert
            Assert.Null(principal.GetExpirationDate());
        }

        [Fact]
        public void GetExpirationDate_ReturnsExpirationDate()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            principal.SetClaim(Claims.Private.ExpirationDate, "Wed, 01 Jan 2020 04:30:30 GMT");

            // Act
            var date = principal.GetExpirationDate();

            // Assert
            Assert.Equal(new DateTimeOffset(2020, 01, 01, 05, 30, 30, TimeSpan.FromHours(1)), date);
        }

        [Fact]
        public void GetAudiences_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetAudiences());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "fabrikam" }, new[] { "fabrikam" })]
        [InlineData(new[] { "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "FABRIKAM", "contoso" }, new[] { "fabrikam", "FABRIKAM", "contoso" })]
        public void GetAudiences_ReturnsExpectedAudiences(string[] audience, string[] audiences)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Audience, audience.ToImmutableArray());

            // Act and assert
            Assert.Equal(audiences, principal.GetAudiences());
        }

        [Fact]
        public void GetPresenters_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetPresenters());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "fabrikam" }, new[] { "fabrikam" })]
        [InlineData(new[] { "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "FABRIKAM", "contoso" }, new[] { "fabrikam", "FABRIKAM", "contoso" })]
        public void GetPresenters_ReturnsExpectedPresenters(string[] presenter, string[] presenters)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Presenter, presenter.ToImmutableArray());

            // Act and assert
            Assert.Equal(presenters, principal.GetPresenters());
        }

        [Fact]
        public void GetResources_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetResources());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "fabrikam" }, new[] { "fabrikam" })]
        [InlineData(new[] { "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "FABRIKAM", "contoso" }, new[] { "fabrikam", "FABRIKAM", "contoso" })]
        public void GetResources_ReturnsExpectedResources(string[] resource, string[] resources)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Resource, resource.ToImmutableArray());

            // Act and assert
            Assert.Equal(resources, principal.GetResources());
        }

        [Fact]
        public void GetScopes_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetScopes());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "openid" }, new[] { "openid" })]
        [InlineData(new[] { "openid", "profile" }, new[] { "openid", "profile" })]
        [InlineData(new[] { "openid", "openid", "profile" }, new[] { "openid", "profile" })]
        [InlineData(new[] { "openid", "OPENID", "profile" }, new[] { "openid", "OPENID", "profile" })]
        public void ClaimsPrincipal_GetScopes_ReturnsExpectedScopes(string[] scope, string[] scopes)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Scope, scope.ToImmutableArray());

            // Act and assert
            Assert.Equal(scopes, principal.GetScopes());
        }

        [Fact]
        public void GetAccessTokenLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetAccessTokenLifetime());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void GetAccessTokenLifetime_ReturnsExpectedResult(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaim(Claims.Private.AccessTokenLifetime, lifetime);

            // Act and assert
            Assert.Equal(ParseLifeTime(lifetime), principal.GetAccessTokenLifetime());
        }

        [Fact]
        public void GetAuthorizationCodeLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetAuthorizationCodeLifetime());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void GetAuthorizationCodeLifetime_ReturnsExpectedResult(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaim(Claims.Private.AuthorizationCodeLifetime, lifetime);

            // Act and assert
            Assert.Equal(ParseLifeTime(lifetime), principal.GetAuthorizationCodeLifetime());
        }

        [Fact]
        public void GetDeviceCodeLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetDeviceCodeLifetime());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void GetDeviceCodeLifetime_ReturnsExpectedResult(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaim(Claims.Private.DeviceCodeLifetime, lifetime);

            // Act and assert
            Assert.Equal(ParseLifeTime(lifetime), principal.GetDeviceCodeLifetime());
        }

        [Fact]
        public void GetIdentityTokenLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetIdentityTokenLifetime());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void GetIdentityTokenLifetime_ReturnsExpectedResult(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaim(Claims.Private.IdentityTokenLifetime, lifetime);

            // Act and assert
            Assert.Equal(ParseLifeTime(lifetime), principal.GetIdentityTokenLifetime());
        }

        [Fact]
        public void GetRefreshTokenLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetRefreshTokenLifetime());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void GetRefreshTokenLifetime_ReturnsExpectedResult(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaim(Claims.Private.RefreshTokenLifetime, lifetime);

            // Act and assert
            Assert.Equal(ParseLifeTime(lifetime), principal.GetRefreshTokenLifetime());
        }

        [Fact]
        public void GetUserCodeLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetUserCodeLifetime());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void GetUserCodeLifetime_ReturnsExpectedResult(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaim(Claims.Private.UserCodeLifetime, lifetime);

            // Act and assert
            Assert.Equal(ParseLifeTime(lifetime), principal.GetUserCodeLifetime());
        }

        [Fact]
        public void GetAuthorizationId_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetAuthorizationId());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("identifier")]
        public void GetAuthorizationId_ReturnsExpectedResult(string identifier)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaim(Claims.Private.AuthorizationId, identifier);

            // Act and assert
            Assert.Equal(identifier, principal.GetAuthorizationId());
        }

        [Fact]
        public void GetTokenId_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetTokenId());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("identifier")]
        public void GetTokenId_ReturnsExpectedResult(string identifier)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaim(Claims.Private.TokenId, identifier);

            // Act and assert
            Assert.Equal(identifier, principal.GetTokenId());
        }

        [Fact]
        public void GetTokenType_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetTokenType());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("access_token")]
        public void GetTokenType_ReturnsExpectedResult(string type)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetTokenType(type);

            // Act and assert
            Assert.Equal(type, principal.GetTokenType());
        }

        [Fact]
        public void HasAudience_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.HasAudience("Fabrikam"));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void HasAudience_ThrowsAnExceptionForNullOrEmptyAudience(string audience)
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => principal.HasAudience(audience));

            Assert.Equal("audience", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0186), exception.Message);
        }

        [Theory]
        [InlineData(new string[0], false)]
        [InlineData(new[] { "fabrikam" }, true)]
        public void HasAudience_ReturnsExpectedResult(string[] audience, bool result)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Audience, audience.ToImmutableArray());

            // Act and assert
            Assert.Equal(result, principal.HasAudience());
        }

        [Theory]
        [InlineData(new string[0], false)]
        [InlineData(new[] { "contoso" }, false)]
        [InlineData(new[] { "contoso", "fabrikam" }, true)]
        [InlineData(new[] { "fabrikam" }, true)]
        [InlineData(new[] { "fabrikam", "contoso" }, true)]
        [InlineData(new[] { "CONTOSO" }, false)]
        [InlineData(new[] { "CONTOSO", "FABRIKAM" }, false)]
        [InlineData(new[] { "FABRIKAM" }, false)]
        [InlineData(new[] { "FABRIKAM", "CONTOSO" }, false)]
        public void HasAudience_ReturnsAppropriateResult(string[] audience, bool result)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Audience, audience.ToImmutableArray());

            // Act and assert
            Assert.Equal(result, principal.HasAudience("fabrikam"));
        }

        [Fact]
        public void HasPresenter_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.HasPresenter("Fabrikam"));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void HasPresenter_ThrowsAnExceptionForNullOrEmptyPresenter(string presenter)
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => principal.HasPresenter(presenter));

            Assert.Equal("presenter", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0187), exception.Message);
        }

        [Theory]
        [InlineData(new string[0], false)]
        [InlineData(new[] { "fabrikam" }, true)]
        public void HasPresenter_ReturnsExpectedResult(string[] presenter, bool result)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Presenter, presenter.ToImmutableArray());

            // Act and assert
            Assert.Equal(result, principal.HasPresenter());
        }

        [Theory]
        [InlineData(new string[0], false)]
        [InlineData(new[] { "contoso" }, false)]
        [InlineData(new[] { "contoso", "fabrikam" }, true)]
        [InlineData(new[] { "fabrikam" }, true)]
        [InlineData(new[] { "fabrikam", "contoso" }, true)]
        [InlineData(new[] { "CONTOSO" }, false)]
        [InlineData(new[] { "CONTOSO", "FABRIKAM" }, false)]
        [InlineData(new[] { "FABRIKAM" }, false)]
        [InlineData(new[] { "FABRIKAM", "CONTOSO" }, false)]
        public void HasPresenter_ReturnsAppropriateResult(string[] presenter, bool result)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Presenter, presenter.ToImmutableArray());

            // Act and assert
            Assert.Equal(result, principal.HasPresenter("fabrikam"));
        }

        [Fact]
        public void HasResource_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.HasResource("Fabrikam"));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void HasResource_ThrowsAnExceptionForNullOrEmptyResource(string resource)
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => principal.HasResource(resource));

            Assert.Equal("resource", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0062), exception.Message);
        }

        [Theory]
        [InlineData(new string[0], false)]
        [InlineData(new[] { "fabrikam" }, true)]
        public void HasResource_ReturnsExpectedResult(string[] resource, bool result)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Resource, resource.ToImmutableArray());

            // Act and assert
            Assert.Equal(result, principal.HasResource());
        }

        [Theory]
        [InlineData(new string[0], false)]
        [InlineData(new[] { "contoso" }, false)]
        [InlineData(new[] { "contoso", "fabrikam" }, true)]
        [InlineData(new[] { "fabrikam" }, true)]
        [InlineData(new[] { "fabrikam", "contoso" }, true)]
        [InlineData(new[] { "CONTOSO" }, false)]
        [InlineData(new[] { "CONTOSO", "FABRIKAM" }, false)]
        [InlineData(new[] { "FABRIKAM" }, false)]
        [InlineData(new[] { "FABRIKAM", "CONTOSO" }, false)]
        public void HasResource_ReturnsAppropriateResult(string[] resource, bool result)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Resource, resource.ToImmutableArray());

            // Act and assert
            Assert.Equal(result, principal.HasResource("fabrikam"));
        }

        [Fact]
        public void HasScope_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.HasScope(Scopes.OpenId));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ClaimsPrincipal_HasScope_ThrowsAnExceptionForNullOrEmptyScope(string scope)
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => principal.HasScope(scope));

            Assert.Equal("scope", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0180), exception.Message);
        }

        [Theory]
        [InlineData(new string[0], false)]
        [InlineData(new[] { "openid" }, true)]
        public void ClaimsPrincipal_HasScope_ReturnsExpectedResult(string[] scope, bool result)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Scope, scope.ToImmutableArray());

            // Act and assert
            Assert.Equal(result, principal.HasScope());
        }

        [Theory]
        [InlineData(new string[0], false)]
        [InlineData(new[] { "profile" }, false)]
        [InlineData(new[] { "profile", "openid" }, true)]
        [InlineData(new[] { "openid" }, true)]
        [InlineData(new[] { "openid", "profile" }, true)]
        [InlineData(new[] { "PROFILE" }, false)]
        [InlineData(new[] { "PROFILE", "OPENID" }, false)]
        [InlineData(new[] { "OPENID" }, false)]
        [InlineData(new[] { "OPENID", "PROFILE" }, false)]
        public void HasScope_ReturnsAppropriateResult(string[] scope, bool result)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaims(Claims.Private.Scope, scope.ToImmutableArray());

            // Act and assert
            Assert.Equal(result, principal.HasScope(Scopes.OpenId));
        }

        [Fact]
        public void HasTokenType_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.HasTokenType(TokenTypeHints.AccessToken));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void HasTokenType_ThrowsAnExceptionForNullOrEmptyTokenType(string type)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => principal.HasTokenType(type));

            Assert.Equal("type", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0188), exception.Message);
        }

        [Fact]
        public void HasTokenType_ReturnsExpectedResult()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetTokenType(TokenTypeHints.AccessToken);

            // Act and assert
            Assert.True(principal.HasTokenType(TokenTypeHints.AccessToken));
            Assert.False(principal.HasTokenType(TokenTypeHints.RefreshToken));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AddClaim_ThrowsAnExceptionForNullOrEmptyType(string type)
        {
            // Arrange
            var identity = new ClaimsIdentity();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => identity.AddClaim(type, "value"));

            Assert.Equal("type", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0184), exception.Message);
        }

        [Fact]
        public void AddClaim_AddsExpectedClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();

            // Act
            identity.AddClaim("type", "value");

            // Assert
            Assert.Equal("value", identity.GetClaim("type"));
        }

        [Fact]
        public void GetClaims_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.GetClaims("type"));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetClaims_ThrowsAnExceptionForNullOrEmptyClaimType(string type)
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => principal.GetClaims(type));

            Assert.Equal("type", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0184), exception.Message);
        }

        [Fact]
        public void GetClaims_ReturnsExpectedResult()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(Claims.Name, "Bob le Bricoleur"));
            identity.AddClaim(new Claim(Claims.Scope, Scopes.OpenId));
            identity.AddClaim(new Claim(Claims.Scope, Scopes.Profile));

            var principal = new ClaimsPrincipal(identity);

            // Act and assert
            Assert.Equal(new[] { Scopes.OpenId, Scopes.Profile }, principal.GetClaims(Claims.Scope));
        }

        [Fact]
        public void HasClaim_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.HasClaim("type"));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void HasClaim_ThrowsAnExceptionForNullOrEmptyClaimType(string type)
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => principal.HasClaim(type));

            Assert.Equal("type", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0184), exception.Message);
        }

        [Fact]
        public void HasClaim_ReturnsExpectedResult()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(Claims.Name, "Bob le Bricoleur"));
            identity.AddClaim(new Claim(Claims.Scope, Scopes.OpenId));
            identity.AddClaim(new Claim(Claims.Scope, Scopes.Profile));

            var principal = new ClaimsPrincipal(identity);

            // Act and assert
            Assert.True(principal.HasClaim(Claims.Name));
            Assert.True(principal.HasClaim(Claims.Scope));
            Assert.False(principal.HasClaim(Claims.Nickname));
        }

        [Fact]
        public void RemoveClaims_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.RemoveClaims("type"));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RemoveClaims_ThrowsAnExceptionForNullOrEmptyClaimType(string type)
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => principal.RemoveClaims(type));

            Assert.Equal("type", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0184), exception.Message);
        }

        [Fact]
        public void RemoveClaims_RemoveClaims()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            principal.SetClaim("type", "value");

            // Act
            principal.RemoveClaims("type");

            // Assert
            Assert.Null(principal.GetClaim("type"));
        }

        [Fact]
        public void SetClaim_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetClaim("type", "value"));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void SetClaim_ThrowsAnExceptionForNullOrEmptyProperty(string type)
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act and assert
            var exception = Assert.Throws<ArgumentException>(() => principal.SetClaim(type, "value"));

            Assert.Equal("type", exception.ParamName);
            Assert.StartsWith(SR.GetResourceString(SR.ID0184), exception.Message);
        }

        [Fact]
        public void SetClaim_AddsExpectedClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetClaim("type", "value");

            // Assert
            Assert.Equal("value", principal.GetClaim("type"));
        }

        [Fact]
        public void SetClaim_IsCaseInsensitive()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetClaim("TYPE", "value");

            // Assert
            Assert.Equal("value", principal.GetClaim("type"));
        }

        [Fact]
        public void SetClaim_RemovesEmptyClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetClaim("type", string.Empty);

            // Assert
            Assert.Null(principal.GetClaim("type"));
        }

        [Fact]
        public void SetCreationDate_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetCreationDate(date: null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Fact]
        public void SetCreationDate_AddsClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetCreationDate(new DateTimeOffset(2020, 01, 01, 05, 30, 30, TimeSpan.FromHours(1)));

            // Assert
            Assert.Equal("Wed, 01 Jan 2020 04:30:30 GMT", principal.GetClaim(Claims.Private.CreationDate));
        }

        [Fact]
        public void SetExpirationDate_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetExpirationDate(date: null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Fact]
        public void SetExpirationDate_AddsClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetExpirationDate(new DateTimeOffset(2020, 01, 01, 05, 30, 30, TimeSpan.FromHours(1)));

            // Assert
            Assert.Equal("Wed, 01 Jan 2020 04:30:30 GMT", principal.GetClaim(Claims.Private.ExpirationDate));
        }

        [Fact]
        public void SetAudiences_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetAudiences());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "fabrikam" }, new[] { "fabrikam" })]
        [InlineData(new[] { "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "FABRIKAM", "contoso" }, new[] { "fabrikam", "FABRIKAM", "contoso" })]
        public void SetAudiences_AddsAudiences(string[] audiences, string[] audience)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetAudiences(audiences);

            // Assert
            Assert.Equal(audience, principal.GetClaims(Claims.Private.Audience));
        }

        [Fact]
        public void SetPresenters_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetPresenters());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "fabrikam" }, new[] { "fabrikam" })]
        [InlineData(new[] { "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "FABRIKAM", "contoso" }, new[] { "fabrikam", "FABRIKAM", "contoso" })]
        public void SetPresenters_AddsPresenters(string[] presenters, string[] presenter)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetPresenters(presenters);

            // Assert
            Assert.Equal(presenter, principal.GetClaims(Claims.Private.Presenter));
        }

        [Fact]
        public void SetResources_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetResources());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "fabrikam" }, new[] { "fabrikam" })]
        [InlineData(new[] { "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "fabrikam", "contoso" }, new[] { "fabrikam", "contoso" })]
        [InlineData(new[] { "fabrikam", "FABRIKAM", "contoso" }, new[] { "fabrikam", "FABRIKAM", "contoso" })]
        public void SetResources_AddsResources(string[] resources, string[] resource)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetResources(resources);

            // Assert
            Assert.Equal(resource, principal.GetClaims(Claims.Private.Resource));
        }

        [Fact]
        public void SetScopes_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetScopes());

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "openid" }, new[] { "openid" })]
        [InlineData(new[] { "openid", "profile" }, new[] { "openid", "profile" })]
        [InlineData(new[] { "openid", "openid", "profile" }, new[] { "openid", "profile" })]
        [InlineData(new[] { "openid", "OPENID", "profile" }, new[] { "openid", "OPENID", "profile" })]
        public void SetScopes_AddsScopes(string[] scopes, string[] scope)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetScopes(scopes);

            // Assert
            Assert.Equal(scope, principal.GetClaims(Claims.Private.Scope));
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "openid" }, new[] { "openid" })]
        [InlineData(new[] { "openid", "profile" }, new[] { "openid", "profile" })]
        [InlineData(new[] { "openid", "openid", "profile" }, new[] { "openid", "profile" })]
        [InlineData(new[] { "openid", "OPENID", "profile" }, new[] { "openid", "OPENID", "profile" })]
        public void SetScopes_IEnumerable_AddsScopes(string[] scopes, string[] scope)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetScopes((IEnumerable<string>) scopes);

            // Assert
            Assert.Equal(scope, principal.GetClaims(Claims.Private.Scope));
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "openid" }, new[] { "openid" })]
        [InlineData(new[] { "openid", "profile" }, new[] { "openid", "profile" })]
        [InlineData(new[] { "openid", "openid", "profile" }, new[] { "openid", "profile" })]
        [InlineData(new[] { "openid", "OPENID", "profile" }, new[] { "openid", "OPENID", "profile" })]
        public void SetScopes_ImmutableArray_AddsScopes(string[] scopes, string[] scope)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetScopes(ImmutableArray.Create(scopes));

            // Assert
            Assert.Equal(scope, principal.GetClaims(Claims.Private.Scope));
        }

        [Fact]
        public void SetAccessTokenLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetAccessTokenLifetime(null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void SetAccessTokenLifetime_AddsLifetime(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetAccessTokenLifetime(ParseLifeTime(lifetime));

            // Assert
            Assert.Equal(lifetime, principal.GetClaim(Claims.Private.AccessTokenLifetime));
        }

        [Fact]
        public void SetAuthorizationCodeLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetAuthorizationCodeLifetime(null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void SetAuthorizationCodeLifetime_AddsLifetime(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetAuthorizationCodeLifetime(ParseLifeTime(lifetime));

            // Assert
            Assert.Equal(lifetime, principal.GetClaim(Claims.Private.AuthorizationCodeLifetime));
        }

        [Fact]
        public void SetDeviceCodeLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetDeviceCodeLifetime(null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void SetDeviceCodeLifetime_AddsLifetime(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetDeviceCodeLifetime(ParseLifeTime(lifetime));

            // Assert
            Assert.Equal(lifetime, principal.GetClaim(Claims.Private.DeviceCodeLifetime));
        }

        [Fact]
        public void SetIdentityTokenLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetIdentityTokenLifetime(null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void SetIdentityTokenLifetime_AddsLifetime(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetIdentityTokenLifetime(ParseLifeTime(lifetime));

            // Assert
            Assert.Equal(lifetime, principal.GetClaim(Claims.Private.IdentityTokenLifetime));
        }

        [Fact]
        public void SetRefreshTokenLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetRefreshTokenLifetime(null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void SetRefreshTokenLifetime_AddsLifetime(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetRefreshTokenLifetime(ParseLifeTime(lifetime));

            // Assert
            Assert.Equal(lifetime, principal.GetClaim(Claims.Private.RefreshTokenLifetime));
        }

        [Fact]
        public void SetUserCodeLifetime_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetUserCodeLifetime(null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("62")]
        public void SetUserCodeLifetime_AddsLifetime(string lifetime)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetUserCodeLifetime(ParseLifeTime(lifetime));

            // Assert
            Assert.Equal(lifetime, principal.GetClaim(Claims.Private.UserCodeLifetime));
        }

        [Fact]
        public void SetAuthorizationId_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetAuthorizationId(null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("identifier")]
        public void SetAuthorizationId_AddsScopes(string identifier)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetAuthorizationId(identifier);

            // Assert
            Assert.Equal(identifier, principal.GetClaim(Claims.Private.AuthorizationId));
        }

        [Fact]
        public void SetTokenId_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetTokenId(null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("identifier")]
        public void SetTokenId_AddsScopes(string identifier)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetTokenId(identifier);

            // Assert
            Assert.Equal(identifier, principal.GetClaim(Claims.Private.TokenId));
        }

        [Fact]
        public void SetTokenType_ThrowsAnExceptionForNullPrincipal()
        {
            // Arrange
            var principal = (ClaimsPrincipal) null;

            // Act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => principal.SetTokenType(null));

            Assert.Equal("principal", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("access_token")]
        public void SetTokenType_AddsType(string type)
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            principal.SetTokenType(type);

            // Assert
            Assert.Equal(type, principal.GetClaim(Claims.Private.TokenType));
        }

        private TimeSpan? ParseLifeTime(string lifetime)
        {
            var lifeT = lifetime is not null
                ? (TimeSpan?) TimeSpan.FromSeconds(double.Parse(lifetime, NumberStyles.Number, CultureInfo.InvariantCulture))
                : null;

            return lifeT;
        }
    }
}
