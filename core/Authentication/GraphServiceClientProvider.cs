using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace Glow.Core.Authentication
{
    public static class GraphServiceClientProvider
    {
        public static GraphServiceClient CreateGetClient(string token, bool useBeta = false)
        {
            var client = new GraphServiceClient(
                useBeta ? "https://graph.microsoft.com/beta" : "https://graph.microsoft.com/v1.0/",
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