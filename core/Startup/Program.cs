using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.GlowStartup
{
    public static class HostExtensions
    {
        public static void MigrateDatabase<DataContext>(this IWebHost host, string migration = "") where DataContext : DbContext
        {
            using IServiceScope scope = host.Services.CreateScope();
            DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
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
    public class Program
    {
        public static IWebHostBuilder CreateDefaultWebHostBuilder<Startup>(string[] args) where Startup : class
        {

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureKestrel((context, options) =>
                {
                    // Handle requests up to 50 MB
                    options.Limits.MaxRequestBodySize = 52428800;
                })
                .UseStartup<Startup>();
            //.ConfigureAppConfiguration((context, config) =>
            //{
            //    IConfigurationRoot cfg = config.Build();
            //    var keyVault = cfg.GetValue<string>("KeyVaultUri");

            //    if (context.HostingEnvironment.IsProduction() && !string.IsNullOrEmpty(keyVault))
            //    {
            //        Log.Information("Using KeyVault {KeyVault}", keyVault);
            //        var azureServiceTokenProvider = new AzureServiceTokenProvider();
            //        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            //        config.AddAzureKeyVault(keyVault, keyVaultClient, new DefaultKeyVaultSecretManager());
            //    }
            //    else
            //    {
            //        Log.Information("No KeyVault configured");
            //    }
            //});
        }

    }
}