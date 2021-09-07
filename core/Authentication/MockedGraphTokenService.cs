using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Glow.Authentication.Aad;
using Glow.Core.Authentication;
using Glow.TestAutomation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Common;

namespace Glow.Core.Authentication
{
    // glow/profile uses IGraphTokenService to get some scopes
    // in azdo apps we use this to mock it (workaround)
    public class MockedGraphTokenService : IGraphTokenService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private readonly IOptions<AadFakeAuthenticationOptions> fakeAuthOptions;
        private readonly IOptionsSnapshot<AzureAdOptions> aadOptions;

        public MockedGraphTokenService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IOptions<AadFakeAuthenticationOptions> fakeAuthOptions,
            IOptionsSnapshot<AzureAdOptions> aadOptions
        )
        {
            this.httpContextAccessor = httpContextAccessor;
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.fakeAuthOptions = fakeAuthOptions;
            this.aadOptions = aadOptions;
        }

        public Task<string> AccessTokenForApp()
        {
            return Task.FromResult("");
        }

        public async Task<string> AccessTokenForCurrentUser(string[] scope)
        {
            var authResult = await TokenForCurrentUser(scope);
            return authResult.AccessToken;
        }

        public Task<string> AccessTokenForServiceUser()
        {
            return Task.FromResult("");
        }

        public async Task<GraphServiceClient> GetClientForUser(string[] scopes, bool useBetaEndpoint = false)
        {
            var token = await AccessTokenForCurrentUser(scopes);
            var client = new GraphServiceClient(
                useBetaEndpoint ? "https://graph.microsoft.com/beta/" : "https://graph.microsoft.com/v1.0/",
                new DelegateAuthenticationProvider(
                    (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                        return Task.FromResult(requestMessage);
                    }
                ));
            return client;
        }

        public Task ThrowIfCurrentUserNotConsentedToScope(string scope)
        {
            return Task.CompletedTask;
        }

        public async Task<AuthenticationResult> TokenForCurrentUser(string[] scope)
        {
            StringValues userIds = httpContextAccessor.HttpContext.Request.Headers["x-userid"];
            var userId = userIds.FirstOrDefault() ?? fakeAuthOptions.Value.DefaultUserName;

            FakeUser? user = fakeAuthOptions.Value.Users?.FirstOrDefault(v => v.UserName == userId);
            if (user == null)
            {
                return null;
            }

            var aad = aadOptions.Value;
            var tenantId = aad.TenantId;//  configuration["OpenIdConnect:TenantId"];
            var clientId = aad.ClientId;// configuration["OpenIdConnect:ClientId"];
            var clientSecret = aad.ClientSecret;// configuration["ClientSecret"];

            HttpClient client = clientFactory.CreateClient();
            PasswordFlow.AccessTokenResponse result = await client.GetMsAccessTokentWithUsernamePassword(
                clientSecret,
                userId,
                user.Password,
                clientId,
                tenantId);

            return new AuthenticationResult(
                accessToken: result.access_token,
                isExtendedLifeTimeToken: true,
                uniqueId: Guid.NewGuid().ToString(),
                expiresOn: DateTimeOffset.Now,
                extendedExpiresOn: DateTimeOffset.Now,
                tenantId: tenantId,
                account: null,
                idToken: null,
                scopes: scope,
                correlationId: Guid.NewGuid(),
                authenticationResultMetadata: null);
        }
    }
}