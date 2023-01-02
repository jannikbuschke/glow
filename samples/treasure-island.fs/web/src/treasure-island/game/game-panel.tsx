import { Button, Center, Paper } from "@mantine/core"
import { useNotify } from "glow-core"
import { useTypedAction } from "../../client/api"
import { Game } from "../../client/TreasureIsland"

export function GamePanel({ state }: { state: Game }) {
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
          {state.activeUnit
            ? state.activeUnit?.substring(0, 4) +
              "..." +
              state.activeUnit?.substring(
                state.activeUnit.length - 4,
                state.activeUnit.length,
              )
            : "N/A"}
        </div>
        <div>Status: {state.status.Case}</div>
        <div>Tick: {state.tick}</div>
        <div>Version: {state.version}</div>
        {state.status.Case === "Initializing" && (
          <Center pt="xs">
            <Button
              loading={submitting}
              variant="white"
              onClick={async () => {
                const result = await startGame({
                  data: { gameId: state.id },
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
