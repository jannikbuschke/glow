using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        public async Task<AuthenticationResult> GetAccessTokenByIdToken(ClaimsPrincipal principal, string jwtToken)
        {
            //IConfidentialClientApplication app = BuildApp(principal);
            //AuthenticationResult result = await app.AcquireTokenOnBehalfOf(new[] { "user.read" }, new UserAssertion(idToken)).ExecuteAsync().ConfigureAwait(false);
            //IAccount account = await app.GetAccountAsync(principal.GetMsalAccountId());
            //return result;

            if (jwtToken == null)
            {
                throw new ArgumentNullException(jwtToken, "tokenValidationContext.SecurityToken should be a JWT Token");
            }

            var userAssertion = new UserAssertion(jwtToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");

            IConfidentialClientApplication confidentialClientApp = BuildApp(principal);

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

        public void AddAccountToCacheFromJwt(
            IEnumerable<string> scopes,
            string jwtToken,
            ClaimsPrincipal principal,
            HttpContext httpContext
        )
        {
            try
            {
                //UserAssertion userAssertion;
                //IEnumerable<string> requestedScopes;
                //if (jwtToken != null)
                //{
                //    userAssertion = new UserAssertion(jwtToken.RawData, "urn:ietf:params:oauth:grant-type:jwt-bearer");
                //    requestedScopes = scopes ?? jwtToken.Audiences.Select(a => $"{a}/.default");
                //}
                //else
                //{
                //    throw new ArgumentOutOfRangeException("tokenValidationContext.SecurityToken should be a JWT Token");
                //}

                // Create the application
                IConfidentialClientApplication application = BuildApp(principal);

                // .Result to make sure that the cache is filled-in before the controller tries to get access tokens
                AuthenticationResult result = application.AcquireTokenOnBehalfOf(
                    new string[] { "user.read" },
                    new UserAssertion(jwtToken))
                    .ExecuteAsync()
                    .GetAwaiter().GetResult();
            }
            catch (MsalException)
            {
                throw;
            }
        }

        private IConfidentialClientApplication BuildApp(ClaimsPrincipal principal)
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(azureAdOptions.ClientId)
                .WithClientSecret(azureAdOptions.ClientSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, azureAdOptions.TenantId)
                .WithRedirectUri(azureAdOptions.BaseUrl + azureAdOptions.CallbackPath)
                .Build();

            userTokenCacheProviderFactory.Create(principal).Initialize(app.UserTokenCache);

            return app;
        }
    }
}
