using Glow.Core.EfCore;
using Microsoft.AspNetCore.Hosting;

namespace Glow.Core.EfTicketStore
{
    public static class IWebhostExtensions
    {
        public static void MigrateTicketStoreDbContext(this IWebHost self)
        {
            self.MigrateDatabase<SqlServerTicketStoreDbContext>();

        }
    }
}
