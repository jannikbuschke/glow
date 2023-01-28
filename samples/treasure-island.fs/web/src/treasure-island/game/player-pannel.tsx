import { Hex } from "react-hexgrid"
import { Paper, Text } from "@mantine/core"
import { css } from "@emotion/react"
import { Game, Player, PlayerUnit } from "../../client/TreasureIsland"
import { FsMap } from "../fsharp-map"

function CoordinatesView({ hex: { q, r, s } }: { hex: Hex }) {
  return (
    <div>
      {q} {r} {s}
    </div>
  )
}

export function RenderPlayer({
  player,
  currentState,
}: {
  player: Player
  currentState: Game
}) {
  const activeUnit =
    currentState.activeUnit !== null
      ? FsMap.get(currentState.playerUnits, currentState.activeUnit)
      : undefined

  const playerUnit = FsMap.values(currentState.playerUnits).find(
    (v) => v.playerId === player.id,
  )
  const activeUnitIsCurrentPlayerUnit = Boolean(
    activeUnit !== undefined && activeUnit.playerId === player.id,
  )
  return (
    <Paper
      shadow="xs"
      withBorder={activeUnitIsCurrentPlayerUnit}
      sx={(theme) => ({
        border: activeUnitIsCurrentPlayerUnit
          ? `2px solid ${theme.colors.blue![3]}`
          : `2px solid ${theme.colors.gray![0]}`,
        background: activeUnitIsCurrentPlayerUnit
          ? theme.colors.blue![0]
          : undefined,
      })}
      p="xs"
      css={css`
        transition: background 0.5s, border 0.5s;
      `}
      style={
        {
          // background: "var(--mantine-color-dark-2)",
        }
      }
    >
      {!playerUnit ? (
        <div>no unit ({player.name})</div>
      ) : (
        <div>
          <div>
            <b>
              {playerUnit?.icon} {player.name}
            </b>
            <br />
            <Text color="gray">{player.id.substring(0, 8)}</Text>
            <div>
              ❤ {playerUnit.health} ({playerUnit.isAlive ? "alive" : "dead"})
            </div>
            {/* <div>⚔ {playerUnit.baseAttack}</div>
            <div>🛡 {playerUnit.baseProtection}</div> */}
            <div>
              {/* <span style={{ color: "green" }}>➕</span> {playerUnit.regenRate} */}
            </div>
            {/* 💎💍👑🧿🔮♟🗿⚱ */}
          </div>
          <div>{playerUnit.items.map((v) => v.icon).join(" ")}</div>
          <CoordinatesView hex={playerUnit.position} />
        </div>
      )}
    </Paper>
  )
}
