using System.Net;
using System.Security.Claims;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Glow.Configurations;
using Glow.Core;
using Glow.Core.Actions;
using Glow.Core.Typescript;
using Glow.Sample.Configurations;
using Glow.Sample.Users;
using Glow.TypeScript;
using JannikB.Glue.AspNetCore.Tests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Services.Common;

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
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
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
            services.AddGlow();

            services.AddTestAuthentication(testUser.Id, testUser.DisplayName, testUser.Email);

            services.Configure<SampleConfiguration>(configuration.GetSection("sample-configuration"));

            services.AddEfConfiguration(options =>
            {
                //options.SetPartialReadPolicy("sample-configuration", "test-policy");
                //options.SetPartialWritePolicy("sample-configuration", "test-policy");
            }, new[] { typeof(Startup).Assembly });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                //options.SuppressConsumesConstraintForFormFileParameters = true;
                //options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddMediatR(typeof(Startup), typeof(Clocks.Clock));
            services.AddAutoMapper(cfg => { cfg.AddCollectionMappers(); }, typeof(Startup));

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=glow-sample;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                options.EnableSensitiveDataLogging(true);
            });

            services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddOptions();

            // Reinforced typings:
            //services.AddTransient<IStartupFilter, CreateTypescriptDefinitions>();
            services.AddTypescriptGeneration(new[] { GetType().Assembly }, true);
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc();

            app.Map("/hello", app =>
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("hello world");
                });
            });

            new string[] { "/api" }.ForEach(v =>
            {
                app.Map(v, app =>
                {
                    app.Run(async ctx =>
                    {
                        ctx.Response.StatusCode = (int) HttpStatusCode.NotFound;
                        await ctx.Response.WriteAsync("Not found");
                    });
                });
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "web";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3001");
                }
            });
        }
    }
}
