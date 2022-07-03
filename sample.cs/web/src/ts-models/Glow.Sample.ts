/* eslint-disable prettier/prettier */
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

export type GameStatus = "Initializing" | "Running" | "Paused" | "Ended" | "Aborted"
export const defaultGameStatus = "Initializing"
export const GameStatusValues: { [key in GameStatus]: GameStatus } = {
  Initializing: "Initializing",
  Running: "Running",
  Paused: "Paused",
  Ended: "Ended",
  Aborted: "Aborted",
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
  unitId: string
  position: Position
}

export const defaultPlayerInitialized: PlayerInitialized = {
  unitId: "00000000-0000-0000-0000-000000000000",
  position: {} as any,
}

export interface UnitCreated {
  id: string
  gameId: string
  name: string | null
  icon: string | null
}

export const defaultUnitCreated: UnitCreated = {
  id: "00000000-0000-0000-0000-000000000000",
  gameId: "00000000-0000-0000-0000-000000000000",
  name: null,
  icon: null,
}

export interface UnitMoved {
  unitId: string
  oldPosition: Position
  position: Position
}

export const defaultUnitMoved: UnitMoved = {
  unitId: "00000000-0000-0000-0000-000000000000",
  oldPosition: {} as any,
  position: {} as any,
}

export interface UnitAttacked {
  attackingUnit: string
  targetUnit: string
  damage: number
}

export const defaultUnitAttacked: UnitAttacked = {
  attackingUnit: "00000000-0000-0000-0000-000000000000",
  targetUnit: "00000000-0000-0000-0000-000000000000",
  damage: 0,
}

export interface GameTick {
}

export const defaultGameTick: GameTick = {
}

export interface UnitDied {
}

export const defaultUnitDied: UnitDied = {
}

export interface DamageTaken {
  attackingUnit: string
  targetUnit: string
  damage: number
}

export const defaultDamageTaken: DamageTaken = {
  attackingUnit: "00000000-0000-0000-0000-000000000000",
  targetUnit: "00000000-0000-0000-0000-000000000000",
  damage: 0,
}

export interface ActiveUnitChanged {
  unitId: string
}

export const defaultActiveUnitChanged: ActiveUnitChanged = {
  unitId: "00000000-0000-0000-0000-000000000000",
}

export interface UnitEnabledForWalk {
}

export const defaultUnitEnabledForWalk: UnitEnabledForWalk = {
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

export interface GameDrawn {
}

export const defaultGameDrawn: GameDrawn = {
}

export interface GameAborted {
}

export const defaultGameAborted: GameAborted = {
}

export interface GameEnded {
  winner: string
}

export const defaultGameEnded: GameEnded = {
  winner: "00000000-0000-0000-0000-000000000000",
}

export interface Game {
  id: string
  version: number
  tick: number
  status: GameStatus
  items: ItemDropped[]
  field: GameField
  mode: GameMode
  units: string[]
  activeUnit: string
}

export const defaultGame: Game = {
  id: "00000000-0000-0000-0000-000000000000",
  version: 0,
  tick: 0,
  status: {} as any,
  items: [],
  field: {} as any,
  mode: {} as any,
  units:  [],
  activeUnit: "00000000-0000-0000-0000-000000000000",
}

export interface CurrentGameState {
  gameId: string
  units: { [key: string]: Unit }
  game: Game
}

export const defaultCurrentGameState: CurrentGameState = {
  gameId: "00000000-0000-0000-0000-000000000000",
  units: {},
  game: {} as any,
}

export interface Unit {
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
  isAlive: boolean
}

export const defaultUnit: Unit = {
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
  isAlive: false,
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

