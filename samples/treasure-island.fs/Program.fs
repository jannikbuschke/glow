namespace TreasureIsland

open System.Text.Json.Serialization
open System.Threading.Tasks
open Glow
open Glow.Api.EventsQueries
open Glow.Core.MartenSubscriptions
open LamarCodeGeneration
open Marten.Events.Daemon.Resiliency
open Marten.Events.Projections
open Marten
open System
open System.Reflection
open Marten.Services
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open Glow.Core
open Weasel.Core
open Glow.Core.Notifications
open Glow.Azure.AzureKeyVault
open Glow.Configurations

#nowarn "20"

module Program =

  type NotificationsHub() =
    inherit Hub()

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
    let logger: Serilog.ILogger =
      upcast getPreStartLogger ()

    Log.Logger <- logger
    let assemblies =
      [| Assembly.GetEntryAssembly()
         typedefof<Glow.Clocks.Clock>.Assembly
         typedefof<GetEsEvents>.Assembly
         typedefof<Glow.Core.Profiles.GetProfile>.Assembly |]

    Glow.Core.TsGen.Generate2.renderTsTypes (assemblies |> Seq.toList)

    let builder =
      WebApplication.CreateBuilder(args)

    let services = builder.Services
    let configuration = builder.Configuration

    services.AddGlowApplicationServices(null, null, JsonSerializationStrategy.SystemTextJson, assemblies)
    |> ignore

    // services.AddTestAuthentication()
    services.AddHostedService<DungeonWorker>()

    let serializer = SystemTextJsonSerializer()
    serializer.Customize(fun v ->
        Glow.JsonSerializationSettings.ConfigureStjSerializerDefaultsForWeb v
        // v.Converters.Add(JsonFSharpConverter(Glow.JsonSerializationSettings.JsonDefaultWebUnionEncoding, allowNullFields = false))
    )
    services
      .AddMarten(fun (sp: IServiceProvider) ->
        let v = StoreOptions()
        v.GeneratedCodeMode <- TypeLoadMode.Auto
        v.AutoCreateSchemaObjects <- AutoCreate.All
        v.Connection(configuration.GetValue<string>("ConnectionString"))
        // v.Projections.SelfAggregate<PlayerUnit>(ProjectionLifecycle.Inline)
        v.Projections.SelfAggregate<Game>(ProjectionLifecycle.Inline)
        // v.UseNodaTime()
        v.Events.MetadataConfig.HeadersEnabled <- true
    
        v.Serializer(serializer)

        let logger = sp.GetService<Microsoft.Extensions.Logging.ILogger<MartenSubscription>>()
        v.Projections.Add(
            MartenSubscription([MartenSignalrConsumer(sp)], logger),
            ProjectionLifecycle.Inline,
            "customConsumer"
        )
        v)
      .UseLightweightSessions()
      .AddAsyncDaemon(DaemonMode.Solo)

    let options = System.Text.Json.JsonSerializerOptions()
    options.PropertyNamingPolicy <- System.Text.Json.JsonNamingPolicy.CamelCase
    JsonSerializationSettings.ConfigureStjSerializerDefaultsForWeb(options)   
    let serialize (obj: obj) =
       System.Text.Json.JsonSerializer.Serialize(obj, options)

    // Glow.SampleData.generateSampleData assemblies serialize
    
    services.AddFixForGlowConfigurationMissingDependencies(assemblies)
    services.AddAzureKeyvaultClientProvider()
    services.AddGlowNotifications<NotificationsHub>()
    builder

  [<EntryPoint>]
  let main2 args =

    let builder = getBuilder args

    builder.Host.UseSerilog (fun ctx lc ->
      lc
        .ReadFrom.Configuration(ctx.Configuration)
      ())

    let app = builder.Build()

    let env =
      app.Services.GetService<IWebHostEnvironment>()

    let store = app.Services.GetService<IDocumentStore>()
    store.Advanced.Clean.CompletelyRemoveAll()
    store.Storage.ApplyAllConfiguredChangesToDatabaseAsync() |> Async.AwaitTask|>Async.RunSynchronously

    let configuration =
      app.Services.GetService<IConfiguration>()

    let cs =
      configuration.GetValue<string>("ConnectionString")

    Console.WriteLine(cs)
    app.UseGlow(env, configuration, (fun options -> options.SpaDevServerUri <- "http://localhost:3004"))

    app.UseEndpoints (fun routes ->
      // routes.MapControllers();
      routes.MapHub<NotificationsHub>("/notifications")
      ())

    app.Run()

    exitCode

  // [<EntryPoint>]
  // let main args =
  //
  //   let assemblies =
  //     [| Assembly.GetEntryAssembly()
  //        typedefof<Glow.Clocks.Clock>.Assembly
  //        typedefof<GetEsEvents>.Assembly
  //        typedefof<Glow.Core.Profiles.GetProfile>.Assembly |]
  //
  //   Glow.Core.TsGen.Generate.renderTsTypes (assemblies |> Seq.toList)
  //   0
