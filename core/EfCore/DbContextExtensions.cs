using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core.EfCore
{
    public static class HostExtensions
    {
        public static void MigrateDatabase<T>(this IWebHost host, string migration = "") where T: DbContext
        {
            using IServiceScope scope = host.Services.CreateScope();
            T db = scope.ServiceProvider.GetRequiredService<T>();
            if (string.IsNullOrEmpty(migration))
            {
                db.Database.Migrate();
            }
            else
            {
                IMigrator migrator = db.GetInfrastructure().GetService<IMigrator>();
                migrator.Migrate(migration);
            }
        }
    }

    public static class DbContextExtensions
    {

        public static void Migrate(this DbContext v, string migration = "")
        {
            if (string.IsNullOrEmpty(migration))
            {

                v.Database.Migrate();
            }
            else
            {
                IMigrator migrator = v.GetInfrastructure().GetService<IMigrator>();
                migrator.Migrate(migration);
            }
        }

        public static Task MigrateAsync(this DbContext v, string migration = "")
        {
            if (string.IsNullOrEmpty(migration))
            {
                return v.Database.MigrateAsync();
            }
            else
            {
                IMigrator migrator = v.GetInfrastructure().GetService<IMigrator>();
                return migrator.MigrateAsync(migration);
            }
        }
    }

    public enum DatabaseProvider
    {
        SqlServer = 1
    }
}
