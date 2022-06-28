using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Glow.Core;
using Glow.Core.Marten;
using Glow.Core.Notifications;
using Glow.Sample.TreasureIsland.Api;
using Glow.TypeScript;
using Jering.Javascript.NodeJS;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Weasel.Core;

namespace Glow.Sample;

public class Startup
{
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment env;

    public class NotificationsHub : Hub
    {
    }

    public Startup(IConfiguration config, IWebHostEnvironment env)
    {
        configuration = config;
        this.env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var assemblies = new[] { Assembly.GetEntryAssembly(), typeof(Clocks.Clock).Assembly };
        services.AddGlowApplicationServices(assembliesToScan: new[] { typeof(Startup).Assembly, typeof(Clocks.Clock).Assembly });
        // services.AddTypescriptGeneration(new TsGenerationOptions(){ Assemblies = new[] { Assembly.GetEntryAssembly()}, Path = "./web/src/ts-models/", GenerateApi = true });
        //
        // UserDto testUser = TestUsers.TestUser();
        //
        // services.AddAuthorization(options =>
        // {
        //     options.AddPolicy(Policies.Authorized, v =>
        //     {
        //         v.RequireAuthenticatedUser();
        //     });
        //     options.AddPolicy(Policies.Privileged, v =>
        //     {
        //         v.RequireAuthenticatedUser();
        //         v.RequireClaim(ClaimTypes.NameIdentifier, testUser.Id);
        //     });
        // });
        //
        // services.AddTestAuthentication(testUser.Id, testUser.DisplayName, testUser.Email);
        //
        // services.Configure<SampleConfiguration>(configuration.GetSection("sample-configuration"));


        // services.AddMediatR(typeof(Startup), typeof(Clocks.Clock));
        // services.AddAutoMapper(cfg => { cfg.AddCollectionMappers(); }, typeof(Startup));


        ApiOptions apiOptions = new() { ApiFileFirstLines = new List<string>(new[] { "/* eslint-disable prettier/prettier */" }) };
        services.AddTypescriptGeneration(new[]
        {
            new TsGenerationOptions { Assemblies = new[] { this.GetType().Assembly }, Path = "./web/src/ts-models/", GenerateApi = true, GenerateSubscriptions = true, ApiOptions=apiOptions}
        });

        services.AddHostedService<DungeonWorker>();
        services.AddNodeJS();
        // services.Configure<NodeJSProcessOptions>(options => options.ProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NodeRuntime","js")); // AppDomain.CurrentDomain.BaseDirectory is your bin/<configuration>/<targetframework> directory
        services.Configure<NodeJSProcessOptions>(options =>
            options.ProjectPath =
                Path.Combine(env.ContentRootPath, "MdxBundle", "js"));

        services
            .AddMarten(sp =>
            {
                var v = new StoreOptions();
                v.AutoCreateSchemaObjects = AutoCreate.All;
                v.Connection(configuration.GetValue<string>("ConnectionString"));
                v.Projections.SelfAggregate<Player>(ProjectionLifecycle.Inline);
                v.Projections.SelfAggregate<Game>(ProjectionLifecycle.Inline);
                var logger = sp.GetService<ILogger<MartenSubscription>>();
                v.Projections.Add(
                    new MartenSubscription(new[] { new MartenSignalrConsumer(sp) }, logger),
                    ProjectionLifecycle.Async,
                    "customConsumer"
                );
                return v;
            })
            .UseLightweightSessions()
            // Run the asynchronous projections in this node
            .AddAsyncDaemon(DaemonMode.Solo);

        services.AddGlowNotifications<NotificationsHub>();
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        // app.UseCors(options =>
        // {
        //     options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        // });
        app.UseGlow(env, configuration, options =>
        {
            options.SpaDevServerUri = "http://localhost:3004";
        });
        app.UseEndpoints(routes =>
        {
            // routes.MapControllers();
            routes.MapHub<NotificationsHub>("/notifications");
        });
    }
}