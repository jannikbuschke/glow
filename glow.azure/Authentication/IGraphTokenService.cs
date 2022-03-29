extern alias GraphBeta;
using Beta = GraphBeta.Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Glow.Core.Authentication
{
    public abstract class IGraphTokenService
    {
        public abstract Task<AuthenticationResult> TokenForCurrentUser(string[] scope);
        public abstract Task<string> AccessTokenForCurrentUser(string[] scope);
        public abstract Task<string> AccessTokenForApp();
        public abstract Task<string> AccessTokenForServiceUser();
        public abstract Task<GraphServiceClient> GetClientForUser(string[] scopes, bool useBetaEndpoint = false);
        public abstract Task<Beta.GraphServiceClient> GetBetaClientForUser(string[] scopes);
        public abstract Task ThrowIfCurrentUserNotConsentedToScope(string scope);

        public GraphServiceClient ClientForAccessToken(string accessToken, bool useBetaEndpoint = false)
        {
            return CreateClient(accessToken, useBetaEndpoint);
        }

        public GraphServiceClient CreateClient(string token, bool useBetaEndpoint = false)
        {
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

        public Beta.GraphServiceClient CreateBetaClient(string token)
        {
            var client = new Beta.GraphServiceClient("https://graph.microsoft.com/beta/",
                new DelegateAuthenticationProvider(
                    (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                        return Task.FromResult(requestMessage);
                    }
                ));
            return client;
        }
    }
}