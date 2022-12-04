module marten.stj.LogApplicationParts

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc.ApplicationParts
open Microsoft.AspNetCore.Mvc.Controllers
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open System.Linq

type ApplicationPartsLogger(_logger: ILogger<ApplicationPartsLogger>, _partManager: ApplicationPartManager) =
  interface IHostedService with
    member this.StartAsync(cancellationToken) =
      let _logger = Serilog.Log.Logger
      // Get the names of all the application parts. This is the short assembly name for AssemblyParts
      let applicationParts =
        _partManager.ApplicationParts
        |> Seq.map (fun x -> x.Name)
        |> Seq.fold (fun x y -> x + ", " + y) ""

      // Create a controller feature, and populate it from the application parts
      let controllerFeature = ControllerFeature()
      _partManager.PopulateFeature(controllerFeature)

      // Get the names of all of the controllers

      let controllers =
        controllerFeature.Controllers
        |> Seq.map (fun x -> x.Name)
        |> Seq.fold (fun x y -> x + ", " + y) ""

      _logger.Information("Applicationparts:")
      _partManager.ApplicationParts        |> Seq.map (fun x -> x.Name) |> Seq.iter(fun v -> _logger.Information(" {part}",v))

      _logger.Information("Controllers:")

      ControllerFeature().Controllers
        |> Seq.map (fun x -> x.Name)
        |> Seq.iter(fun v -> _logger.Information(" {controller}",v))

      // Log the application parts and controllers
//      _logger.LogInformation(
//        "Found the following application parts: '{ApplicationParts}' with the following controllers: '{Controllers}'",
//        applicationParts,
//        controllers
//      )

      Task.CompletedTask

    // Required by the interface

    member this.StopAsync(cancellationToken) = Task.CompletedTask
