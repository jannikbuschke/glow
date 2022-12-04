import { Button, Center, Paper } from "@mantine/core"
import { useNotify } from "glow-core"
import { useTypedAction } from "../../client/api"
import { CurrentGameState } from "../../client/TreasureIsland"

export function GamePanel({ state }: { state: CurrentGameState }) {
  const [startGame, , { submitting }] = useTypedAction("/api/start-game")
  const { notifyError } = useNotify()
  return (
    <Paper
      shadow="lg"
      withBorder={true}
      p="xs"
      sx={(theme) => ({ background: theme.colors.dark![0] })}
      // style={{ background: "var(--mantine-color-dark-2)" }}
    >
      <div>
        <div>
          active unit:{" "}
          {state.game.activeUnit
            ? state.game.activeUnit?.substring(0, 4) +
              "..." +
              state.game.activeUnit?.substring(
                state.game.activeUnit.length - 4,
                state.game.activeUnit.length,
              )
            : "N/A"}
        </div>
        <div>Status: {state.game.status.Case}</div>
        <div>Tick: {state.game.tick}</div>
        <div>Version: {state.game.version}</div>
        {state.game.status.Case === "Initializing" && (
          <Center pt="xs">
            <Button
              loading={submitting}
              variant="white"
              onClick={async () => {
                const result = await startGame({
                  data: { gameId: state.gameId },
                })
                if (!result.ok) {
                  notifyError(result.error)
                }
              }}
            >
              Start game
            </Button>
          </Center>
        )}
      </div>
    </Paper>
  )
}
