namespace TreasureIsland

open System.Runtime.CompilerServices
open Marten

[<Extension>]
type MartenExtensions() =
  [<Extension>]
  static member AppendGameEvent(ty: IDocumentSession, GameId id, e: GameEvent) = ty.Events.Append(id, [ e :> obj ])

  [<Extension>]
  static member StartGameStream(ty: IDocumentSession, GameId id, e: GameCreated) = ty.Events.StartStream(id, [ e :> obj ])

  [<Extension>]
  static member GetGameAsync(ty: IDocumentSession, GameId id) = ty.LoadAsync<Game>(id)

  [<Extension>]
  static member StartPlayerUnitStream(ty: IDocumentSession, PlayerUnitId id, e: PlayerUnitCreated) = ty.Events.StartStream(id, [ e :> obj ])

  [<Extension>]
  static member AppendPlayerUnitEvent(ty: IDocumentSession, PlayerUnitId id, e: PlayerUnitEvent) = ty.Events.Append(id, [ e :> obj ])

  [<Extension>]
  static member GetPlayerUnitAsync(ty: IDocumentSession, PlayerUnitId id) = ty.LoadAsync<PlayerUnit>(id)

  [<Extension>]
  static member GetPlayerUnitsAsync(ty: IDocumentSession, ids: PlayerUnitId list) =
    ty.LoadManyAsync<PlayerUnit>(ids |> List.map (fun (PlayerUnitId id) -> id))
