namespace TreasureIsland

open System
open System.Threading.Tasks
open System.Threading
open Marten
open Microsoft.Extensions.Hosting


type DungeonWorker(db: IDocumentStore, provider: IServiceProvider) =
  inherit BackgroundService()

  override this.ExecuteAsync(stoppingToken) =
    task {
      use session = db.OpenSession()
      let! games = session.Query<Game>().ToListAsync()

      games
      |> Seq.iter (fun game ->
        if (game.Status = GameStatus.Running
            || game.Status = GameStatus.Initializing) then
          let drawn: GameDrawn = { Data = () }

          session.Events.Append(game.Id, [ drawn :> obj ])
          |> ignore

      )

      let gameField = GameFieldGenerator.hexagon 5
      // GameFieldGenerator.orientedRectangle 15 15

      let id = GameId.GameId(Guid.NewGuid())

      let gameCreated: GameCreated =
        { GameField = gameField
          Mode = GameMode.RoundBased }

      session.StartGameStream(id, gameCreated) |> ignore


      do! session.SaveChangesAsync(stoppingToken)
    }
