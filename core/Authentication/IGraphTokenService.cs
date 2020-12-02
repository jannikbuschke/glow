using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Glow.Core.Authentication
{
    public interface IGraphTokenService
    {
        Task<AuthenticationResult> TokenForCurrentUser(string[] scope);
        Task<string> AccessTokenForCurrentUser(string[] scope);
        Task<string> AccessTokenForApp();
    }
}
