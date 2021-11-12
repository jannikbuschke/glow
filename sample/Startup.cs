using System;
using System.IO;
using System.Security.Claims;
using EFCoreSecondLevelCacheInterceptor;
using Glow.Azdo.Authentication;
using Glow.Configurations;
using Glow.Core;
using Glow.Core.EfCore;
using Glow.Sample.Configurations;
using Glow.Sample.Users;
using Glow.Tests;
using Glow.TypeScript;
using Glow.Users;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Sample;

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
        services.AddGlowApplicationServices(assembliesToScan: new[] { typeof(Startup).Assembly, typeof(Clocks.Clock).Assembly });

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

        // services.AddDbContext<DataContext>(options =>
        // {
        //     options.UseSqlServer(
        //         "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=glow-sample;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //     options.EnableSensitiveDataLogging(true);
        // });
        var connectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=glow-sample;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        services.AddAuthentication().AddAzdo(options =>
        {
            configuration.Bind("azdo", options);
        }, DatabaseProvider.SqlServer, connectionString);

        services.AddDbContextPool<DataContext>((serviceProvider, optionsBuilder) =>
            optionsBuilder
                .UseSqlServer(
                    connectionString,
                    sqlServerOptionsBuilder =>
                    {
                        sqlServerOptionsBuilder
                            .CommandTimeout((int) TimeSpan.FromMinutes(3).TotalSeconds)
                            .EnableRetryOnFailure();
                    })
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
        services.AddOptions();

        // services.AddTypescriptGeneration(new[]
        // {
        //     new TsGenerationOptions
        //     {
        //         Assemblies = new[] {Assembly.GetAssembly(typeof(GlowCoreModule))},
        //         Path = "../core/web/src/ts-models-core/",
        //         GenerateApi = false
        //     },
        //     new TsGenerationOptions
        //     {
        //         Assemblies = new[] {this.GetType().Assembly},
        //         Path = "./web/src/ts-models/",
        //         GenerateApi = true
        //     }
        // });

        services.AddTypescriptGeneration(new[]
        {
            new TsGenerationOptions { Assemblies = new[] { this.GetType().Assembly }, Path = "./web/src/ts-models/", GenerateApi = true, }
        });

        services.AddNodeJS();
        // services.Configure<NodeJSProcessOptions>(options => options.ProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NodeRuntime","js")); // AppDomain.CurrentDomain.BaseDirectory is your bin/<configuration>/<targetframework> directory
        services.Configure<NodeJSProcessOptions>(options =>
            options.ProjectPath =
                Path.Combine(env.ContentRootPath, "MdxBundle",
                    "js")); // AppDomain.CurrentDomain.BaseDirectory is your bin/<configuration>/<targetframework> directory

        services.AddEFSecondLevelCache(options =>
            options.UseMemoryCacheProvider().DisableLogging(false).UseCacheKeyPrefix("EF_")
        );
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
            options.SpaDevServerUri = "http://localhost:3001";
        });
    }
}