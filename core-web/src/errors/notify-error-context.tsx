import * as React from "react"
import { ProblemDetails } from "../actions/use-submit"
import { notifyError } from "./error-banner"

type NotifyErrorType = (r: ProblemDetails | string) => void

const context = React.createContext<NotifyErrorType>(notifyError)

// notifyError

export function useNotifyError() {
  const ctx = React.useContext(context)
  return ctx
}

export function NotifyErrorContext({
  children,
  errorHandler,
}: React.PropsWithChildren<{ errorHandler: NotifyErrorType }>) {
  return <context.Provider value={errorHandler}>{children}</context.Provider>
}
