using System;
using System.Reflection;
using System.Text.Json.Serialization;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Extensions.ExpressionMapping;
using Glow.Clocks;
using Glow.Files;
using Glow.Ts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using IClock = Glow.Clocks.IClock;
using NodaTime.Serialization.SystemTextJson;

namespace Glow.Core
{
    public static class DefaultFsharpJsonSerializationOptions
    {
        public const JsonUnionEncoding UnionEncoding = JsonUnionEncoding.AdjacentTag
                                                       | JsonUnionEncoding.UnwrapRecordCases
                                                       | JsonUnionEncoding.UnwrapOption
                                                       | JsonUnionEncoding.UnwrapSingleCaseUnions | JsonUnionEncoding.AllowUnorderedTag;
    }

    public enum JsonSerializationStrategy
    {
        NewtonSoft = 0,
        SystemTextJson = 1
    }

    public static class ServiceExtensions
    {
        public static IMvcBuilder AddGlowNewtonsoftControllers(
            this IServiceCollection services,
            Action<MvcOptions> options = null
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
                    options.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .ConfigureApiBehaviorOptions(v =>
                {
                    v.SuppressModelStateInvalidFilter = true;
                });
            return mvcBuilder;
        }

        public static IMvcBuilder AddGlowSystemTextJsonControllers(
            this IServiceCollection services,
            Action<MvcOptions> options = null
        )
        {
            // let options = JsonSerializerOptions()
            // options.Converters.Add(JsonFSharpConverter())
            //
            // JsonSerializer.Serialize({| x = "Hello"; y = "world!" |}, options)

            var mvcBuilder = services.AddControllers(o =>
                {
                    o.EnableEndpointRouting = true;
                    if (options != null) { options(o); }
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    options.JsonSerializerOptions.WriteIndented = true;
                    // options.JsonSerializerOptions.Form = Formatting.Indented;
                    // options.JsonSerializerOptions.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    // options.JsonSerializerOptions.Converters.Add(new StringEnumConverter());
                    // options.JsonSerializerOptions.Date = DateTimeZoneHandling.Utc;

                    // unionEncoding = (
                    //     // Base encoding:
                    //     JsonUnionEncoding.InternalTag
                    //     // Additional options:
                    //     ||| JsonUnionEncoding.UnwrapOption
                    //     ||| JsonUnionEncoding.UnwrapRecordCases
                    //     )

                    // default: JsonUnionEncoding.AdjacentTag
                    //     ||| JsonUnionEncoding.UnwrapOption
                    //     ||| JsonUnionEncoding.UnwrapSingleCaseUnions
                    //     ||| JsonUnionEncoding.AllowUnorderedTag

                    options.JsonSerializerOptions.Converters.Add(new JsonFSharpConverter(
                        unionEncoding: DefaultFsharpJsonSerializationOptions.UnionEncoding));
                })
                // .AddNewtonsoftJson(options =>
                // {
                //     options.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                //     options.SerializerSettings.Formatting = Formatting.Indented;
                //     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //     options.SerializerSettings.Converters.Add(new StringEnumConverter());
                //     options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                // })
                .ConfigureApiBehaviorOptions(v =>
                {
                    v.SuppressModelStateInvalidFilter = true;
                });
            return mvcBuilder;
        }

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
            JsonSerializationStrategy? useSystemTextJson = JsonSerializationStrategy.NewtonSoft,
            params Assembly[] assembliesToScan
        )
        {
            var mvcBuilder = useSystemTextJson == JsonSerializationStrategy.NewtonSoft
                ? services.AddGlowNewtonsoftControllers(options)
                : services.AddGlowSystemTextJsonControllers(options);
            configureAdditionalMvcOptions?.Invoke(mvcBuilder);

            services.AddGlowActions(assembliesToScan);

            services.AddGlow();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "web/build";
            });

            services.AddSignalR().AddJsonProtocol(v =>
            {
                v.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddOptions();

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