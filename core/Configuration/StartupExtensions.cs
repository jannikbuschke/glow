using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Configurations
{
    public static class StartupExtensions
    {
        internal static Action<DbContextOptionsBuilder> optionsAction;
        public static IConfigurationBuilder AddEFConfiguration(
            this IConfigurationBuilder builder,
            Action<DbContextOptionsBuilder> optionsAction)
        {
            StartupExtensions.optionsAction = optionsAction;
            return builder.Add(new ConfigurationSource(optionsAction));
        }

        public static IServiceCollection AddEfConfiguration(
            this IServiceCollection services,
            Action<ConfigurationOptions> configure = null,
            IEnumerable<Assembly> assemblies = null
        )
        {
            var configuration = new ConfigurationOptions();
            configure?.Invoke(configuration);
            services.AddSingleton(configuration);
            services.AddSingleton<AuthorizationService>();
            services.AddHttpContextAccessor();

            services.AddSingleton((services) => new AssembliesCache(assemblies ?? new[] { Assembly.GetCallingAssembly() }));
            //services.AddMvcCore().AddApplicationPart(typeof(EntityFrameworkExtensions).Assembly);

            services.AddSingleton<Configurations>();

            services.AddMvcCore(options =>
            {
                options.Conventions.Add(new ConfigurationControllerRouteConvention());
            })
            .ConfigureApplicationPartManager(m =>
            {
                m.FeatureProviders.Add(new ControllerProvider(assemblies));
            });

            return services;
        }

    }
}
