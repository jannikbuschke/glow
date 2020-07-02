import * as React from "react"

interface IReadContext<T = any> {
  value: T
  reload: () => void
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
  reload,
  value,
  children,
}: React.PropsWithChildren<{ value: T; reload?: () => void }>) {
  const reloadValue = () => {
    console.warn("reload not implemented by this ReadContextProvider")
  }
  return (
    <ReadContext.Provider value={{ value, reload: reload || reloadValue }}>
      {children}
    </ReadContext.Provider>
  )
}
