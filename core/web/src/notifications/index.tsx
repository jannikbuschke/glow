import * as React from "react"
import * as signalR from "@aspnet/signalr"
import mitt from "mitt"
import { useAuthentication } from "../authentication"

interface INotificationsContext {
  emitter: mitt.Emitter
}

const NotificationsContext = React.createContext<INotificationsContext>(
  null as any,
)

export interface Message {
  kind: string
  payload: { entityName: string; operation: string }
}

export function useNotifications() {
  const ctx = React.useContext(NotificationsContext)
  if (!ctx) {
    throw new Error(
      "cannot use useNotifications outside of NotificationsProvider",
    )
  }
  return ctx
}

export function NotificationsProvider({
  children,
}: React.PropsWithChildren<{}>) {
  const { status } = useAuthentication()
  const [connectionClosed, setConnectionClosed] = React.useState(Math.random())
  const value = React.useMemo(
    () => ({
      emitter: mitt(),
    }),
    [],
  )
  const { emitter } = value
  const connection = React.useMemo(() => {
    if (status === "loggedIn") {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notifications")
        .configureLogging(signalR.LogLevel.Information)
        .build()
      return connection
    } else {
      return null
    }
  }, [connectionClosed, status])

  React.useEffect(() => {
    ;(async function setup() {
      if (connection === null) {
        return
      }
      connection.onclose((error) => {
        console.log("[[notifications]] closed", error)
        setTimeout(() => setConnectionClosed(Math.random()), 3000)
      })
      console.log("[[notifications]] Start event emitter")
      connection
        .start()
        .then(function () {
          console.log("[[notifications]] event emitter connected")
        })
        .catch((e) => {
          console.error("[[notifications]] could not start signalr connection")
          console.error(e)
        })

      connection.on("message", (messageType: string, payload: any) => {
        console.log(`emitting [[message]] ${messageType}`, payload)
        console.log(messageType)
        emitter.emit(messageType, payload)
      })

      connection.on(
        "notification",
        (notificationType: string, notification: any) => {
          console.log(
            `emitting [[notification]] ${notificationType}`,
            notification,
          )
          emitter.emit(notificationType, notification)
        },
      )
    })()
  }, [connection, emitter])

  return (
    <NotificationsContext.Provider value={value}>
      {children}
    </NotificationsContext.Provider>
  )
}
