import * as React from "react"
import {
  messageSuccess as mMessageSuccess,
  notifyError as mNotifyError,
  notifyInfo as mNotifyInfo,
  notifySuccess as mNotifySuccess,
} from "./errors/mantine-notifies"
import {
  messageSuccess as aMessageSuccess,
  notifyError as aNotifyError,
  notifyInfo as aNotifyInfo,
  notifySuccess as aNotifySuccess,
} from "./errors/antd-notifies"
import { NotifyContext } from "./errors/notify-context"

type IContext = {
  componentLibrary: "antd" | "mantine"
}

const context = React.createContext<IContext>({ componentLibrary: "antd" })

export function useGlowContext() {
  const ctx = React.useContext(context)
  return ctx
}

export function GlowProvider({
  children,
  value,
}: {
  children: React.ReactNode | React.ReactNode[]
  value: IContext
}) {
  const value2 = React.useMemo(() => {
    if (value.componentLibrary === "mantine") {
      console.log("use mantine")
    } else {
      console.log("use antd")
    }
    return value.componentLibrary === "mantine"
      ? {
          messageSuccess: mMessageSuccess,
          notifyError: mNotifyError,
          notifyInfo: mNotifyInfo,
          notifySuccess: mNotifySuccess,
          //
        }
      : {
          messageSuccess: aMessageSuccess,
          notifyError: aNotifyError,
          notifyInfo: aNotifyInfo,
          notifySuccess: aNotifySuccess,
          //
        }
  }, [value])

  return (
    <NotifyContext.Provider value={value2}>
      <context.Provider value={value}>{children}</context.Provider>
    </NotifyContext.Provider>
  )
}
