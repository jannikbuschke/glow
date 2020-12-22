using Glow.Core.EfCore;
using Microsoft.AspNetCore.Hosting;

namespace Glow.Core.EfMsalTokenStore
{
    public static class IWebhostExtensions
    {
        public static void MigrateMsalTokenDbContext(this IWebHost self)
        {
            self.MigrateDatabase<SqlServerMsalTokenDbContext>();
        }
    }
}
