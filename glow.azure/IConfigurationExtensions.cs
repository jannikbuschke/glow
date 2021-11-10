using Glow.Authentication.Aad;
using Glow.Core.StringExtensions;
using Microsoft.Extensions.Configuration;

namespace Glow.Azure
{
    public static class IConfigurationExtensions
    {
        public static string GetKeyvaultDns(this IConfiguration cfg)
        {
            var name = cfg.GetValue<string>("KeyVaultName");
            if (string.IsNullOrEmpty(name)) { return null; }

            var keyvaultDns = name.StartsWith("https://") ? name : $"https://{name}.vault.azure.net/";
            return keyvaultDns;
        }

        public static bool AllowConfiguration(this IConfiguration cfg)
        {
            return cfg.GetValue<bool>("AllowConfiguration") == true;
        }

        public static bool OpenIdConnectIsConfigured(this IConfiguration cfg)
        {
            var options = new AzureAdOptions();
            cfg.Bind("OpenIdConnect", options);
            if (string.IsNullOrEmpty(options.ClientSecret))
            {
                options.ClientSecret = cfg["ClientSecret"];
            }

            return !options.ClientSecret.IsNullOrEmpty();
        }

        public static void BindOpenIdConnect(this IConfiguration cfg, AzureAdOptions options)
        {
            cfg.Bind("OpenIdConnect", options);
            if (string.IsNullOrEmpty(options.ClientSecret))
            {
                options.ClientSecret = cfg["ClientSecret"];
            }
        }
    }
}