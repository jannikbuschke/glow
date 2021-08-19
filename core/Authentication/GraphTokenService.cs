using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Glow.Authentication.Aad;
using Glow.Glue.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Glow.Core.Authentication
{
    public class GraphTokenService : IGraphTokenService
    {
        private readonly IHttpContextAccessor accessor;
        private readonly TokenService tokenService;
        private readonly IOptions<AzureAdOptions> options;
        private readonly ILogger<GraphTokenService> logger;
        private readonly IOptionsSnapshot<ServiceUserConfiguration> serviceUserConfiguration;
        private readonly IHttpClientFactory clientFactory;

        public GraphTokenService(
            IHttpContextAccessor accessor,
            TokenService tokenService,
            IOptions<AzureAdOptions> options,
            ILogger<GraphTokenService> logger,
            IOptionsSnapshot<ServiceUserConfiguration> serviceUserConfiguration,
            IHttpClientFactory clientFactory
        )
        {
            this.accessor = accessor;
            this.tokenService = tokenService;
            this.options = options;
            this.logger = logger;
            this.serviceUserConfiguration = serviceUserConfiguration;
            this.clientFactory = clientFactory;
        }

        public async Task<string> AccessTokenForApp()
        {
            AzureAdOptions o = options.Value;

            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId: o.ClientId)
                .WithClientSecret(o.ClientSecret)
                .WithTenantId(o.TenantId)
                .Build();

            var scopes = new string[] {"https://graph.microsoft.com/.default"};

            var token = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return token.AccessToken;
        }

        public async Task<string> AccessTokenForServiceUser()
        {
            var httpClient = clientFactory.CreateClient();
            var token = await httpClient.GetMsAccessTokentWithUsernamePassword(
                options.Value.ClientSecret,
                serviceUserConfiguration.Value.Username,
                serviceUserConfiguration.Value.Password,
                options.Value.ClientId,
                options.Value.TenantId);

            return token.access_token;
        }

        public async Task<AuthenticationResult> TokenForCurrentUser(string[] scopes)
        {
            if (accessor.HttpContext.Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value))
            {
                var token = value.FirstOrDefault();

                AzureAdOptions o = options.Value;

                IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId: o.ClientId)
                    .WithClientSecret(o.ClientSecret)
                    .WithTenantId(o.TenantId)
                    .Build();

                AuthenticationResult t = await app.AcquireTokenOnBehalfOf(scopes, new UserAssertion(token.Split(" ")[1])).ExecuteAsync();

                return t;
            }
            else
            {
                ClaimsPrincipal user = accessor.HttpContext.User;
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    throw new ForbiddenException("Not authenticated");
                }
                try
                {
                    AuthenticationResult result = await tokenService.GetAccessTokenAsync(user, scopes);
                    return result;
                }
                catch (MsalUiRequiredException e)
                {
                    if (e.Classification == UiRequiredExceptionClassification.ConsentRequired)
                    {
                        var message = $"Consent missing: The app does not yet have your consent to the scope '{scopes[0]}'." +
                                      $" Please first allow the app to act on your behalf, then try again."+
                                      $" Scopes can be granted under the account settings.";
                        logger.LogError("Missing consent {@scopes}", scopes);
                        throw new MissingConsentException(message, scopes[0]);
                    }
                    throw;
                }
            }
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

        public async Task ThrowIfCurrentUserNotConsentedToScope(string scope)
        {
            var _ = await TokenForCurrentUser(new string[] {scope});
        }

        public async Task<string> AccessTokenForCurrentUser(string[] scopes)
        {
            AuthenticationResult result = await TokenForCurrentUser(scopes);
            return result.AccessToken;
        }
    }
}
