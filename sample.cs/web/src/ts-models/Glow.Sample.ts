export type TileName = "Grass" | "Water" | "Mountain" | "Wood" | "Corn"
export const defaultTileName = "Grass"
export const TileNameValues: { [key in TileName]: TileName } = {
  Grass: "Grass",
  Water: "Water",
  Mountain: "Mountain",
  Wood: "Wood",
  Corn: "Corn",
}
export const TileNameValuesArray: TileName[] = Object.keys(TileNameValues) as TileName[]

export type GameMode = "RoundBased"
export const defaultGameMode = "RoundBased"
export const GameModeValues: { [key in GameMode]: GameMode } = {
  RoundBased: "RoundBased",
}
export const GameModeValuesArray: GameMode[] = Object.keys(GameModeValues) as GameMode[]

export type GameStatus = "Initializing" | "Running" | "Paused" | "Ended"
export const defaultGameStatus = "Initializing"
export const GameStatusValues: { [key in GameStatus]: GameStatus } = {
  Initializing: "Initializing",
  Running: "Running",
  Paused: "Paused",
  Ended: "Ended",
}
export const GameStatusValuesArray: GameStatus[] = Object.keys(GameStatusValues) as GameStatus[]

export interface AttackModifier {
  baseAttack: number
}

export const defaultAttackModifier: AttackModifier = {
  baseAttack: 0,
}

export interface Protection {
  baseDamageReduction: number
}

export const defaultProtection: Protection = {
  baseDamageReduction: 0,
}

export interface Item {
  name: string | null
  icon: string | null
  regeneration: number
  attackModifier: AttackModifier
  protection: Protection
}

export const defaultItem: Item = {
  name: null,
  icon: null,
  regeneration: 0,
  attackModifier: {} as any,
  protection: {} as any,
}

export interface ItemPicked {
  item: Item
}

export const defaultItemPicked: ItemPicked = {
  item: {} as any,
}

export interface Position {
  r: number
  q: number
  s: number
}

export const defaultPosition: Position = {
  r: 0,
  q: 0,
  s: 0,
}

export interface ItemRemoved {
  item: Item
  position: Position
}

export const defaultItemRemoved: ItemRemoved = {
  item: {} as any,
  position: {} as any,
}

export interface ItemDropped {
  position: Position
  item: Item
}

export const defaultItemDropped: ItemDropped = {
  position: {} as any,
  item: {} as any,
}

export interface PlayerJoined {
  playerId: string
}

export const defaultPlayerJoined: PlayerJoined = {
  playerId: "00000000-0000-0000-0000-000000000000",
}

export interface PlayerInitialized {
  playerId: string
  position: Position
}

export const defaultPlayerInitialized: PlayerInitialized = {
  playerId: "00000000-0000-0000-0000-000000000000",
  position: {} as any,
}

export interface PlayerCreated {
  id: string
  gameId: string
  name: string | null
  icon: string | null
}

export const defaultPlayerCreated: PlayerCreated = {
  id: "00000000-0000-0000-0000-000000000000",
  gameId: "00000000-0000-0000-0000-000000000000",
  name: null,
  icon: null,
}

export interface PlayerMoved {
  playerId: string
  oldPosition: Position
  position: Position
}

export const defaultPlayerMoved: PlayerMoved = {
  playerId: "00000000-0000-0000-0000-000000000000",
  oldPosition: {} as any,
  position: {} as any,
}

export interface PlayerAttacked {
  attackingPlayer: string
  targetPlayer: string
  damage: number
}

export const defaultPlayerAttacked: PlayerAttacked = {
  attackingPlayer: "00000000-0000-0000-0000-000000000000",
  targetPlayer: "00000000-0000-0000-0000-000000000000",
  damage: 0,
}

export interface DamageTaken {
  attackingPlayer: string
  targetPlayer: string
  damage: number
}

export const defaultDamageTaken: DamageTaken = {
  attackingPlayer: "00000000-0000-0000-0000-000000000000",
  targetPlayer: "00000000-0000-0000-0000-000000000000",
  damage: 0,
}

export interface PlayerEnabledForWalk {
}

export const defaultPlayerEnabledForWalk: PlayerEnabledForWalk = {
}

export interface GameField {
  fields: Field[]
}

export const defaultGameField: GameField = {
  fields: [],
}

export interface GameCreated {
  id: string
  field: GameField
  mode: GameMode
}

export const defaultGameCreated: GameCreated = {
  id: "00000000-0000-0000-0000-000000000000",
  field: {} as any,
  mode: {} as any,
}

export interface Tile {
  color: string | null
  name: TileName
  walkable: boolean
}

export const defaultTile: Tile = {
  color: null,
  name: {} as any,
  walkable: false,
}

export interface Field {
  position: Position
  tile: Tile
  items: Item[]
}

export const defaultField: Field = {
  position: {} as any,
  tile: {} as any,
  items: [],
}

export interface GameStarted {
}

export const defaultGameStarted: GameStarted = {
}

export interface GameRestarted {
  field: GameField
}

export const defaultGameRestarted: GameRestarted = {
  field: {} as any,
}

export interface GameEnded {
}

export const defaultGameEnded: GameEnded = {
}

export interface Game {
  id: string
  version: number
  status: GameStatus
  players: string[]
  items: ItemDropped[]
  field: GameField
  mode: GameMode
}

export const defaultGame: Game = {
  id: "00000000-0000-0000-0000-000000000000",
  version: 0,
  status: {} as any,
  players:  [],
  items: [],
  field: {} as any,
  mode: {} as any,
}

export interface CurrentGameState {
  gameId: string
  players: { [key: string]: Player }
  game: Game
}

export const defaultCurrentGameState: CurrentGameState = {
  gameId: "00000000-0000-0000-0000-000000000000",
  players: {},
  game: {} as any,
}

export interface Player {
  id: string
  gameId: string
  name: string | null
  icon: string | null
  items: Item[]
  isEnabledToWalk: boolean
  position: Position
  regenRate: number
  baseAttack: number
  baseProtection: number
  health: number
}

export const defaultPlayer: Player = {
  id: "00000000-0000-0000-0000-000000000000",
  gameId: "00000000-0000-0000-0000-000000000000",
  name: null,
  icon: null,
  items: [],
  isEnabledToWalk: false,
  position: {} as any,
  regenRate: 0,
  baseAttack: 0,
  baseProtection: 0,
  health: 0,
}

export interface Direction {
  r: number
  q: number
  s: number
}

export const defaultDirection: Direction = {
  r: 0,
  q: 0,
  s: 0,
}

