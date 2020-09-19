using Microsoft.Extensions.Configuration;

namespace Glow.Core
{
    public static class IConfigurationExtensions
    {
        public static bool MockExternalSystems(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("MockExternalSystems")|| configuration.GetValue<bool>("MockDependencies");
        }
    }
}
