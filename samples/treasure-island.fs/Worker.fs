﻿namespace TreasureIsland

open System
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

      let gameField = GameFieldGenerator.hexagon 6
      // GameFieldGenerator.orientedRectangle 15 15

      let id = GameId.GameId(Guid.NewGuid())

      session.StartGameStream(id, GameEvent.GameCreated { GameField = gameField; Mode = GameMode.RoundBased }) |> ignore

      do! session.SaveChangesAsync(stoppingToken)
    }
