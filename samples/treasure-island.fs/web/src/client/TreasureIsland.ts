//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as System from "./System"
import * as Microsoft_FSharp_Core from "./Microsoft_FSharp_Core"
import * as Microsoft_FSharp_Collections from "./Microsoft_FSharp_Collections"
import * as System_Collections_Generic from "./System_Collections_Generic"

export type AttackModifier = {
  baseAttack: System.Int32
}
export var defaultAttackModifier: AttackModifier = {
  baseAttack: 0
}

export type Protection = {
  baseDamageReduction: System.Int32
}
export var defaultProtection: Protection = {
  baseDamageReduction: 0
}

export type Item = {
  name: System.String
  icon: System.String
  regeneration: System.Int32
  attackModifier: AttackModifier
  protection: Protection
}
export var defaultItem: Item = {
  name: '',
  icon: '',
  regeneration: 0,
  attackModifier: defaultAttackModifier,
  protection: defaultProtection
}

export type Position = {
  r: System.Int32
  q: System.Int32
  s: System.Int32
}
export var defaultPosition: Position = {
  r: 0,
  q: 0,
  s: 0
}

export type ItemPicked = {
  item: Item
  position: Position
}
export var defaultItemPicked: ItemPicked = {
  item: defaultItem,
  position: defaultPosition
}

export type ItemRemoved = {
  item: Item
  position: Position
}
export var defaultItemRemoved: ItemRemoved = {
  item: defaultItem,
  position: defaultPosition
}

export type ItemDropped = {
  position: Position
  item: Item
}
export var defaultItemDropped: ItemDropped = {
  position: defaultPosition,
  item: defaultItem
}

export type PlayerUnitId_Case_PlayerUnitId = System.Guid
export type PlayerUnitId = PlayerUnitId_Case_PlayerUnitId
export type PlayerUnitId_Case = "PlayerUnitId"
export var PlayerUnitId_AllCases = [ "PlayerUnitId" ] as const
export var defaultPlayerUnitId_Case_PlayerUnitId = '00000000-0000-0000-0000-000000000000'
export var defaultPlayerUnitId = defaultPlayerUnitId_Case_PlayerUnitId as PlayerUnitId

export type UnitMoved = {
  unitId: PlayerUnitId
  oldPosition: Position
  position: Position
}
export var defaultUnitMoved: UnitMoved = {
  unitId: defaultPlayerUnitId,
  oldPosition: defaultPosition,
  position: defaultPosition
}

export type UnitAttacked = {
  attackingUnit: System.Guid
  targetUnit: System.Guid
  damage: System.Int32
}
export var defaultUnitAttacked: UnitAttacked = {
  attackingUnit: '00000000-0000-0000-0000-000000000000',
  targetUnit: '00000000-0000-0000-0000-000000000000',
  damage: 0
}

export type GameTick = {
  data: Microsoft_FSharp_Core.Unit
}
export var defaultGameTick: GameTick = {
  data: ({})
}

export type UnitDied = {
  data: Microsoft_FSharp_Core.Unit
}
export var defaultUnitDied: UnitDied = {
  data: ({})
}

export type DamageTaken = {
  attackingUnit: System.Guid
  targetUnit: System.Guid
  damage: System.Int32
}
export var defaultDamageTaken: DamageTaken = {
  attackingUnit: '00000000-0000-0000-0000-000000000000',
  targetUnit: '00000000-0000-0000-0000-000000000000',
  damage: 0
}

export type ActiveUnitChanged = {
  unitId: PlayerUnitId
}
export var defaultActiveUnitChanged: ActiveUnitChanged = {
  unitId: defaultPlayerUnitId
}

export type UnitEnabledForWalk = {
  data: Microsoft_FSharp_Core.Unit
}
export var defaultUnitEnabledForWalk: UnitEnabledForWalk = {
  data: ({})
}

export type TileName_Case_Grass = { Case: "Grass" }
export type TileName_Case_Water = { Case: "Water" }
export type TileName_Case_Mountain = { Case: "Mountain" }
export type TileName_Case_Wood = { Case: "Wood" }
export type TileName_Case_Corn = { Case: "Corn" }
export type TileName = TileName_Case_Grass | TileName_Case_Water | TileName_Case_Mountain | TileName_Case_Wood | TileName_Case_Corn
export type TileName_Case = "Grass" | "Water" | "Mountain" | "Wood" | "Corn"
export var TileName_AllCases = [ "Grass", "Water", "Mountain", "Wood", "Corn" ] as const
export var defaultTileName_Case_Grass = { Case: "Grass" }
export var defaultTileName_Case_Water = { Case: "Water" }
export var defaultTileName_Case_Mountain = { Case: "Mountain" }
export var defaultTileName_Case_Wood = { Case: "Wood" }
export var defaultTileName_Case_Corn = { Case: "Corn" }
export var defaultTileName = defaultTileName_Case_Grass as TileName

export type Tile = {
  color: System.String
  name: TileName
  walkable: System.Boolean
  assetIds: Microsoft_FSharp_Collections.FSharpList<System.String>
}
export var defaultTile: Tile = {
  color: '',
  name: defaultTileName,
  walkable: false,
  assetIds: []
}

export type Field = {
  position: Position
  tile: Tile
  items: Microsoft_FSharp_Collections.FSharpList<Item>
}
export var defaultField: Field = {
  position: defaultPosition,
  tile: defaultTile,
  items: []
}

export type GameField = {
  fields: Microsoft_FSharp_Collections.FSharpList<Field>
}
export var defaultGameField: GameField = {
  fields: []
}

export type GameMode_Case_RoundBased = { Case: "RoundBased" }
export type GameMode_Case_Other = { Case: "Other" }
export type GameMode = GameMode_Case_RoundBased | GameMode_Case_Other
export type GameMode_Case = "RoundBased" | "Other"
export var GameMode_AllCases = [ "RoundBased", "Other" ] as const
export var defaultGameMode_Case_RoundBased = { Case: "RoundBased" }
export var defaultGameMode_Case_Other = { Case: "Other" }
export var defaultGameMode = defaultGameMode_Case_RoundBased as GameMode

export type GameCreated = {
  gameField: GameField
  mode: GameMode
}
export var defaultGameCreated: GameCreated = {
  gameField: defaultGameField,
  mode: defaultGameMode
}

export type GameRestarted = {
  gameField: GameField
}
export var defaultGameRestarted: GameRestarted = {
  gameField: defaultGameField
}

export type GameDrawn = {
  data: Microsoft_FSharp_Core.Unit
}
export var defaultGameDrawn: GameDrawn = {
  data: ({})
}

export type GameAborted = {
  data: Microsoft_FSharp_Core.Unit
}
export var defaultGameAborted: GameAborted = {
  data: ({})
}

export type PlayerId_Case_PlayerId = System.Guid
export type PlayerId = PlayerId_Case_PlayerId
export type PlayerId_Case = "PlayerId"
export var PlayerId_AllCases = [ "PlayerId" ] as const
export var defaultPlayerId_Case_PlayerId = '00000000-0000-0000-0000-000000000000'
export var defaultPlayerId = defaultPlayerId_Case_PlayerId as PlayerId

export type GameEnded = {
  winner: PlayerId
}
export var defaultGameEnded: GameEnded = {
  winner: defaultPlayerId
}

export type Player = {
  id: PlayerId
  name: System.String
}
export var defaultPlayer: Player = {
  id: defaultPlayerId,
  name: ''
}

export type Health_Case_Health = System.Int32
export type Health = Health_Case_Health
export type Health_Case = "Health"
export var Health_AllCases = [ "Health" ] as const
export var defaultHealth_Case_Health = 0
export var defaultHealth = defaultHealth_Case_Health as Health

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
export var defaultPlayerUnit: PlayerUnit = {
  id: defaultPlayerUnitId,
  playerId: defaultPlayerId,
  name: '',
  icon: '',
  assetId: '',
  position: defaultPosition,
  items: [],
  health: defaultHealth,
  isAlive: false
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
export var GameEvent_AllCases = [ "GameCreated", "GameStarted", "GameRestarted", "GameDrawn", "GameAborted", "GameEnded", "PlayerJoined", "PlayerUnitCreated", "DamageTaken", "UnitEnabledForWalk", "ActiveUnitChanged", "UnitDied", "UnitAttacked", "UnitMoved", "ItemDropped", "ItemPicked", "ItemRemoved", "GameTick" ] as const
export var defaultGameEvent_Case_GameCreated = { Case: "GameCreated", Fields: defaultGameCreated }
export var defaultGameEvent_Case_GameStarted = { Case: "GameStarted" }
export var defaultGameEvent_Case_GameRestarted = { Case: "GameRestarted", Fields: defaultGameRestarted }
export var defaultGameEvent_Case_GameDrawn = { Case: "GameDrawn", Fields: defaultGameDrawn }
export var defaultGameEvent_Case_GameAborted = { Case: "GameAborted", Fields: defaultGameAborted }
export var defaultGameEvent_Case_GameEnded = { Case: "GameEnded", Fields: defaultGameEnded }
export var defaultGameEvent_Case_PlayerJoined = { Case: "PlayerJoined", Fields: defaultPlayer }
export var defaultGameEvent_Case_PlayerUnitCreated = { Case: "PlayerUnitCreated", Fields: defaultPlayerUnit }
export var defaultGameEvent_Case_DamageTaken = { Case: "DamageTaken", Fields: defaultDamageTaken }
export var defaultGameEvent_Case_UnitEnabledForWalk = { Case: "UnitEnabledForWalk", Fields: defaultUnitEnabledForWalk }
export var defaultGameEvent_Case_ActiveUnitChanged = { Case: "ActiveUnitChanged", Fields: defaultActiveUnitChanged }
export var defaultGameEvent_Case_UnitDied = { Case: "UnitDied", Fields: defaultUnitDied }
export var defaultGameEvent_Case_UnitAttacked = { Case: "UnitAttacked", Fields: defaultUnitAttacked }
export var defaultGameEvent_Case_UnitMoved = { Case: "UnitMoved", Fields: defaultUnitMoved }
export var defaultGameEvent_Case_ItemDropped = { Case: "ItemDropped", Fields: defaultItemDropped }
export var defaultGameEvent_Case_ItemPicked = { Case: "ItemPicked", Fields: defaultItemPicked }
export var defaultGameEvent_Case_ItemRemoved = { Case: "ItemRemoved", Fields: defaultItemRemoved }
export var defaultGameEvent_Case_GameTick = { Case: "GameTick", Fields: defaultGameTick }
export var defaultGameEvent = defaultGameEvent_Case_GameCreated as GameEvent

export type GameEventNotification = {
  gameEvent: GameEvent
}
export var defaultGameEventNotification: GameEventNotification = {
  gameEvent: defaultGameEvent
}

export type GetEsEvents = {
}
export var defaultGetEsEvents: GetEsEvents = {
}

export type GameId_Case_GameId = System.Guid
export type GameId = GameId_Case_GameId
export type GameId_Case = "GameId"
export var GameId_AllCases = [ "GameId" ] as const
export var defaultGameId_Case_GameId = '00000000-0000-0000-0000-000000000000'
export var defaultGameId = defaultGameId_Case_GameId as GameId

export type Direction = {
  r: System.Int32
  q: System.Int32
  s: System.Int32
}
export var defaultDirection: Direction = {
  r: 0,
  q: 0,
  s: 0
}

export type MoveOrAttack = {
  gameId: GameId
  unitId: PlayerUnitId
  direction: Direction
}
export var defaultMoveOrAttack: MoveOrAttack = {
  gameId: defaultGameId,
  unitId: defaultPlayerUnitId,
  direction: defaultDirection
}

export type StartGame = {
  gameId: GameId
}
export var defaultStartGame: StartGame = {
  gameId: defaultGameId
}

export type StartGameRequest = {
  data: StartGame
}
export var defaultStartGameRequest: StartGameRequest = {
  data: defaultStartGame
}

export type RestartGame = {
  data: Microsoft_FSharp_Core.Unit
}
export var defaultRestartGame: RestartGame = {
  data: ({})
}

export type CreatePlayer = {
  name: System.String
  icon: System.String
  gameId: System.Guid
}
export var defaultCreatePlayer: CreatePlayer = {
  name: '',
  icon: '',
  gameId: '00000000-0000-0000-0000-000000000000'
}

export type GetPlayers = {
  data: Microsoft_FSharp_Core.Unit
}
export var defaultGetPlayers: GetPlayers = {
  data: ({})
}

export type GameStatus_Case_Initializing = { Case: "Initializing" }
export type GameStatus_Case_Running = { Case: "Running" }
export type GameStatus_Case_Paused = { Case: "Paused" }
export type GameStatus_Case_Ended = { Case: "Ended" }
export type GameStatus_Case_Aborted = { Case: "Aborted" }
export type GameStatus = GameStatus_Case_Initializing | GameStatus_Case_Running | GameStatus_Case_Paused | GameStatus_Case_Ended | GameStatus_Case_Aborted
export type GameStatus_Case = "Initializing" | "Running" | "Paused" | "Ended" | "Aborted"
export var GameStatus_AllCases = [ "Initializing", "Running", "Paused", "Ended", "Aborted" ] as const
export var defaultGameStatus_Case_Initializing = { Case: "Initializing" }
export var defaultGameStatus_Case_Running = { Case: "Running" }
export var defaultGameStatus_Case_Paused = { Case: "Paused" }
export var defaultGameStatus_Case_Ended = { Case: "Ended" }
export var defaultGameStatus_Case_Aborted = { Case: "Aborted" }
export var defaultGameStatus = defaultGameStatus_Case_Initializing as GameStatus

export type GetGames = {
  status: GameStatus
}
export var defaultGetGames: GetGames = {
  status: defaultGameStatus
}

export type GetGameState = {
  gameId: GameId
}
export var defaultGetGameState: GetGameState = {
  gameId: defaultGameId
}

export type RebuildProjections = {
  projectionName: System.String
}
export var defaultRebuildProjections: RebuildProjections = {
  projectionName: ''
}

export type GetDocuments = {
  documentName: System.String
}
export var defaultGetDocuments: GetDocuments = {
  documentName: ''
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
export var defaultEventViewmodel: EventViewmodel = {
  id: '00000000-0000-0000-0000-000000000000',
  version: 0,
  sequence: 0,
  data: {},
  streamId: '00000000-0000-0000-0000-000000000000',
  streamKey: '',
  timestamp: "0000-00-00T00:00:00+00:00",
  tenantId: '',
  eventTypeName: '',
  dotNetTypeName: '',
  causationId: '',
  correlationId: '',
  headers: ({}),
  isArchived: false,
  aggregateTypeName: ''
}

export type Error_Case_NotFound = { Case: "NotFound", Fields: System.String }
export type Error_Case_InvalidState = { Case: "InvalidState", Fields: System.String }
export type Error_Case_InvalidRequest = { Case: "InvalidRequest", Fields: System.String }
export type Error_Case_Other = { Case: "Other", Fields: System.String }
export type Error = Error_Case_NotFound | Error_Case_InvalidState | Error_Case_InvalidRequest | Error_Case_Other
export type Error_Case = "NotFound" | "InvalidState" | "InvalidRequest" | "Other"
export var Error_AllCases = [ "NotFound", "InvalidState", "InvalidRequest", "Other" ] as const
export var defaultError_Case_NotFound = { Case: "NotFound", Fields: '' }
export var defaultError_Case_InvalidState = { Case: "InvalidState", Fields: '' }
export var defaultError_Case_InvalidRequest = { Case: "InvalidRequest", Fields: '' }
export var defaultError_Case_Other = { Case: "Other", Fields: '' }
export var defaultError = defaultError_Case_NotFound as Error

export type CreatePlayerResult = {
  playerId: PlayerId
  playerUnitId: PlayerUnitId
  gameId: GameId
}
export var defaultCreatePlayerResult: CreatePlayerResult = {
  playerId: defaultPlayerId,
  playerUnitId: defaultPlayerUnitId,
  gameId: defaultGameId
}

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
export var defaultGame: Game = {
  id: '00000000-0000-0000-0000-000000000000',
  version: 0,
  tick: 0,
  status: defaultGameStatus,
  items: [],
  field: defaultGameField,
  mode: defaultGameMode,
  playerUnits: [],
  players: [],
  activeUnit: null
}

