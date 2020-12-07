using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Glow.Authentication.Aad
{
    public static partial class AzureAdAuthenticationBuilderExtensions
    {
        public class ConfigureCookieOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
        {
            private readonly ITicketStore ticketStore;

            public ConfigureCookieOptions(ITicketStore ticketStore)
            {
                this.ticketStore = ticketStore;
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
