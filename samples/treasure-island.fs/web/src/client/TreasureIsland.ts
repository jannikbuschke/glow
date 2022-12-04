///////////////////////////////////////////////////////////
//                          This file is auto generated //
//////////////////////////////////////////////////////////

import * as System from "./System"
import * as Microsoft_FSharp_Core from "./Microsoft_FSharp_Core"
import * as System_Collections_Generic from "./System_Collections_Generic"
import * as Microsoft_FSharp_Collections from "./Microsoft_FSharp_Collections"
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
export type ItemPicked = {
  item: Item
}
export const defaultItemPicked: ItemPicked = {
  item: defaultItem,
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
export type PlayerJoined = {
  playerId: PlayerUnitId
}
export const defaultPlayerJoined: PlayerJoined = {
  playerId: defaultPlayerUnitId,
}
export type PlayerUnitInitialized = {
  position: Position
}
export const defaultPlayerUnitInitialized: PlayerUnitInitialized = {
  position: defaultPosition,
}
export type GameId = System.Guid
export const defaultGameId: GameId = System.defaultGuid
export type PlayerUnitCreated = {
  name: System.String
  gameId: GameId
  icon: System.String
}
export const defaultPlayerUnitCreated: PlayerUnitCreated = {
  name: System.defaultString,
  gameId: defaultGameId,
  icon: System.defaultString,
}
export type UnitMoved = {
  unitId: System.Guid
  oldPosition: Position
  position: Position
}
export const defaultUnitMoved: UnitMoved = {
  unitId: System.defaultGuid,
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
  unitId: System.Guid
}
export const defaultActiveUnitChanged: ActiveUnitChanged = {
  unitId: System.defaultGuid,
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
}
export const defaultTile: Tile = {
  color: System.defaultString,
  name: defaultTileName,
  walkable: System.defaultBoolean,
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
export type GameEnded = {
  winner: System.Guid
}
export const defaultGameEnded: GameEnded = {
  winner: System.defaultGuid,
}
export type GameEvent_Case_GameCreated = { Case: "GameCreated", Fields: GameCreated }
export type GameEvent_Case_GameStarted = { Case: "GameStarted" }
export type GameEvent_Case_GameRestarted = { Case: "GameRestarted", Fields: GameRestarted }
export type GameEvent_Case_GameDrawn = { Case: "GameDrawn", Fields: GameDrawn }
export type GameEvent_Case_GameAborted = { Case: "GameAborted", Fields: GameAborted }
export type GameEvent_Case_GameEnded = { Case: "GameEnded", Fields: GameEnded }
export type GameEvent_Case_PlayerJoined = { Case: "PlayerJoined", Fields: PlayerJoined }
export type GameEvent = GameEvent_Case_GameCreated | GameEvent_Case_GameStarted | GameEvent_Case_GameRestarted | GameEvent_Case_GameDrawn | GameEvent_Case_GameAborted | GameEvent_Case_GameEnded | GameEvent_Case_PlayerJoined
export type GameEvent_Case = "GameCreated" | "GameStarted" | "GameRestarted" | "GameDrawn" | "GameAborted" | "GameEnded" | "PlayerJoined"
export const GameEvent_AllCases = [ "GameCreated", "GameStarted", "GameRestarted", "GameDrawn", "GameAborted", "GameEnded", "PlayerJoined" ] as const
export const defaultGameEvent_Case_GameCreated = { Case: "GameCreated", Fields: defaultGameCreated }
export const defaultGameEvent_Case_GameStarted = { Case: "GameStarted" }
export const defaultGameEvent_Case_GameRestarted = { Case: "GameRestarted", Fields: defaultGameRestarted }
export const defaultGameEvent_Case_GameDrawn = { Case: "GameDrawn", Fields: defaultGameDrawn }
export const defaultGameEvent_Case_GameAborted = { Case: "GameAborted", Fields: defaultGameAborted }
export const defaultGameEvent_Case_GameEnded = { Case: "GameEnded", Fields: defaultGameEnded }
export const defaultGameEvent_Case_PlayerJoined = { Case: "PlayerJoined", Fields: defaultPlayerJoined }
export const defaultGameEvent = null as any as GameEvent
export type GameEventNotification = {
  gameEvent: GameEvent
}
export const defaultGameEventNotification: GameEventNotification = {
  gameEvent: defaultGameEvent,
}
export type PlayerUnit = {
  id: System.Guid
  key: PlayerUnitId
  gameId: GameId
  name: System.String
  icon: System.String
  items: Microsoft_FSharp_Collections.FSharpList<Item>
  isEnabledToWalk: System.Boolean
  position: Position
  regenRate: System.Int32
  baseAttack: System.Int32
  baseProtection: System.Int32
  health: System.Int32
  isAlive: System.Boolean
}
export const defaultPlayerUnit: PlayerUnit = {
  id: System.defaultGuid,
  key: defaultPlayerUnitId,
  gameId: defaultGameId,
  name: System.defaultString,
  icon: System.defaultString,
  items: Microsoft_FSharp_Collections.defaultFSharpList(defaultItem),
  isEnabledToWalk: System.defaultBoolean,
  position: defaultPosition,
  regenRate: System.defaultInt32,
  baseAttack: System.defaultInt32,
  baseProtection: System.defaultInt32,
  health: System.defaultInt32,
  isAlive: System.defaultBoolean,
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
export type Game = {
  id: System.Guid
  key: GameId
  version: System.Int64
  tick: System.Int32
  status: GameStatus
  items: Microsoft_FSharp_Collections.FSharpList<Item>
  field: GameField
  mode: GameMode
  playerUnitIds: Microsoft_FSharp_Collections.FSharpList<PlayerUnitId>
  activeUnit: Microsoft_FSharp_Core.FSharpOption<System.Guid>
}
export const defaultGame: Game = {
  id: System.defaultGuid,
  key: defaultGameId,
  version: System.defaultInt64,
  tick: System.defaultInt32,
  status: defaultGameStatus,
  items: Microsoft_FSharp_Collections.defaultFSharpList(defaultItem),
  field: defaultGameField,
  mode: defaultGameMode,
  playerUnitIds: Microsoft_FSharp_Collections.defaultFSharpList(defaultPlayerUnitId),
  activeUnit: Microsoft_FSharp_Core.defaultFSharpOption(System.defaultGuid),
}
export type CurrentGameState = {
  gameId: System.Guid
  units: System_Collections_Generic.Dictionary<System.Guid,PlayerUnit>
  game: Game
}
export const defaultCurrentGameState: CurrentGameState = {
  gameId: System.defaultGuid,
  units: System_Collections_Generic.defaultDictionary(System.defaultGuid,defaultPlayerUnit),
  game: defaultGame,
}
export type GetEsEvents = {
  
}
export const defaultGetEsEvents: GetEsEvents = {
  
}
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
  id: System.Guid
  direction: Direction
}
export const defaultMoveOrAttack: MoveOrAttack = {
  id: System.defaultGuid,
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
  id: PlayerUnitId
  gameId: GameId
}
export const defaultCreatePlayerResult: CreatePlayerResult = {
  id: defaultPlayerUnitId,
  gameId: defaultGameId,
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
