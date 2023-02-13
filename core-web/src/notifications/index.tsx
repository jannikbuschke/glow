import * as React from "react"
import * as signalR from "@microsoft/signalr"
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
  const value = React.useMemo(
    () => ({
      emitter: mitt(),
    }),
    [],
  )
  const { emitter } = value
  React.useEffect(() => {
    if (status === "loggedIn" || !requireLoggedIn) {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notifications")
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: retryContext => {
            return Math.min(1000 * 2 ** retryContext.previousRetryCount, 30000);
          }
        })
        .configureLogging(signalR.LogLevel.Information)
        .build()

      console.log("[[notifications]] Start event emitter")
      async function start() {
        try {
            await connection.start();
        } catch (err) {
            setTimeout(() => start(), 5000);
        }
      }
      start()

      //remove
      connection.on("message", (messageType: string, payload: any) => {
        emitter.emit(messageType, payload)
      })

      connection.on(
        "notification",
        (notificationType: string, notification: any) => {
          emitter.emit(notificationType, notification)
        },
      )
    } else {
      console.log("skip configuring connection (not logged in)")
    }
  }, [status])

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
