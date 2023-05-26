import * as React from "react"
import {
  messageSuccess as mMessageSuccess,
  notifyError as mNotifyError,
  notifyInfo as mNotifyInfo,
  notifySuccess as mNotifySuccess,
} from "./errors/mantine-notifies"

import { NotifyContext } from "./errors/notify-context"

type IContext = {
  componentLibrary: "antd" | "mantine"
}

const context = React.createContext<IContext>({ componentLibrary: "mantine" })

export function useGlowContext() {
  const ctx = React.useContext(context)
  return ctx
}

// TODO: should be removed
export function GlowProvider({
  children,
  value,
}: {
  children: React.ReactNode | React.ReactNode[]
  value: IContext
}) {
  const value2 = React.useMemo(() => {
    return {
      messageSuccess: mMessageSuccess,
      notifyError: mNotifyError,
      notifyInfo: mNotifyInfo,
      notifySuccess: mNotifySuccess,
    }
  }, [value])

  return (
    <NotifyContext.Provider value={value2}>
      <context.Provider value={value}>{children}</context.Provider>
    </NotifyContext.Provider>
  )
}
