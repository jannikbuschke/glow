//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as TsType from "./TsType"
import * as System from "./System"
import * as Microsoft_FSharp_Core from "./Microsoft_FSharp_Core"
import * as Microsoft_FSharp_Collections from "./Microsoft_FSharp_Collections"
import * as System_Collections_Generic from "./System_Collections_Generic"

export type AttackModifier = {
  baseAttack: System.Int32
}
export const defaultAttackModifier: AttackModifier = {
  baseAttack: System.defaultInt32,
}
export type Protection = {
  baseDamageReduction: System.Int32
}
export const defaultProtection: Protection = {
  baseDamageReduction: System.defaultInt32,
}
export type Item = {
  name: System.String
  icon: System.String
  regeneration: System.Int32
  attackModifier: AttackModifier
  protection: Protection
}
export const defaultItem: Item = {
  name: System.defaultString,
  icon: System.defaultString,
  regeneration: System.defaultInt32,
  attackModifier: defaultAttackModifier,
  protection: defaultProtection,
}
export type Position = {
  r: System.Int32
  q: System.Int32
  s: System.Int32
}
export const defaultPosition: Position = {
  r: System.defaultInt32,
  q: System.defaultInt32,
  s: System.defaultInt32,
}
export type ItemPicked = {
  item: Item
  position: Position
}
export const defaultItemPicked: ItemPicked = {
  item: defaultItem,
  position: defaultPosition,
}
export type ItemRemoved = {
  item: Item
  position: Position
}
export const defaultItemRemoved: ItemRemoved = {
  item: defaultItem,
  position: defaultPosition,
}
export type ItemDropped = {
  position: Position
  item: Item
}
export const defaultItemDropped: ItemDropped = {
  position: defaultPosition,
  item: defaultItem,
}
export type PlayerUnitId = System.Guid
export const defaultPlayerUnitId: PlayerUnitId = System.defaultGuid
export type UnitMoved = {
  unitId: PlayerUnitId
  oldPosition: Position
  position: Position
}
export const defaultUnitMoved: UnitMoved = {
  unitId: defaultPlayerUnitId,
  oldPosition: defaultPosition,
  position: defaultPosition,
}
export type UnitAttacked = {
  attackingUnit: System.Guid
  targetUnit: System.Guid
  damage: System.Int32
}
export const defaultUnitAttacked: UnitAttacked = {
  attackingUnit: System.defaultGuid,
  targetUnit: System.defaultGuid,
  damage: System.defaultInt32,
}
export type GameTick = {
  data: Microsoft_FSharp_Core.Unit
}
export const defaultGameTick: GameTick = {
  data: Microsoft_FSharp_Core.defaultUnit,
}
export type UnitDied = {
  data: Microsoft_FSharp_Core.Unit
}
export const defaultUnitDied: UnitDied = {
  data: Microsoft_FSharp_Core.defaultUnit,
}
export type DamageTaken = {
  attackingUnit: System.Guid
  targetUnit: System.Guid
  damage: System.Int32
}
export const defaultDamageTaken: DamageTaken = {
  attackingUnit: System.defaultGuid,
  targetUnit: System.defaultGuid,
  damage: System.defaultInt32,
}
export type ActiveUnitChanged = {
  unitId: PlayerUnitId
}
export const defaultActiveUnitChanged: ActiveUnitChanged = {
  unitId: defaultPlayerUnitId,
}
export type UnitEnabledForWalk = {
  data: Microsoft_FSharp_Core.Unit
}
export const defaultUnitEnabledForWalk: UnitEnabledForWalk = {
  data: Microsoft_FSharp_Core.defaultUnit,
}
export type TileName_Case_Grass = { Case: "Grass" }
export type TileName_Case_Water = { Case: "Water" }
export type TileName_Case_Mountain = { Case: "Mountain" }
export type TileName_Case_Wood = { Case: "Wood" }
export type TileName_Case_Corn = { Case: "Corn" }
export type TileName = TileName_Case_Grass | TileName_Case_Water | TileName_Case_Mountain | TileName_Case_Wood | TileName_Case_Corn
export type TileName_Case = "Grass" | "Water" | "Mountain" | "Wood" | "Corn"
export const TileName_AllCases = [ "Grass", "Water", "Mountain", "Wood", "Corn" ] as const
export const defaultTileName_Case_Grass = { Case: "Grass" }
export const defaultTileName_Case_Water = { Case: "Water" }
export const defaultTileName_Case_Mountain = { Case: "Mountain" }
export const defaultTileName_Case_Wood = { Case: "Wood" }
export const defaultTileName_Case_Corn = { Case: "Corn" }
export const defaultTileName = null as any as TileName
export type Tile = {
  color: System.String
  name: TileName
  walkable: System.Boolean
  assetIds: Microsoft_FSharp_Collections.FSharpList<System.String>
}
export const defaultTile: Tile = {
  color: System.defaultString,
  name: defaultTileName,
  walkable: System.defaultBoolean,
  assetIds: Microsoft_FSharp_Collections.defaultFSharpList(System.defaultString),
}
export type Field = {
  position: Position
  tile: Tile
  items: Microsoft_FSharp_Collections.FSharpList<Item>
}
export const defaultField: Field = {
  position: defaultPosition,
  tile: defaultTile,
  items: Microsoft_FSharp_Collections.defaultFSharpList(defaultItem),
}
export type GameField = {
  fields: Microsoft_FSharp_Collections.FSharpList<Field>
}
export const defaultGameField: GameField = {
  fields: Microsoft_FSharp_Collections.defaultFSharpList(defaultField),
}
export type GameMode_Case_RoundBased = { Case: "RoundBased" }
export type GameMode_Case_Other = { Case: "Other" }
export type GameMode = GameMode_Case_RoundBased | GameMode_Case_Other
export type GameMode_Case = "RoundBased" | "Other"
export const GameMode_AllCases = [ "RoundBased", "Other" ] as const
export const defaultGameMode_Case_RoundBased = { Case: "RoundBased" }
export const defaultGameMode_Case_Other = { Case: "Other" }
export const defaultGameMode = null as any as GameMode
export type GameCreated = {
  gameField: GameField
  mode: GameMode
}
export const defaultGameCreated: GameCreated = {
  gameField: defaultGameField,
  mode: defaultGameMode,
}
export type GameRestarted = {
  gameField: GameField
}
export const defaultGameRestarted: GameRestarted = {
  gameField: defaultGameField,
}
export type GameDrawn = {
  data: Microsoft_FSharp_Core.Unit
}
export const defaultGameDrawn: GameDrawn = {
  data: Microsoft_FSharp_Core.defaultUnit,
}
export type GameAborted = {
  data: Microsoft_FSharp_Core.Unit
}
export const defaultGameAborted: GameAborted = {
  data: Microsoft_FSharp_Core.defaultUnit,
}
export type PlayerId = System.Guid
export const defaultPlayerId: PlayerId = System.defaultGuid
export type GameEnded = {
  winner: PlayerId
}
export const defaultGameEnded: GameEnded = {
  winner: defaultPlayerId,
}
export type Player = {
  id: PlayerId
  name: System.String
}
export const defaultPlayer: Player = {
  id: defaultPlayerId,
  name: System.defaultString,
}
export type Health = System.Int32
export const defaultHealth: Health = System.defaultInt32
export type PlayerUnit = {
  id: PlayerUnitId
  playerId: PlayerId
  name: System.String
  icon: System.String
  assetId: System.String
  position: Position
  items: Microsoft_FSharp_Collections.FSharpList<Item>
  health: Health
  isAlive: System.Boolean
}
export const defaultPlayerUnit: PlayerUnit = {
  id: defaultPlayerUnitId,
  playerId: defaultPlayerId,
  name: System.defaultString,
  icon: System.defaultString,
  assetId: System.defaultString,
  position: defaultPosition,
  items: Microsoft_FSharp_Collections.defaultFSharpList(defaultItem),
  health: defaultHealth,
  isAlive: System.defaultBoolean,
}
export type GameEvent_Case_GameCreated = { Case: "GameCreated", Fields: GameCreated }
export type GameEvent_Case_GameStarted = { Case: "GameStarted" }
export type GameEvent_Case_GameRestarted = { Case: "GameRestarted", Fields: GameRestarted }
export type GameEvent_Case_GameDrawn = { Case: "GameDrawn", Fields: GameDrawn }
export type GameEvent_Case_GameAborted = { Case: "GameAborted", Fields: GameAborted }
export type GameEvent_Case_GameEnded = { Case: "GameEnded", Fields: GameEnded }
export type GameEvent_Case_PlayerJoined = { Case: "PlayerJoined", Fields: Player }
export type GameEvent_Case_PlayerUnitCreated = { Case: "PlayerUnitCreated", Fields: PlayerUnit }
export type GameEvent_Case_DamageTaken = { Case: "DamageTaken", Fields: DamageTaken }
export type GameEvent_Case_UnitEnabledForWalk = { Case: "UnitEnabledForWalk", Fields: UnitEnabledForWalk }
export type GameEvent_Case_ActiveUnitChanged = { Case: "ActiveUnitChanged", Fields: ActiveUnitChanged }
export type GameEvent_Case_UnitDied = { Case: "UnitDied", Fields: UnitDied }
export type GameEvent_Case_UnitAttacked = { Case: "UnitAttacked", Fields: UnitAttacked }
export type GameEvent_Case_UnitMoved = { Case: "UnitMoved", Fields: UnitMoved }
export type GameEvent_Case_ItemDropped = { Case: "ItemDropped", Fields: ItemDropped }
export type GameEvent_Case_ItemPicked = { Case: "ItemPicked", Fields: ItemPicked }
export type GameEvent_Case_ItemRemoved = { Case: "ItemRemoved", Fields: ItemRemoved }
export type GameEvent_Case_GameTick = { Case: "GameTick", Fields: GameTick }
export type GameEvent = GameEvent_Case_GameCreated | GameEvent_Case_GameStarted | GameEvent_Case_GameRestarted | GameEvent_Case_GameDrawn | GameEvent_Case_GameAborted | GameEvent_Case_GameEnded | GameEvent_Case_PlayerJoined | GameEvent_Case_PlayerUnitCreated | GameEvent_Case_DamageTaken | GameEvent_Case_UnitEnabledForWalk | GameEvent_Case_ActiveUnitChanged | GameEvent_Case_UnitDied | GameEvent_Case_UnitAttacked | GameEvent_Case_UnitMoved | GameEvent_Case_ItemDropped | GameEvent_Case_ItemPicked | GameEvent_Case_ItemRemoved | GameEvent_Case_GameTick
export type GameEvent_Case = "GameCreated" | "GameStarted" | "GameRestarted" | "GameDrawn" | "GameAborted" | "GameEnded" | "PlayerJoined" | "PlayerUnitCreated" | "DamageTaken" | "UnitEnabledForWalk" | "ActiveUnitChanged" | "UnitDied" | "UnitAttacked" | "UnitMoved" | "ItemDropped" | "ItemPicked" | "ItemRemoved" | "GameTick"
export const GameEvent_AllCases = [ "GameCreated", "GameStarted", "GameRestarted", "GameDrawn", "GameAborted", "GameEnded", "PlayerJoined", "PlayerUnitCreated", "DamageTaken", "UnitEnabledForWalk", "ActiveUnitChanged", "UnitDied", "UnitAttacked", "UnitMoved", "ItemDropped", "ItemPicked", "ItemRemoved", "GameTick" ] as const
export const defaultGameEvent_Case_GameCreated = { Case: "GameCreated", Fields: defaultGameCreated }
export const defaultGameEvent_Case_GameStarted = { Case: "GameStarted" }
export const defaultGameEvent_Case_GameRestarted = { Case: "GameRestarted", Fields: defaultGameRestarted }
export const defaultGameEvent_Case_GameDrawn = { Case: "GameDrawn", Fields: defaultGameDrawn }
export const defaultGameEvent_Case_GameAborted = { Case: "GameAborted", Fields: defaultGameAborted }
export const defaultGameEvent_Case_GameEnded = { Case: "GameEnded", Fields: defaultGameEnded }
export const defaultGameEvent_Case_PlayerJoined = { Case: "PlayerJoined", Fields: defaultPlayer }
export const defaultGameEvent_Case_PlayerUnitCreated = { Case: "PlayerUnitCreated", Fields: defaultPlayerUnit }
export const defaultGameEvent_Case_DamageTaken = { Case: "DamageTaken", Fields: defaultDamageTaken }
export const defaultGameEvent_Case_UnitEnabledForWalk = { Case: "UnitEnabledForWalk", Fields: defaultUnitEnabledForWalk }
export const defaultGameEvent_Case_ActiveUnitChanged = { Case: "ActiveUnitChanged", Fields: defaultActiveUnitChanged }
export const defaultGameEvent_Case_UnitDied = { Case: "UnitDied", Fields: defaultUnitDied }
export const defaultGameEvent_Case_UnitAttacked = { Case: "UnitAttacked", Fields: defaultUnitAttacked }
export const defaultGameEvent_Case_UnitMoved = { Case: "UnitMoved", Fields: defaultUnitMoved }
export const defaultGameEvent_Case_ItemDropped = { Case: "ItemDropped", Fields: defaultItemDropped }
export const defaultGameEvent_Case_ItemPicked = { Case: "ItemPicked", Fields: defaultItemPicked }
export const defaultGameEvent_Case_ItemRemoved = { Case: "ItemRemoved", Fields: defaultItemRemoved }
export const defaultGameEvent_Case_GameTick = { Case: "GameTick", Fields: defaultGameTick }
export const defaultGameEvent = null as any as GameEvent
export type GameEventNotification = {
  gameEvent: GameEvent
}
export const defaultGameEventNotification: GameEventNotification = {
  gameEvent: defaultGameEvent,
}
export type GetEsEvents = {
  
}
export const defaultGetEsEvents: GetEsEvents = {
  
}
export type GameId = System.Guid
export const defaultGameId: GameId = System.defaultGuid
export type Direction = {
  r: System.Int32
  q: System.Int32
  s: System.Int32
}
export const defaultDirection: Direction = {
  r: System.defaultInt32,
  q: System.defaultInt32,
  s: System.defaultInt32,
}
export type MoveOrAttack = {
  gameId: GameId
  unitId: PlayerUnitId
  direction: Direction
}
export const defaultMoveOrAttack: MoveOrAttack = {
  gameId: defaultGameId,
  unitId: defaultPlayerUnitId,
  direction: defaultDirection,
}
export type StartGame = {
  gameId: GameId
}
export const defaultStartGame: StartGame = {
  gameId: defaultGameId,
}
export type StartGameRequest = {
  data: StartGame
}
export const defaultStartGameRequest: StartGameRequest = {
  data: defaultStartGame,
}
export type RestartGame = {
  data: Microsoft_FSharp_Core.Unit
}
export const defaultRestartGame: RestartGame = {
  data: Microsoft_FSharp_Core.defaultUnit,
}
export type CreatePlayer = {
  name: System.String
  icon: System.String
  gameId: System.Guid
}
export const defaultCreatePlayer: CreatePlayer = {
  name: System.defaultString,
  icon: System.defaultString,
  gameId: System.defaultGuid,
}
export type GetPlayers = {
  data: Microsoft_FSharp_Core.Unit
}
export const defaultGetPlayers: GetPlayers = {
  data: Microsoft_FSharp_Core.defaultUnit,
}
export type GameStatus_Case_Initializing = { Case: "Initializing" }
export type GameStatus_Case_Running = { Case: "Running" }
export type GameStatus_Case_Paused = { Case: "Paused" }
export type GameStatus_Case_Ended = { Case: "Ended" }
export type GameStatus_Case_Aborted = { Case: "Aborted" }
export type GameStatus = GameStatus_Case_Initializing | GameStatus_Case_Running | GameStatus_Case_Paused | GameStatus_Case_Ended | GameStatus_Case_Aborted
export type GameStatus_Case = "Initializing" | "Running" | "Paused" | "Ended" | "Aborted"
export const GameStatus_AllCases = [ "Initializing", "Running", "Paused", "Ended", "Aborted" ] as const
export const defaultGameStatus_Case_Initializing = { Case: "Initializing" }
export const defaultGameStatus_Case_Running = { Case: "Running" }
export const defaultGameStatus_Case_Paused = { Case: "Paused" }
export const defaultGameStatus_Case_Ended = { Case: "Ended" }
export const defaultGameStatus_Case_Aborted = { Case: "Aborted" }
export const defaultGameStatus = null as any as GameStatus
export type GetGames = {
  status: GameStatus
}
export const defaultGetGames: GetGames = {
  status: defaultGameStatus,
}
export type GetGameState = {
  gameId: GameId
}
export const defaultGetGameState: GetGameState = {
  gameId: defaultGameId,
}
export type RebuildProjections = {
  projectionName: System.String
}
export const defaultRebuildProjections: RebuildProjections = {
  projectionName: System.defaultString,
}
export type GetDocuments = {
  documentName: System.String
}
export const defaultGetDocuments: GetDocuments = {
  documentName: System.defaultString,
}
export type CreatePlayerResult = {
  playerId: PlayerId
  playerUnitId: PlayerUnitId
  gameId: GameId
}
export const defaultCreatePlayerResult: CreatePlayerResult = {
  playerId: defaultPlayerId,
  playerUnitId: defaultPlayerUnitId,
  gameId: defaultGameId,
}
export type Error_Case_NotFound = { Case: "NotFound", Fields: System.String }
export type Error_Case_InvalidState = { Case: "InvalidState", Fields: System.String }
export type Error_Case_InvalidRequest = { Case: "InvalidRequest", Fields: System.String }
export type Error_Case_Other = { Case: "Other", Fields: System.String }
export type Error = Error_Case_NotFound | Error_Case_InvalidState | Error_Case_InvalidRequest | Error_Case_Other
export type Error_Case = "NotFound" | "InvalidState" | "InvalidRequest" | "Other"
export const Error_AllCases = [ "NotFound", "InvalidState", "InvalidRequest", "Other" ] as const
export const defaultError_Case_NotFound = { Case: "NotFound", Fields: System.defaultString }
export const defaultError_Case_InvalidState = { Case: "InvalidState", Fields: System.defaultString }
export const defaultError_Case_InvalidRequest = { Case: "InvalidRequest", Fields: System.defaultString }
export const defaultError_Case_Other = { Case: "Other", Fields: System.defaultString }
export const defaultError = null as any as Error
export type Game = {
  id: System.Guid
  version: System.Int64
  tick: System.Int32
  status: GameStatus
  items: Microsoft_FSharp_Collections.FSharpList<Item>
  field: GameField
  mode: GameMode
  playerUnits: Microsoft_FSharp_Collections.FSharpMap<PlayerUnitId,PlayerUnit>
  players: Microsoft_FSharp_Collections.FSharpMap<PlayerId,Player>
  activeUnit: Microsoft_FSharp_Core.FSharpOption<PlayerUnitId>
}
export const defaultGame: Game = {
  id: System.defaultGuid,
  version: System.defaultInt64,
  tick: System.defaultInt32,
  status: defaultGameStatus,
  items: Microsoft_FSharp_Collections.defaultFSharpList(defaultItem),
  field: defaultGameField,
  mode: defaultGameMode,
  playerUnits: Microsoft_FSharp_Collections.defaultFSharpMap(defaultPlayerUnitId,defaultPlayerUnit),
  players: Microsoft_FSharp_Collections.defaultFSharpMap(defaultPlayerId,defaultPlayer),
  activeUnit: Microsoft_FSharp_Core.defaultFSharpOption(defaultPlayerUnitId),
}
export type EventViewmodel = {
  id: System.Guid
  version: System.Int64
  sequence: System.Int64
  data: System.Object
  streamId: System.Guid
  streamKey: System.String
  timestamp: System.DateTimeOffset
  tenantId: System.String
  eventTypeName: System.String
  dotNetTypeName: System.String
  causationId: System.String
  correlationId: System.String
  headers: System_Collections_Generic.Dictionary<System.String,System.Object>
  isArchived: System.Boolean
  aggregateTypeName: System.String
}
export const defaultEventViewmodel: EventViewmodel = {
  id: System.defaultGuid,
  version: System.defaultInt64,
  sequence: System.defaultInt64,
  data: System.defaultObject,
  streamId: System.defaultGuid,
  streamKey: System.defaultString,
  timestamp: System.defaultDateTimeOffset,
  tenantId: System.defaultString,
  eventTypeName: System.defaultString,
  dotNetTypeName: System.defaultString,
  causationId: System.defaultString,
  correlationId: System.defaultString,
  headers: System_Collections_Generic.defaultDictionary(System.defaultString,System.defaultObject),
  isArchived: System.defaultBoolean,
  aggregateTypeName: System.defaultString,
}
