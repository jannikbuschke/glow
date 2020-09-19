using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Glow.Authentication.Aad
{
    public class TokenService
    {
        private readonly AzureAdOptions azureAdOptions;
        private static readonly string[] scopes = { "openid" };
        private readonly UserTokenCacheProviderFactory userTokenCacheProviderFactory;

        public TokenService(
            IOptions<AzureAdOptions> options,
            UserTokenCacheProviderFactory userTokenCacheProviderFactory
        )
        {
            azureAdOptions = options.Value;
            this.userTokenCacheProviderFactory = userTokenCacheProviderFactory;
        }

        public async Task<AuthenticationResult> AcquireOnBehalfOf(string accessToken)
        {
            var userAssertion = new UserAssertion(accessToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");

            IConfidentialClientApplication confidentialClientApp = BuildApp();

            AuthenticationResult result = await confidentialClientApp.AcquireTokenOnBehalfOf(
                    new string[] { "user.read" },
                    userAssertion)
                .ExecuteAsync();
            return result;
        }

        public async Task<AuthenticationResult> GetAccessTokenByAuthorizationCodeAsync(ClaimsPrincipal principal, string code)
        {
            IConfidentialClientApplication app = BuildApp(principal);
            AuthenticationResult result = await app.AcquireTokenByAuthorizationCode(scopes, code).ExecuteAsync().ConfigureAwait(false);
            IAccount account = await app.GetAccountAsync(principal.GetMsalAccountId());
            return result;
        }

        public Task<AuthenticationResult> GetAccessTokenAsync(ClaimsPrincipal principal)
        {
            return GetAccessTokenAsync(principal, scopes);
        }

        public async Task<AuthenticationResult> GetAccessTokenAsync(ClaimsPrincipal principal, string[] scopes)
        {
            IConfidentialClientApplication app = BuildApp(principal);
            IAccount account = await app.GetAccountAsync(principal.GetMsalAccountId());

            // guest??
            if (null == account)
            {
                System.Collections.Generic.IEnumerable<IAccount> accounts = await app.GetAccountsAsync();
                account = accounts.FirstOrDefault(a => a.Username == principal.GetLoginHint());
            }

            AuthenticationResult token = await app.AcquireTokenSilent(scopes, account).ExecuteAsync().ConfigureAwait(false);
            return token;
        }

        public void RemoveAccount(ClaimsPrincipal principal)
        {
            userTokenCacheProviderFactory.Create(principal).Clear();
        }

        public async Task AddToCache(ClaimsPrincipal principal)
        {
            IConfidentialClientApplication app = BuildApp(principal);

            //AuthenticationResult result = await app.AcquireTokenByAuthorizationCode(scopes, code).ExecuteAsync().ConfigureAwait(false);
            //IAccount account = await app.GetAccountAsync(principal.GetMsalAccountId());
        }

        private IConfidentialClientApplication BuildApp(ClaimsPrincipal principal = null)
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(azureAdOptions.ClientId)
                .WithClientSecret(azureAdOptions.ClientSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, azureAdOptions.TenantId)
                .WithRedirectUri(azureAdOptions.BaseUrl + azureAdOptions.CallbackPath)
                .Build();

            if (principal != null)
            {
                userTokenCacheProviderFactory.Create(principal).Initialize(app.UserTokenCache);
            }

            return app;
        }
    }
}
