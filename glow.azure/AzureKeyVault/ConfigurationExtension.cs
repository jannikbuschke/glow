using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Serilog;

namespace Glow.Azure.AzureKeyVault;

public static class ConfigurationExtension
{
    public static WebApplicationBuilder AddKeyVaultAsConfigurationProviderIfNameConfigured(this WebApplicationBuilder builder)
    {
        string name = builder.Configuration["KeyVaultName"];

        if (!string.IsNullOrEmpty(name))
        {
            string vaultUri = name.StartsWith("https://") ? name : $"https://{name}.vault.azure.net/";

            builder.Configuration.AddAzureKeyVault(
                vaultUri,
                new DefaultKeyVaultSecretManager());
        }
        else
        {
            Log.Information("No KeyVaultName configured => skip keyvault configuration");
        }

        return builder;
    }

    public static IWebHostBuilder AddKeyVaultAsConfigurationProvider(this IWebHostBuilder builder)
    {
        return builder.ConfigureAppConfiguration((context, config) =>
        {
            IConfigurationRoot cfg = config.Build();
            string name = cfg.GetValue<string>("KeyVaultName");


            // if (context.HostingEnvironment.IsProduction() && !string.IsNullOrEmpty(name))
            if (!string.IsNullOrEmpty(name))
            {
                string keyvaultDns = name.StartsWith("https://") ? name : $"https://{name}.vault.azure.net/";
                Log.Information($"Using KeyVault {keyvaultDns}");
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                KeyVaultClient keyVaultClient =
                    new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                config.AddAzureKeyVault(keyvaultDns, keyVaultClient, new DefaultKeyVaultSecretManager());
            }
            else
            {
                Log.Information("No KeyVault configured");
            }
        });
    }
}