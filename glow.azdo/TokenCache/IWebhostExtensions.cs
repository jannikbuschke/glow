using Glow.Core.EfCore;
using Microsoft.AspNetCore.Hosting;

namespace Glow.TokenCache
{
    public static class IWebhostExtensions
    {
        public static void MigrateAzdoTokenCache(this IWebHost webhost)
        {
            webhost.MigrateDatabase<SqlServerTokenDataContext>();
        }
    }
}