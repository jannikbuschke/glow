import * as React from "react"
import {
  QueryParameter,
  useGlowQuery,
  UseGlowQueryResult,
} from "../query/use-data"

interface IListContext<T> {
  glowQuery: UseGlowQueryResult<T>
}

const context = React.createContext<IListContext<any>>(null as any)

export function ListContext({
  url,
  children,
  initialQuery,
}: React.PropsWithChildren<{ url: string; initialQuery?: QueryParameter }>) {
  const glowQuery = useGlowQuery<any>(
    url,
    {
      value: [],
      count: null,
    },
    undefined,
    initialQuery,
  )

  return <context.Provider value={{ glowQuery }}>{children}</context.Provider>
}

export function useListContext<T = any>() {
  const ctx = React.useContext(context)
  if (ctx === null) {
    throw new Error("cannot useListContext out of context")
  }
  return ctx as IListContext<T>
}
