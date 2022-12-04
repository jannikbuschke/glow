namespace TreasureIsland

open System
open Glow.NotificationsCore

type Position = { R: int; Q: int; S: int }

type Direction =
  { R: int
    Q: int
    S: int }
  member this.AddTo(x: Position) : Position =
    { R = this.R + x.R
      Q = this.Q + x.Q
      S = this.S + x.S }

type UnitHealed = { Health: int }

type AttackModifier = { BaseAttack: int }

type Protection = { BaseDamageReduction: int }

type Item =
  { Name: string
    Icon: string
    Regeneration: int
    AttackModifier: AttackModifier
    Protection: Protection }
// {
//     public static Item New(string name, string icon, int regeneration)
//     {
//         return new(name, icon, regeneration, new AttackModifier(0), new Protection(0));
//     }
// }
type ItemPicked =
  { Item: Item }
  interface IClientNotification

type ItemRemoved =
  { Item: Item
    Position: Position }
  interface IClientNotification

type ItemDropped =
  { Position: Position
    Item: Item }
  interface IClientNotification

type GameId = GameId of Guid

module GameId =
  let create (rawId: System.Guid) : GameId = GameId rawId
  let value (GameId rawId) : System.Guid = rawId

type PlayerUnitId = PlayerUnitId of Guid

module PlayerUnitId =
  let create (rawId: System.Guid) = PlayerUnitId rawId
  let value (PlayerUnitId rawId) : System.Guid = rawId

type PlayerJoined =
  { PlayerId: PlayerUnitId }
  interface IClientNotification

type PlayerUnitCreated =
  { PlayerUnitId: PlayerUnitId
    Name: string
    Icon: string
    Position: Position }
  interface IClientNotification

type UnitMoved =
  { UnitId: Guid
    OldPosition: Position
    Position: Position }
  interface IClientNotification

type UnitAttacked =
  { AttackingUnit: Guid
    TargetUnit: Guid
    Damage: int }
  interface IClientNotification

type GameTick =
  { Data: unit }
  interface IClientNotification

type UnitDied =
  { Data: unit }
  interface IClientNotification

type DamageTaken =
  { AttackingUnit: Guid
    TargetUnit: Guid
    Damage: int }
  interface IClientNotification

type GameMode =
  | RoundBased
  | Other

type GameStatus =
  | Initializing
  | Running
  | Paused
  | Ended
  | Aborted

type TileName =
  | Grass
  | Water
  | Mountain
  | Wood
  | Corn

type Tile =
  { Color: string
    Name: TileName
    Walkable: bool }

type Field =
  { Position: Position
    Tile: Tile
    Items: Item list }

type ActiveUnitChanged =
  { UnitId: Guid }
  interface IClientNotification

type UnitEnabledForWalk =
  { Data: unit }
  interface IClientNotification

type GameField =
  { Fields: Field list }
  member this.GetRandomPosition() = this.GetRandomField().Position

  member this.GetRandomField() =
    this.Fields[Utils1.RandomInt(this.Fields.Length)]

type GameCreated =
  { GameField: GameField
    Mode: GameMode }
  interface IClientNotification

type StartGame = { GameId: GameId }

type GameRestarted =
  { GameField: GameField }
  interface IClientNotification

type GameDrawn =
  { Data: unit }
  interface IClientNotification

type GameAborted =
  { Data: unit }
  interface IClientNotification

type GameEnded =
  { Winner: Guid }
  interface IClientNotification

type GameEvent =
  | GameCreated of GameCreated
  | GameStarted
  | GameRestarted of GameRestarted
  | GameDrawn of GameDrawn
  | GameAborted of GameAborted
  | GameEnded of GameEnded
  | PlayerJoined of PlayerJoined
  | PlayerUnitCreated of PlayerUnitCreated
  | DamageTaken of DamageTaken
  | UnitEnabledForWalk of UnitEnabledForWalk
  | ActiveUnitChanged of ActiveUnitChanged
  | UnitDied of UnitDied
  | UnitAttacked of UnitAttacked
  | UnitMoved of UnitMoved
  | ItemDropped of ItemDropped
  | ItemPicked of ItemPicked
  | ItemRemoved of ItemRemoved
  | GameTick of GameTick

type GameEventNotification =
  { GameEvent: GameEvent }
  interface IClientNotification
