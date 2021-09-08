using System;
using System.Reflection;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Extensions.ExpressionMapping;
using Glow.Authentication.Aad;
using Glow.Clocks;
using Glow.Configurations;
using Glow.Core.Actions;
using Glow.Core.Authentication;
using Glow.Core.EfMsalTokenStore;
using Glow.Core.EfTicketStore;
using Glow.Files;
using Glow.MsGraph.Mails;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddScoped<MailService>();

            return services;
        }

        public static IServiceCollection AddGlowAadIntegration(
            this IServiceCollection services,
            IWebHostEnvironment env,
            IConfiguration configuration
        )
        {
            services.AddScoped<IGraphTokenService, GraphTokenService>();

            services.AddSingleton<TokenService>();
            if (env.IsDevelopment())
            {
                var connectionString = configuration.ConnectionString();
                services.AddEfMsalTokenCache(options =>
                {
                    options.UseSqlServer(connectionString);
                });

                services.AddEfTicketStore(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            }
            else
            {
                services.AddSingleton<ITokenCacheProvider, TokenCacheProvider>();
                services.AddSingleton<ITicketStore, InmemoryTicketStore>();
            }

            services.AddMemoryCache();

            return services;
        }

        /// <summary>
        /// Adds FileService, MockExternalSystems and Clock
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGlow(this IServiceCollection services)
        {
            services.AddSingleton<FileService>();
            services.AddSingleton<MockExternalSystems>();
            services.AddSingleton<IClock, Clock>();

            return services;
        }
    }
}