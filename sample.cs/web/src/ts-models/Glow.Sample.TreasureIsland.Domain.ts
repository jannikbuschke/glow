export type GameStatus = "Initializing" | "Running" | "Paused" | "Ended"
export const defaultGameStatus = "Initializing"
export const GameStatusValues: { [key in GameStatus]: GameStatus } = {
  Initializing: "Initializing",
  Running: "Running",
  Paused: "Paused",
  Ended: "Ended",
}
export const GameStatusValuesArray: GameStatus[] = Object.keys(GameStatusValues) as GameStatus[]

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

export interface Item {
  name: string | null
  icon: string | null
}

export const defaultItem: Item = {
  name: null,
  icon: null,
}

export interface ItemDropped {
  position: Position
  item: Item
}

export const defaultItemDropped: ItemDropped = {
  position: {} as any,
  item: {} as any,
}

export interface GameField {
  fields: Field[]
}

export const defaultGameField: GameField = {
  fields: [],
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
}

export const defaultField: Field = {
  position: {} as any,
  tile: {} as any,
}

export interface PlayerJoined {
  playerId: string
}

export const defaultPlayerJoined: PlayerJoined = {
  playerId: "00000000-0000-0000-0000-000000000000",
}

export interface PlayerInitialized {
  position: Position
}

export const defaultPlayerInitialized: PlayerInitialized = {
  position: {} as any,
}

export interface PlayerCreated {
  id: string
  name: string | null
  icon: string | null
  position: Position
}

export const defaultPlayerCreated: PlayerCreated = {
  id: "00000000-0000-0000-0000-000000000000",
  name: null,
  icon: null,
  position: {} as any,
}

export interface PlayerMoved {
  id: string
  position: Position
}

export const defaultPlayerMoved: PlayerMoved = {
  id: "00000000-0000-0000-0000-000000000000",
  position: {} as any,
}

export interface PlayerEnabledForWalk {
}

export const defaultPlayerEnabledForWalk: PlayerEnabledForWalk = {
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

export interface ItemPicked {
  item: Item
}

export const defaultItemPicked: ItemPicked = {
  item: {} as any,
}

export interface PlayerPickedItem {
  position: Position
  item: Item
  playerId: string
}

export const defaultPlayerPickedItem: PlayerPickedItem = {
  position: {} as any,
  item: {} as any,
  playerId: "00000000-0000-0000-0000-000000000000",
}

