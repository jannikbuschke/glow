/* eslint-disable prettier/prettier */
import { Direction, GameStatus } from "./Glow.Sample"
import { defaultDirection, defaultGameStatus } from "./Glow.Sample"

export interface MoveOrAttack {
  id: string
  direction: Direction
}

export const defaultMoveOrAttack: MoveOrAttack = {
  id: "00000000-0000-0000-0000-000000000000",
  direction: {} as any,
}

export interface RestartGame {
}

export const defaultRestartGame: RestartGame = {
}

export interface CreatePlayer {
  name: string | null
  icon: string | null
}

export const defaultCreatePlayer: CreatePlayer = {
  name: null,
  icon: null,
}

export interface Join {
  playerId: string
  gameId: string
}

export const defaultJoin: Join = {
  playerId: "00000000-0000-0000-0000-000000000000",
  gameId: "00000000-0000-0000-0000-000000000000",
}

export interface GetPlayers {
}

export const defaultGetPlayers: GetPlayers = {
}

export interface GetGames {
  status: GameStatus | null
}

export const defaultGetGames: GetGames = {
  status: {} as any,
}

export interface CreatePlayerResult {
  id: string
  gameId: string
}

export const defaultCreatePlayerResult: CreatePlayerResult = {
  id: "00000000-0000-0000-0000-000000000000",
  gameId: "00000000-0000-0000-0000-000000000000",
}

