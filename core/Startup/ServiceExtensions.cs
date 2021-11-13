using System;
using System.Reflection;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Extensions.ExpressionMapping;
using Glow.Clocks;
using Glow.Configurations;
using Glow.Core.Actions;
using Glow.Files;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Glow.Core
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds Mvc (with Netwonsoft), FileService, Clock, MockExternalSystems, SpaStaticFiles,
        /// SignalR, HttpClient, HttpContextAccessor,
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGlowApplicationServices(
            this IServiceCollection services,
            Action<MvcOptions> options = null,
            Action<IMvcBuilder> configureAdditionalMvcOptions = null,
            params Assembly[] assembliesToScan
        )
        {
            var mvcBuilder = services.AddControllers(o =>
                {
                    o.EnableEndpointRouting = true;
                    if (options != null) { options(o); }
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .ConfigureApiBehaviorOptions(v =>
                {
                    v.SuppressModelStateInvalidFilter = true;
                });

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

            configureAdditionalMvcOptions?.Invoke(mvcBuilder);

            services.AddGlow();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "web/build";
            });

            services.AddSignalR();
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddMediatR(assembliesToScan);
            services.AddAutoMapper(cfg =>
            {
                cfg.AddCollectionMappers();
                cfg.AddExpressionMapping();

            }, assembliesToScan);


            return services;
        }

        public static IServiceCollection AddGlowServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddGlow();
        }

        public static IServiceCollection AddGlow(this IServiceCollection services)
        {
            services.AddSingleton<FileService>();
            services.AddSingleton<MockExternalSystems>();
            services.AddSingleton<IClock, Clock>();

            return services;
        }
    }
}