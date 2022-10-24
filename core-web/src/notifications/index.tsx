import * as React from "react"
import * as signalR from "@aspnet/signalr"
import mitt from "mitt"
import * as emitt from "mitt"
import { useAuthentication } from "../authentication"
import { notification } from "antd"

interface INotificationsContext {
  emitter: emitt.Emitter<any>
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

// use
export function useNotification<T>(
  name: string,
  callback: (notification: T) => void,
  deps?: any[],
) {
  const { emitter } = useNotifications()

  React.useEffect(() => {
    const msgName = name
    const on = callback
    emitter.on(msgName, on)
    return () => {
      emitter.off(msgName, on)
    }
  }, deps)
}

const mockEmitter = mitt()
export function MockNotificationsProvider({
  children,
}: React.PropsWithChildren<{}>) {
  return (
    <NotificationsContext.Provider value={{ emitter: mockEmitter }}>
      {children}
    </NotificationsContext.Provider>
  )
}

export function NotificationsProvider({
  children,
  requireLoggedIn = true,
  disableLegacy = false,
}: React.PropsWithChildren<{
  requireLoggedIn?: boolean
  disableLegacy?: boolean
}>) {
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
    if (status === "loggedIn" || !requireLoggedIn) {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notifications")
        .configureLogging(signalR.LogLevel.Information)
        .build()
      return connection
    } else {
      console.log("skip configuring connection (not logged in)")
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

      //remove
      connection.on("message", (messageType: string, payload: any) => {
        // console.log(`emitting [[message]] ${messageType}`, payload)
        // console.log(messageType)
        emitter.emit(messageType, payload)
      })

      // this is the new one
      connection.on(
        "notification",
        (notificationType: string, notification: any) => {
          // console.log(
          //   `emitting [[notification]] ${notificationType}`,
          //   notification,
          // )
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

function BackgroundNotifications() {
  useNotification(
    "Gertrud.Generic.BackgroundQueueNotification",
    (v: any) => {
      if (process.env.NODE_ENV === "development") {
        notification.info({ message: v.title, description: v.message })
      } else {
        // production code
      }
    },
    [],
  )

  return null
}
