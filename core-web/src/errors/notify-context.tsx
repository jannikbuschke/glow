import * as React from "react"
import { ProblemDetails } from "../actions/use-submit"

import {
  messageSuccess,
  notifyError,
  notifyInfo,
  notifySuccess,
} from "./antd-notifies"

type INotifyContext = {
  notifyError: (r: ProblemDetails | string) => void
  notifySuccess: (message?: string) => void
  messageSuccess: (msg: React.ReactNode | string) => void
  notifyInfo: (msg: React.ReactNode | string) => void
}

export const NotifyContext = React.createContext<INotifyContext>({
  notifyError,
  messageSuccess,
  notifyInfo,
  notifySuccess,
})

export function useNotify() {
  const ctx = React.useContext(NotifyContext)
  return ctx
}
