import * as React from "react"

type FetchType = (input: RequestInfo, init?: RequestInit) => Promise<Response>

const FetchContext = React.createContext(fetch)

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
  const f = React.useCallback(
    (input: RequestInfo, init?: RequestInit) => {
      const i = init || {}
      onPrepareRequest && onPrepareRequest(input, i)
      return fetch(input, i)
    },
    [onPrepareRequest],
  )

  return <FetchContext.Provider value={f}>{children}</FetchContext.Provider>
}
