using Microsoft.Extensions.Configuration;

namespace Glow.Configurations
{
    public static class IConfigurationExtensions
    {
        public static string GetAppBaseUrl(this IConfiguration configuration)
        {
            return configuration.GetValue<string>("OpenIdConnect:BaseUrl");
        }
    }
}