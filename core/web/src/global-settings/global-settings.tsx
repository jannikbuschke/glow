import * as React from "react"

export interface GlobalContext {
  [id: string]: string
}

export const GlobalSettingsContext = React.createContext<GlobalContext>({})

export function useGlobalSettings(): GlobalContext {
  const ctx = React.useContext(GlobalSettingsContext)
  if (!ctx) {
    throw new Error("cannot use GlobalSettings outside of a provider")
  }
  return ctx
}
