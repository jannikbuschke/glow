using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace Glow.Authentication.Aad
{
    public class UserTokenCacheProviderFactory
    {
        private readonly IMemoryCache memoryCache;

        public UserTokenCacheProviderFactory(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public UserTokenCacheProvider Create(ClaimsPrincipal principal)
        {
            return new UserTokenCacheProvider(memoryCache, principal);
        }
    }
}
