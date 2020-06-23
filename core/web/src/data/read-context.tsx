import * as React from "react"

interface IReadContext<T = any> {
  value: T
}

const ReadContext = React.createContext<IReadContext>(undefined as any)

export function useReadContext<T>() {
  const ctx = React.useContext(ReadContext)
  if (ctx === null || ctx === undefined) {
    throw new Error("Cannot use ReadContext outside of a provider.")
  }
  return ctx as IReadContext<T>
}

export function ReadContextProvider<T>({
  value,
  children,
}: React.PropsWithChildren<{ value: T }>) {
  return (
    <ReadContext.Provider value={{ value }}>{children}</ReadContext.Provider>
  )
}
