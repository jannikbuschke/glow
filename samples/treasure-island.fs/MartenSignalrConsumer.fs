namespace TreasureIsland

open System
open FSharp.Control
open Glow.Core.MartenSubscriptions
open Glow.Core.Notifications
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Glow.Core.Utils

type MartenSignalrConsumer(sp: IServiceProvider) =
  interface IMartenEventsConsumer with
    member this.ConsumeAsync(documentOperations, streamActions, ct) =
      task {
        use scope = sp.CreateScope()

        let logger =
          scope.Get<ILogger<MartenSignalrConsumer>>()

        let svc =
          scope.Get<IClientNotificationService>()

        let! result =
          streamActions
          |> AsyncSeq.ofSeq
          |> AsyncSeq.iterAsync (fun actions ->
            async {
              return!
                actions.Events
                |> AsyncSeq.ofSeq
                |> AsyncSeq.iterAsync (fun e ->
                  async {

                    if e.Data :? GameEvent then
                      logger.LogInformation("Publish game event " + e.Data.GetType().Name)

                      do!
                        svc.PublishNotification({ GameEventNotification.GameEvent = e.Data :?> GameEvent })
                        |> Async.AwaitTask
                    else
                      logger.LogInformation("Skip Publish game event " + e.Data.GetType().Name)
                    //
                    // if e.Data :? IClientNotification then
                    //   logger.LogInformation("Publish event " + e.Data.GetType().Name)
                    //
                    //   do!
                    //     svc.PublishNotification(e.Data :?> IClientNotification)
                    //     |> Async.AwaitTask
                    // else
                    //   logger.LogInformation("Publish event " + e.Data.GetType().Name)

                  })
            })
        // |> Async.StartAsTask

        // |> Seq.iter (fun actions ->
        //   actions.Events
        //   |> Seq.iter (fun e ->
        //     logger.LogInformation("Publish event " + e.Data.GetType().Name)
        //     if e.Data :? IClientNotificationService then
        //       do! svc.PublishNotification(e.Data :> IClientNotificationService)
        //
        //
        //   // if (e.Data is IClientNotification clientNotification)
        //   // {
        //   //     await svc.PublishNotification(clientNotification)
        //   // }
        //
        //   ))

        // let session =
        //   scope.GetService<IDocumentSession>()
        //
        // let! games = session.Query<Game>().ToListAsync()
        // let current = games.FirstOrDefault()
        //
        // if box current <> null then
        //
        //   let players =
        //     session.LoadMany<PlayerUnit>(
        //       current.PlayerUnitIds
        //       |> List.map (fun (PlayerUnitId id) -> id)
        //     )
        //
        //   let dict =
        //     players.ToDictionary(fun v -> v.Id)
        //
        //   let notification: CurrentGameState =
        //     { GameId = current.Id
        //       Units = dict
        //       Game = current }
        //
        //   do! svc.PublishNotification(notification)
      ()
      }
