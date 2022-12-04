namespace TreasureIsland

open System
open System.Linq
open System.Runtime.CompilerServices
open Glow.NotificationsCore
open Marten

type CurrentGameState =
  { GameId: Guid
    Units: System.Collections.Generic.Dictionary<Guid, TreasureIsland.PlayerUnit>
    Game: Game }
  interface IClientNotification

[<Extension>]
type Extension() =

  [<Extension>]
  static member GetCurrentGameStateAsync(session: IDocumentSession, gameId: GameId) =
    task {
      let! current = session.GetGameAsync(gameId)

      let players = current.PlayerUnits

      let dict: System.Collections.Generic.Dictionary<Guid, PlayerUnit> =
        players.ToDictionary(fun v -> v.Id)

      let state: CurrentGameState =
        { GameId = current.Id
          Units = dict
          Game = current }

      return state

    }
