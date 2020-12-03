using System;
using System.Threading.Tasks;
using Glow.Authentication.Aad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core.EfTicketStore
{
    public class EfTicketStore : ITicketStore
    {
        private readonly IServiceProvider serviceProvider;

        public EfTicketStore(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task RemoveAsync(string key)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            ITicketStoreDbContext context = scope.ServiceProvider.GetRequiredService<ITicketStoreDbContext>();
            if (Guid.TryParse(key, out Guid id))
            {
                DbAuthenticationTicket ticket = await context.AuthenticationTickets.SingleOrDefaultAsync(x => x.Id == id);
                if (ticket != null)
                {
                    context.AuthenticationTickets.Remove(ticket);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            ITicketStoreDbContext context = scope.ServiceProvider.GetRequiredService<ITicketStoreDbContext>();
            if (Guid.TryParse(key, out Guid id))
            {
                DbAuthenticationTicket authenticationTicket = await context.AuthenticationTickets.FindAsync(id);
                if (authenticationTicket != null)
                {
                    authenticationTicket.Value = SerializeToBytes(ticket);
                    authenticationTicket.LastActivity = DateTimeOffset.UtcNow;
                    authenticationTicket.Expires = ticket.Properties.ExpiresUtc;
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            ITicketStoreDbContext context = scope.ServiceProvider.GetRequiredService<ITicketStoreDbContext>();
            if (Guid.TryParse(key, out Guid id))
            {
                DbAuthenticationTicket authenticationTicket = await context.AuthenticationTickets.FindAsync(id);
                if (authenticationTicket != null)
                {
                    authenticationTicket.LastActivity = DateTimeOffset.UtcNow;
                    await context.SaveChangesAsync();

                    return DeserializeFromBytes(authenticationTicket.Value);
                }
            }

            return null;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            ITicketStoreDbContext context = scope.ServiceProvider.GetRequiredService<ITicketStoreDbContext>();
            //var userId = string.Empty;
            //var nameIdentifier = ticket.Principal.GetNameIdentifier();

            var authenticationTicket = new DbAuthenticationTicket()
            {
                UserId = ticket.Principal.GetMsalAccountId(),
                LastActivity = DateTimeOffset.UtcNow,
                Value = SerializeToBytes(ticket),
                Expires = ticket.Properties.ExpiresUtc.Value
            };

            context.AuthenticationTickets.Add(authenticationTicket);
            await context.SaveChangesAsync();

            return authenticationTicket.Id.ToString();
        }

        private byte[] SerializeToBytes(AuthenticationTicket source)
        {
            return TicketSerializer.Default.Serialize(source);
        }

        private AuthenticationTicket DeserializeFromBytes(byte[] source)
        {
            return source == null ? null : TicketSerializer.Default.Deserialize(source);
        }
    }
}
