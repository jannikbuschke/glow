import * as React from "react"

export type FetchType = (
  input: RequestInfo,
  init?: RequestInit | undefined,
) => Promise<Response>

const FetchContext = React.createContext<FetchType>(fetch)

export { FetchContext }

export function useFetch(): FetchType {
  return React.useContext(FetchContext)
}

export function FetchContextProvider({
  children,
  onPrepareRequest,
}: React.PropsWithChildren<{
  onPrepareRequest?: (input: RequestInfo, init?: RequestInit) => void
}>): JSX.Element | null {
  const f: FetchType = React.useCallback(
    (input: RequestInfo, init?: RequestInit) => {
      const i = init || {}
      onPrepareRequest && onPrepareRequest(input, i)
      return fetch(input, i)
    },
    [onPrepareRequest],
  )

  return <FetchContext.Provider value={f}>{children}</FetchContext.Provider>
}
