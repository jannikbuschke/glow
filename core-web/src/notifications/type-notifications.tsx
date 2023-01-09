import * as React from "react"
import * as signalR from "@aspnet/signalr"
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
  const [connectionClosed, setConnectionClosed] = React.useState(Math.random())
  const value = React.useMemo(
    () => ({
      emitter: mitt<Events>(),
    }),
    [],
  )

  const { emitter } = value
  const connection = React.useMemo(() => {
    if (status === "loggedIn" || !requireLoggedIn) {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notifications", { accessTokenFactory: accessTokenFactory })
        .configureLogging(signalR.LogLevel.Information)
        .build()
      return connection
    } else {
      console.log("skip configuring connection (not logged in)")
      return null
    }
  }, [connectionClosed, status])

  const ref = React.useRef(0)
  React.useEffect(() => {
    console.log("running effect")
    ;(async function setup() {
      if (connection === null) {
        return
      }
      if (ref.current === 5) {
        return
      }
      ref.current = 5

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

      console.log("add mitt emitter")
      connection.on(
        "notification",
        (notificationType: string, notification: any) => {
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
