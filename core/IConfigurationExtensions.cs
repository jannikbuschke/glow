using Microsoft.Extensions.Configuration;

namespace Glow.Core
{
    public static class IConfigurationExtensions
    {
        public static bool MockExternalSystems(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("MockExternalSystems") || configuration.GetValue<bool>("MockDependencies");
        }

        public static string ConnectionString(this IConfiguration configuration)
        {
            return configuration["ConnectionString"] ?? configuration["ConnectionStrings:DefaultConnection"] ?? throw new System.Exception("Connectionstring not configured");
        }
    }
}
