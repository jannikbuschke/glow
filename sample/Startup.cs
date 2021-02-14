using System.Net;
using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Glow.Configurations;
using Glow.Core;
using Glow.Sample.Configurations;
using Glow.Sample.Users;
using Glow.Tests;
using Glow.TypeScript;
using Glow.Users;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Glow.Sample
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            configuration = config;
            this.env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGlowApplicationServices(assembliesToScan: new[] {
                typeof(Startup).Assembly,
                typeof(Clocks.Clock).Assembly
            });

            UserDto testUser = TestUsers.TestUser();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Authorized, v =>
                {
                    v.RequireAuthenticatedUser();
                });
                options.AddPolicy(Policies.Privileged, v =>
                {
                    v.RequireAuthenticatedUser();
                    v.RequireClaim(ClaimTypes.NameIdentifier, testUser.Id);
                });
            });

            services.AddTestAuthentication(testUser.Id, testUser.DisplayName, testUser.Email);

            services.Configure<SampleConfiguration>(configuration.GetSection("sample-configuration"));

            services.AddEfConfiguration(options =>
            {
                //options.SetPartialReadPolicy("sample-configuration", "test-policy");
                //options.SetPartialWritePolicy("sample-configuration", "test-policy");
            }, new[] { typeof(Startup).Assembly });

            // services.AddMediatR(typeof(Startup), typeof(Clocks.Clock));
            // services.AddAutoMapper(cfg => { cfg.AddCollectionMappers(); }, typeof(Startup));

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=glow-sample;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                options.EnableSensitiveDataLogging(true);
            });

            services.AddTypescriptGeneration(new TsGenerationOptions
            {
                Path = "./models/",
                Assemblies = new [] { GetType().Assembly },
                GenerateApi = false
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env
        )
        {
            app.Map("/hello", app =>
            {
                app.Run(async ctx =>
                {
                    ctx.Response.StatusCode = (int) HttpStatusCode.OK;
                    await ctx.Response.WriteAsync("hello world");
                });
            });

            app.UseGlow(env, configuration, options =>
             {
                 options.SpaDevServerUri = "http://localhost:3001";
             });
        }
    }
}
