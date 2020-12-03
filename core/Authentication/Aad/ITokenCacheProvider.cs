using System.Security.Claims;
using Microsoft.Identity.Client;

namespace Glow.Authentication.Aad
{
    public interface ITokenCacheProvider
    {
        void Initialize(ClaimsPrincipal principal, ITokenCache tokenCache);
        void Clear(ClaimsPrincipal principal);
    }
}
