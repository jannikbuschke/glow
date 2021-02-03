using System.Threading.Tasks;
using Glow.Core.Actions;
using Glow.Glue.AspNetCore;
using Glow.TypeScript;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core.EfCore
{
    public static class HostExtensions
    {
        public static void MigrateDatabase<T>(this IWebHost host, string migration = "") where T : DbContext
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

        public static async Task ResetDatabase(this DbContext v, ResetDatabase request)
        {
            if (!request.IKnowWhatIAmDoing)
            {
                throw new BadRequestException("");
            }
            if (request.DeleteDatabase)
            {
                await v.Database.EnsureDeletedAsync();
            }
            await v.Database.EnsureCreatedAsync();
        }
    }

    [Action(Policy = "Admin", Route = "api/glow/db/reset-database")]
    [GenerateTsInterface]
    public class ResetDatabase: IRequest
    {
        public bool DeleteDatabase { get; set; }
        public bool IKnowWhatIAmDoing { get; set; }
    }

    public enum DatabaseProvider
    {
        SqlServer = 1
    }
}
