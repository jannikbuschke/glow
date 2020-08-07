using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Glow.Authentication.Aad
{
    public static partial class AzureAdAuthenticationBuilderExtensions
    {
        public class ConfigureCookieOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
        {
            private readonly TicketStoreService ticketStore;

            public ConfigureCookieOptions(TicketStoreService service)
            {
                ticketStore = service;
            }

            public void Configure(string name, CookieAuthenticationOptions options)
            {
                options.SessionStore = ticketStore;
            }

            public void Configure(CookieAuthenticationOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
