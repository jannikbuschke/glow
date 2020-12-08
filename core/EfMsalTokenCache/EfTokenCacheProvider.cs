using System;
using System.Collections.Generic;
using System.Security.Claims;
using Glow.Core.EfMsalTokenStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace Glow.Authentication.Aad
{
    public class EfTokenCacheProvider : ITokenCacheProvider
    {
        private static readonly Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();
        private readonly IServiceProvider serviceProvider;
        private ClaimsPrincipal principal;

        public EfTokenCacheProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Initialize(ClaimsPrincipal principal, ITokenCache tokenCache)
        {
            this.principal = principal;
            tokenCache.SetBeforeAccess(BeforeAccessNotification);
            tokenCache.SetAfterAccess(AfterAccessNotification);
            tokenCache.SetBeforeWrite(BeforeWriteNotification);
        }

        public void Clear(ClaimsPrincipal principal)
        {
            data.Remove(principal.GetMsalAccountId());
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
            using IServiceScope scope = serviceProvider.CreateScope();
            IMsalTokenDbContext ctx = scope.ServiceProvider.GetRequiredService<IMsalTokenDbContext>();

            var value = tokenCache.SerializeMsalV3();
            MsalToken token = ctx.MsalTokens.Find(key);
            if (token != null)
            {
                token.Value = value;
            }
            else
            {
                ctx.MsalTokens.Add(new MsalToken { Id = key, Value = value });
            }
            ctx.SaveChanges();
        }

        private void Load(string key, ITokenCacheSerializer tokenCache)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IMsalTokenDbContext ctx = scope.ServiceProvider.GetRequiredService<IMsalTokenDbContext>();

            MsalToken token = ctx.MsalTokens.Find(key);
            if (token != null)
            {
                tokenCache.DeserializeMsalV3(token.Value);
            }
        }
    }
}
