using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace Glow.Authentication.Aad
{
    public class InmemoryTicketStore : ITicketStore
    {
        private readonly IMemoryCache cache;
        private const string KeyPrefix = "authticket";

        public InmemoryTicketStore(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        public Task RemoveAsync(string key)
        {
            cache.Remove(key);
            return Task.FromResult(true);
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new MemoryCacheEntryOptions();
            DateTimeOffset? expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }

            options.SetSlidingExpiration(TimeSpan.FromHours(1));

            cache.Set(key, ticket, options);

            return Task.FromResult(true);
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            cache.TryGetValue(key, out AuthenticationTicket ticket);
            return Task.FromResult(ticket);
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var id = Guid.NewGuid();
            var key = KeyPrefix + id;
            await ((ITicketStore) this).RenewAsync(key, ticket);
            return key;
        }
    }
}
