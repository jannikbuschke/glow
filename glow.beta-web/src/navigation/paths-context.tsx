import * as React from "react"

interface IPaths {
  list: string
  create: string
  id: string
}

const context = React.createContext<IPaths>(undefined as any)

export function usePaths() {
  const ctx = React.useContext(context)
  if (ctx === undefined) {
    throw new Error("cannot use 'usePaths' without context")
  }
  return ctx
}

export function PathsContextProvider(
  props: React.PropsWithChildren<{ value: IPaths }>,
) {
  return (
    <context.Provider value={props.value}>{props.children}</context.Provider>
  )
}
