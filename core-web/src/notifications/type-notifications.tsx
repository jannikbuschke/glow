import * as React from "react"
import * as signalR from "@microsoft/signalr"
import mitt from "mitt"
import * as emitt from "mitt"
import { useAuthentication } from "../authentication"

interface INotificationsContext<
  Events extends Record<emitt.EventType, unknown>
> {
  emitter: emitt.Emitter<Events>
}

const NotificationsContext = React.createContext<INotificationsContext<any>>(
  null as any,
)

export function useNotifications<
  Events extends Record<emitt.EventType, unknown>
>() {
  const ctx = React.useContext(NotificationsContext)
  if (!ctx) {
    throw new Error(
      "cannot use (typed) useNotifications outside of NotificationsProvider",
    )
  }
  return ctx as INotificationsContext<Events>
}

// use
export function useNotification<
  Key extends emitt.EventType,
  Events extends Record<Key, unknown>
>(name: Key, callback: emitt.Handler<Events[Key]>, deps?: any[]) {
  const { emitter } = useNotifications<Events>()

  React.useEffect(() => {
    console.log("register handler for " + name.toString())
    emitter.on(name, callback)
    return () => {
      console.log("unregister handler for " + name.toString())
      emitter.off(name, callback)
    }
  }, deps)
}

export function useWildcardNotification<
  Events extends Record<emitt.EventType, unknown>
>(callback: emitt.WildcardHandler<Events>, deps?: any[]) {
  const { emitter } = useNotifications<Events>()

  React.useEffect(() => {
    // const msgName = name

    emitter.on("*", callback)
    return () => {
      emitter.off("*", callback)
    }
  }, deps)
}

const mockEmitter = mitt()
export function MockTypedNotificationsProvider({
  children,
}: React.PropsWithChildren<{}>) {
  return (
    <NotificationsContext.Provider value={{ emitter: mockEmitter }}>
      {children}
    </NotificationsContext.Provider>
  )
}

export function TypedNotificationsProvider<
  Events extends Record<emitt.EventType, unknown>
>({
  children,
  requireLoggedIn = true,
  disableLegacy = false,
  accessTokenFactory,
}: React.PropsWithChildren<{
  requireLoggedIn?: boolean
  disableLegacy?: boolean
  accessTokenFactory?: () => string | Promise<string>
}>) {
  const { status } = useAuthentication()
  const value = React.useMemo(
    () => ({
      emitter: mitt<Events>(),
    }),
    [],
  )

  const { emitter } = value
  React.useEffect(() => {
    if (status === "loggedIn" || !requireLoggedIn) {
      console.log("connecting signalR")

      const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notifications", { accessTokenFactory: accessTokenFactory })
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
