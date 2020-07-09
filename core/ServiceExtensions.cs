using Glow.Clocks;
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
            services.AddSingleton<IClock, Clock>();

            return services;
        }
    }
}
