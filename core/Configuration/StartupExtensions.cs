using System;
using System.Collections.Generic;
using System.Reflection;
using Glow.Core.EfCore;
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
            IEnumerable<Assembly> assemblies = null,
            DatabaseProvider dbProvider = DatabaseProvider.SqlServer
        )
        {
            var configuration = new ConfigurationOptions();
            configure?.Invoke(configuration);
            services.AddSingleton(configuration);
            services.AddScoped<ConfigurationAuthorizationService>();
            services.AddHttpContextAccessor();

            IEnumerable<Assembly> a = assemblies ?? new[] { Assembly.GetCallingAssembly() };

            services.AddSingleton((services) => new AssembliesCache(a));
            //services.AddMvcCore().AddApplicationPart(typeof(EntityFrameworkExtensions).Assembly);

            services.AddSingleton<Configurations>();

            services.AddMvcCore(options =>
            {
                options.Conventions.Add(new ConfigurationControllerRouteConvention());
            })
            .ConfigureApplicationPartManager(m =>
            {
                m.FeatureProviders.Add(new ControllerProvider(a));
            });

            switch (dbProvider)
            {
                case DatabaseProvider.SqlServer:
                    services.AddDbContext<SqlServerConfigurationDataContext>(StartupExtensions.optionsAction);
                    services.AddScoped<IConfigurationDataContext, SqlServerConfigurationDataContext>();
                    break;
                default:
                    throw new Exception("Unsupported Database Provider");
            }

            return services;
        }

    }
}
