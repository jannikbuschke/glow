namespace TreasureIsland

open System.Runtime.CompilerServices
open Glow.Glue.AspNetCore
open Marten

[<Extension>]
type MartenExtensions() =
  [<Extension>]
  static member AppendGameEvent(ty: IDocumentSession, GameId id, e: GameEvent) = ty.Events.Append(id, [ e :> obj ])

  [<Extension>]
  static member StartGameStream(ty: IDocumentSession, GameId id, e: GameEvent) = ty.Events.StartStream(id, [ e :> obj ])

  [<Extension>]
  static member GetGameAsync(ty: IDocumentSession, GameId id) =
    task{
      let! result = ty.LoadAsync<Game>(id)
      if (box result = null) then
        raise (BadRequestException("Game not found"))
      return result
    }
