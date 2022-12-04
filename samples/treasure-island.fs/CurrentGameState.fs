namespace TreasureIsland

open System
open System.Linq
open System.Runtime.CompilerServices
open Glow.Glue.AspNetCore
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

      if (box current = null) then
        raise (BadRequestException("Game not found"))

      let playerIds = current.PlayerUnitIds

      let! players = session.GetPlayerUnitsAsync(playerIds)

      let dict: System.Collections.Generic.Dictionary<Guid, PlayerUnit> =
        players.ToDictionary(fun v -> v.Id)

      let state: CurrentGameState =
        { GameId = current.Id
          Units = dict
          Game = current }

      return state

    }
