namespace GlowSample

open System
open System.Collections.Generic
open System.Reflection
open Glow.Core
open Glow.Core.Notifications
open Glow.TypeScript
open Marten
open Marten.Events.Daemon.Resiliency
open Marten.Events.Projections
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Weasel.Core
open Serilog

#nowarn "20"

module Program =

    let getPreStartLogger () =
        let envName =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")

        let env =
            if String.IsNullOrEmpty envName then
                "Production"
            else
                envName

        if env = "Production" then
            LoggerConfiguration()
                .WriteTo.File("logs/prestart-log-.txt")
                .WriteTo.Console()
                .CreateLogger()
        else
            LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger()

    let exitCode = 0

    let getBuilder (args: string array) =
        let logger: Serilog.ILogger = upcast getPreStartLogger ()
        Log.Logger = logger
        let builder = WebApplication.CreateBuilder(args)

        let services = builder.Services


        // var assemblies = new[] { A
        //                          ssembly.GetEntryAssembly(),
        //                          typeof(Clocks.Clock).Assembly,
        //                          typeof(EventsQueries.GetEsEvents).Assembly,
        //                          typeof(Glow.Core.Profiles.GetProfile).Assembly };
        //
        // services.AddGlowApplicationServices(assembliesToScan: assemblies);
        let assemblies = [|
          Assembly.GetEntryAssembly();
          typedefof<Glow.Clocks.Clock>.Assembly;
          // typedefof<EventsQueries.GetEsEvents>.Assembly;
          typedefof<Glow.Core.Profiles.GetProfile>.Assembly |]
        services.AddGlowApplicationServices(null, null, JsonSerializationStrategy.SystemTextJson, assemblies)

        let firstLine = ResizeArray()
        firstLine.Add("/* eslint-disable prettier/prettier */")
        let apiOptions = ApiOptions(ApiFileFirstLines = firstLine)

        services.AddTypescriptGeneration [| TsGenerationOptions(
                                                    Assemblies = assemblies,
                                                    Path = "./web/src/ts-models/",
                                                    GenerateApi = true,
                                                    ApiOptions = apiOptions
                                                ) |]

        // services.AddHostedService<DungeonWorker>();
          // services
          //   .AddMarten(sp =>
          //   {
          //       var v = new StoreOptions();
          //       v.AutoCreateSchemaObjects = AutoCreate.All;
          //       v.Connection(configuration.GetValue<string>("ConnectionString"));
          //       v.Projections.SelfAggregate<Unit>(ProjectionLifecycle.Inline);
          //       v.Projections.SelfAggregate<Game>(ProjectionLifecycle.Inline);
          //       var logger = sp.GetService<ILogger<MartenSubscription>>();
          //       v.Projections.Add(
          //           new MartenSubscription(new[] { new MartenSignalrConsumer(sp) }, logger),
          //           ProjectionLifecycle.Async,
          //           "customConsumer"
          //       );
          //       return v;
          //   })
          //   .UseLightweightSessions()
          //   .AddAsyncDaemon(DaemonMode.Solo);

        // services.AddTestAuthentication()

        // services.AddHostedService<ApplicationPartsLogger>()

        // builder.Services.AddAuthorization
        //     (fun options -> options.AddPolicy("Authenticated", (fun v -> v.RequireAuthenticatedUser() |> ignore)))

        builder

    [<EntryPoint>]
    let main args =

        let builder = getBuilder args

        let app = builder.Build()

        let env =
            app.Services.GetService<IWebHostEnvironment>()

        let configuration =
            app.Services.GetService<IConfiguration>()

        app.UseResponseCaching()

        app.UseGlow(env, configuration, (fun options -> options.SpaDevServerUri <- "http://localhost:3001"))

        app.Run()

        exitCode
