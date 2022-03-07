using System;
using Glow.Core.StringExtensions;
using Microsoft.Extensions.Configuration;

namespace Glow.Core
{
    public static class IConfigurationExtensions
    {
        public static bool MockExternalSystems(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("MockExternalSystems") ||
                   configuration.GetValue<bool>("MockDependencies");
        }

        public static string ConnectionString(this IConfiguration configuration)
        {
            return configuration["ConnectionString"] ?? configuration["ConnectionStrings:DefaultConnection"] ??
                throw new System.Exception("Connectionstring not configured");
        }

        public static bool IsDemo(this IConfiguration configuration)
        {
            return configuration.GetValue<string>("Global:IsDemo") == "true";
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

        public static bool IsOldCodeEnabled(this IConfiguration cfg)
        {
            return cfg.GetValue<bool>("EnableOldCode");
        }
    }
}