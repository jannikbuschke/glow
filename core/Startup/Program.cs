using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Glow.GlowStartup
{
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