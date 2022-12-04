import { Button } from "@mantine/core"
import { showNotification, updateNotification } from "@mantine/notifications"
import { useTypedAction } from "../client/api"

export function AdminView() {
  const [restart] = useTypedAction("/api/restart-game")
  return (
    <div>
      <Button
        onClick={async () => {
          showNotification({
            id: "restarting",
            message: "restarting",
            loading: true,
          })
          const response = await restart({ data: {} })
          if (response.ok) {
            updateNotification({
              id: "restarting",
              message: "done",
              color: "green",
            })
          } else {
            updateNotification({
              id: "restarting",
              message: "error",
              color: "red",
            })
          }
        }}
      >
        Restart game
      </Button>
    </div>
  )
}
