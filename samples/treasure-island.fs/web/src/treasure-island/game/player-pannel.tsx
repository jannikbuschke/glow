import { Hex } from "react-hexgrid"
import { Paper, Text } from "@mantine/core"
import { css } from "@emotion/react"
import { CurrentGameState, PlayerUnit } from "../../client/TreasureIsland"

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
  player: PlayerUnit
  currentState: CurrentGameState | null
}) {
  const isActivePlayer = Boolean(
    currentState !== null &&
      currentState !== undefined &&
      currentState.game.activeUnit !== null &&
      currentState.game.activeUnit === player.id,
  )
  return (
    <Paper
      shadow="xs"
      withBorder={isActivePlayer}
      sx={(theme) => ({
        border: isActivePlayer
          ? `2px solid ${theme.colors.blue![3]}`
          : `2px solid ${theme.colors.gray![0]}`,
        background: isActivePlayer ? theme.colors.blue![0] : undefined,
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
      <div>
        <div>
          <b>
            {player.icon} {player.name}
          </b>
          <br />
          <Text color="gray">{player.id.substring(0, 8)}</Text>
          <div>
            ❤ {player.health} ({player.isAlive ? "alive" : "dead"})
          </div>
          <div>⚔ {player.baseAttack}</div>
          <div>🛡 {player.baseProtection}</div>
          <div>
            <span style={{ color: "green" }}>➕</span> {player.regenRate}
          </div>
          {/* 💎💍👑🧿🔮♟🗿⚱ */}
        </div>
        <div>{player?.items?.map((v) => v.icon).join(" ")}</div>
        <CoordinatesView hex={player.position} />
      </div>
    </Paper>
  )
}
