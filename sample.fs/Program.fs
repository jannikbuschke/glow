namespace sample.fs

#nowarn "20"

open System
open System.Reflection
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open Glow.Core
open Microsoft.EntityFrameworkCore
open EFCoreSecondLevelCacheInterceptor
open Glow.Tests

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

  [<EntryPoint>]
  let main args =
    let logger: ILogger = upcast getPreStartLogger ()
    Log.Logger = logger
    let builder = WebApplication.CreateBuilder(args)

    let services = builder.Services
    builder.Services.AddControllers()
    builder.Services.AddGlowApplicationServices(null, null, [| Assembly.GetEntryAssembly() |])

    services.AddTestAuthentication()

//    let connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=glow-sample-fs;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

    services
      .AddDbContextPool<DataContext>(fun serviceProvider optionsBuilder ->
        optionsBuilder
          .UseInMemoryDatabase("inmemdb")
//          .UseSqlServer(connectionString)
          .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>())
        |> ignore
        )
      .AddEFSecondLevelCache(fun options ->
        options
          .UseMemoryCacheProvider()
          .DisableLogging(false)
          .UseCacheKeyPrefix("EF_")
        |> ignore)

    builder.Services.AddAuthorization(fun options -> options.AddPolicy("Authenticated", (fun v -> v.RequireAuthenticatedUser() |> ignore)))

    let app = builder.Build()

    let env =
      app.Services.GetService<IWebHostEnvironment>()

    let configuration =
      app.Services.GetService<IConfiguration>()

    app.UseGlow(env, configuration, (fun options -> options.SpaDevServerUri <- "http://localhost:3001"))

    app.Run()

    exitCode
