using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Glow.Core.Authentication
{
    public interface IGraphTokenService
    {
        Task<AuthenticationResult> TokenForCurrentUser(string[] scope);
        Task<string> AccessTokenForCurrentUser(string[] scope);
        Task<string> AccessTokenForApp();
        Task<string> AccessTokenForServiceUser();
        Task<GraphServiceClient> GetClientForUser(string[] scopes, bool useBetaEndpoint = false);
        Task ThrowIfCurrentUserNotConsentedToScope(string scope);
    }
}