using System;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Glow.Authentication.Aad;
using Glow.Configurations;
using Glow.Core;
using Glow.Core.EfMsalTokenStore;
using Glow.Glue.AspNetCore;
using Glow.Tests;
using Glow.TypeScript;
using Glow.Users;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TemplateName
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;

        public Startup(IWebHostEnvironment env, IConfiguration configuration, ILogger<Startup> logger)
        {
            this.env = env;
            this.configuration = configuration;
            logger.LogInformation($"Configuring: '{env.ApplicationName}'");
            logger.LogInformation($"Environment: '{env.EnvironmentName}'");
            logger.LogInformation($"ContentRoot: '{env.ContentRootPath}'");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomAuthentication(env, configuration);
            services.AddCustomAuthorization();

            services.AddGlowApplicationServices(assembliesToScan: new[]
            {
                typeof(Startup).Assembly, typeof(Glow.Core.ServiceExtensions).Assembly
            });

            services.AddApplicationInsightsTelemetry();

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetValue<string>("ConnectionString"));
            });

            services.AddTypescriptGeneration(new TsGenerationOptions
            {
                Path = "./web/src/ts-models/", Assemblies = new[] {typeof(Startup).Assembly}, GenerateApi = true
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env
        )
        {
            app.UseCors(options =>
            {
                options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });
            app.UseGlow(env, configuration, options =>
            {
                options.SpaDevServerUri = "http://localhost:3000";
            });
        }
    }

    public static class StartupExtensions
    {
        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //options.AddPolicy(Policies.Admin, builder =>
                //{
                //    builder.RequireAuthenticatedUser();
                //    builder.Requirements.Add(new IsAdminRequirement());
                //});

                //options.AddPolicy(Policies.Planner, builder =>
                //{
                //    builder.RequireAuthenticatedUser();
                //    builder.Requirements.Add(new IsPlannerRequirement());
                //});

                //options.AddPolicy(Policies.AuthenticatedUser, builder =>
                //{
                //    builder.RequireAuthenticatedUser();
                //});
            });
        }

        public static void AddCustomAuthentication(
            this IServiceCollection services,
            IWebHostEnvironment env,
            IConfiguration configuration
        )
        {
            services.AddGlowAadIntegration(env, configuration);

            var testUser = new UserDto {DisplayName = "testuser", Email = "test@sample.com", Id = "1"};
            if (env.IsDevelopment() && configuration.MockExternalSystems())
            {
                services.AddTestAuthentication(
                    testUser.Id,
                    testUser.DisplayName,
                    testUser.Email,
                    new[]
                    {
                        new Claim(ClaimsPrincipalExtensions.ObjectId, testUser.Id),
                        new Claim(ClaimsPrincipalExtensions.TenantId, "our-comp-tenant@id.com")
                    });
            }
            else
            {
                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    })
                    .AddAzureAd(options =>
                    {
                        configuration.Bind("OpenIdConnect", options);

                        if (string.IsNullOrEmpty(options.ClientSecret))
                        {
                            options.ClientSecret = configuration["ClientSecret"] ??
                                                   throw new Exception("No Clientsecret configured");
                        }
                    });
            }
        }
    }
}