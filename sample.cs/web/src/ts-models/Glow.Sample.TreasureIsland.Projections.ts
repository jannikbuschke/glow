import { ItemDropped, Position, Item, GameStatus, GameField, GameMode } from "./Glow.Sample.TreasureIsland.Domain"
import { defaultItemDropped, defaultPosition, defaultItem, defaultGameStatus, defaultGameField, defaultGameMode } from "./Glow.Sample.TreasureIsland.Domain"

export interface Game {
  id: string
  status: GameStatus
  players: string[]
  items: ItemDropped[]
  field: GameField
  mode: GameMode
}

export const defaultGame: Game = {
  id: "00000000-0000-0000-0000-000000000000",
  status: {} as any,
  players:  [],
  items: [],
  field: {} as any,
  mode: {} as any,
}

export interface CurrentGameState {
  gameId: string
  currentItems: ItemDropped[]
  players: Player[]
  game: Game
}

export const defaultCurrentGameState: CurrentGameState = {
  gameId: "00000000-0000-0000-0000-000000000000",
  currentItems: [],
  players: [],
  game: {} as any,
}

export interface Player {
  id: string
  name: string | null
  icon: string | null
  position: Position
  items: Item[]
  isEnabledToWalk: boolean
}

export const defaultPlayer: Player = {
  id: "00000000-0000-0000-0000-000000000000",
  name: null,
  icon: null,
  position: {} as any,
  items: [],
  isEnabledToWalk: false,
}

