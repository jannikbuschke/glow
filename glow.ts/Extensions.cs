using System.Reflection;
using Glow.Configurations;
using Glow.Core.Actions;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Ts;

public static class ServiceExtensions
{
    public static IServiceCollection AddGlowActions(
        this IServiceCollection services,
        params Assembly[] assembliesToScan
    )
    {
        services.AddMvcCore(options =>
            {
                options.Conventions.Add(new ConfigurationControllerRouteConvention());
                options.Conventions.Add(new ActionsControllerRouteConvention());
            })
            .ConfigureApplicationPartManager(m =>
            {
                m.FeatureProviders.Add(new ConfigurationsControllerProvider(assembliesToScan));
                m.FeatureProviders.Add(new ActionsControllerProvider(assembliesToScan));
            })
            .AddApplicationPart(typeof(ActionsControllerProvider).Assembly);
        return services;
    }
}