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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Glow.Core.Authentication
{
    extern alias GraphBeta;

    // glow/profile uses IGraphTokenService to get some scopes
    // in azdo apps we use this to mock it (workaround)
    public class MockedGraphTokenService : IGraphTokenService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private readonly IOptions<AadFakeAuthenticationOptions> fakeAuthOptions;
        private readonly IOptionsSnapshot<AzureAdOptions> aadOptions;
        private readonly ILogger<MockedGraphTokenService> logger;
        private static System.Collections.Generic.Dictionary<string, AuthenticationResult> Cache = new();

        public MockedGraphTokenService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IOptions<AadFakeAuthenticationOptions> fakeAuthOptions,
            IOptionsSnapshot<AzureAdOptions> aadOptions,
            ILogger<MockedGraphTokenService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.fakeAuthOptions = fakeAuthOptions;
            this.aadOptions = aadOptions;
            this.logger = logger;
        }

        public override Task<string> AccessTokenForApp()
        {
            return Task.FromResult("");
        }

        public override async Task<string> AccessTokenForCurrentUser(string[] scope)
        {
            AuthenticationResult authResult = await TokenForCurrentUser(scope);
            return authResult.AccessToken;
        }

        public override Task<string> AccessTokenForServiceUser()
        {
            return Task.FromResult("");
        }

        public override async Task<GraphServiceClient> GetClientForUser(string[] scopes, bool useBetaEndpoint = false)
        {
            var token = await AccessTokenForCurrentUser(scopes);
            return CreateClient(token, useBetaEndpoint);
        }

        public override async Task<GraphBeta::Microsoft.Graph.GraphServiceClient> GetBetaClientForUser(string[] scopes)
        {
            var token = await AccessTokenForCurrentUser(scopes);
            return CreateBetaClient(token);
        }

        public override Task ThrowIfCurrentUserNotConsentedToScope(string scope)
        {
            return Task.CompletedTask;
        }

        public override async Task<AuthenticationResult> TokenForCurrentUser(string[] scope)
        {
            StringValues? ids = httpContextAccessor?.HttpContext?.Request?.Headers["x-userid"];
            var id = ids?.FirstOrDefault();

            StringValues? usernames = httpContextAccessor?.HttpContext?.Request?.Headers["x-username"];
            var username = usernames?.FirstOrDefault() ?? fakeAuthOptions.Value.DefaultUserName;

            FakeUser? user = id != null
                ? fakeAuthOptions.Value.Users?.FirstOrDefault(v => v.UserName == id)
                : username == null
                    ? null //fakeAuthOptions.Value.Users?.FirstOrDefault(v => v.UserName == fakeAuthOptions.Value.DefaultUserName)
                    : fakeAuthOptions.Value.Users?.FirstOrDefault(v => v.UserName == username);

            if (user == null)
            {
                logger.LogInformation("No user found");
                return null;
            }

            var isExpired = Cache.ContainsKey(user.UserName) && Cache[user.UserName].ExpiresOn < DateTimeOffset.UtcNow.AddMinutes(60);
            if (!Cache.ContainsKey(user.UserName) || isExpired)
            {
                if (isExpired)
                {
                    logger.LogInformation($"Token expired, refresh");
                }
                try
                {
                    logger.LogInformation($"User {user.UserName} not yet cached");
                    AzureAdOptions aad = aadOptions.Value;
                    var tenantId = aad.TenantId;
                    var clientId = aad.ClientId;
                    var clientSecret = aad.ClientSecret;

                    HttpClient client = clientFactory.CreateClient();
                    PasswordFlow.AccessTokenResponse result = await client.GetMsAccessTokentWithUsernamePassword(
                        clientSecret,
                        user.UserName,
                        user.Password,
                        clientId,
                        tenantId);

                    Cache[user.UserName] = new AuthenticationResult(
                        accessToken: result.access_token,
                        isExtendedLifeTimeToken: false,
                        uniqueId: Guid.NewGuid().ToString(),
                        expiresOn: DateTimeOffset.UtcNow.AddSeconds(result.expires_in),
                        extendedExpiresOn: DateTimeOffset.UtcNow.AddSeconds(result.ext_expires_in),
                        tenantId: tenantId,
                        account: null,
                        idToken: null,
                        scopes: scope,
                        correlationId: Guid.NewGuid());
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Could not authenticate " + user.UserName);
                    return null;
                }
            }

            return Cache[user.UserName];
        }
    }
}