using System;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;

namespace Glow.Authentication.Aad
{
    public class UserTokenCacheProvider
    {
        // the in memory cache
        private readonly IMemoryCache cache;
        private readonly ClaimsPrincipal principal;

        public UserTokenCacheProvider(IMemoryCache cache, ClaimsPrincipal principal)
        {
            this.cache = cache;
            this.principal = principal;
        }

        public void Initialize(ITokenCache tokenCache)
        {
            tokenCache.SetBeforeAccess(BeforeAccessNotification);
            tokenCache.SetAfterAccess(AfterAccessNotification);
            tokenCache.SetBeforeWrite(BeforeWriteNotification);
        }

        public void Clear()
        {
            cache.Remove(principal.GetMsalAccountId());
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (args.HasStateChanged)
            {
                Persist(principal.GetMsalAccountId(), args.TokenCache);
            }
        }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load(principal.GetMsalAccountId(), args.TokenCache);
        }

        private void BeforeWriteNotification(TokenCacheNotificationArgs args)
        {
        }

        private void Persist(string key, ITokenCacheSerializer tokenCache)
        {
            cache.Set(key, tokenCache.SerializeMsalV3(), DateTimeOffset.Now.AddHours(12));
        }

        private void Load(string key, ITokenCacheSerializer tokenCache)
        {
            var data = (byte[]) cache.Get(key);
            tokenCache.DeserializeMsalV3(data);
        }
    }
}
