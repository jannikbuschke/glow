using Glue.Files;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddGlow(this IServiceCollection services)
        {
            services.AddSingleton<FileService>();
            services.AddSingleton<MockExternalSystems>();
            return services;
        }
    }
}
