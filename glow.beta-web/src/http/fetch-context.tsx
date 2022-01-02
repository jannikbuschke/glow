import * as React from "react"
import * as microsoftTeams from "@microsoft/teams-js"
import { notification } from "antd"
import { FetchContext } from "glow-core/es/actions/fetch-context"

const teamsInitialized = new Promise<string>((resolve, reject) => {
  console.log("init teams...")
  let initialized = false
  microsoftTeams.initialize(() => {
    initialized = true
    console.log("init teams done")
    microsoftTeams.appInitialization.notifySuccess()
    microsoftTeams.authentication.getAuthToken({
      successCallback: (result) => {
        resolve(result)
      },
      failureCallback: (result) => {
        notification.error({
          message: "error: " + JSON.stringify(result),
        })
        console.log("error", result)
      },
    })
  })
  setTimeout(() => {
    if (!initialized) {
      reject("timeout")
    }
  }, 1500)
})
teamsInitialized.catch((e) => {
  console.log(`Teams could not be initialized (${e.toString()})`)
})

export function TeamsFetchContextProvider({
  children,
  onError,
  onSuccess,
  onPrepareRequest,
}: React.PropsWithChildren<{
  onSuccess?: () => void
  onError?: (e: any) => void
  onPrepareRequest?: (input: RequestInfo, init?: RequestInit) => void
}>): JSX.Element | null {
  const [loading, setLoading] = React.useState(true)
  const [token, setToken] = React.useState<string | null>(null)
  const f = React.useCallback(
    (input: RequestInfo, init?: RequestInit) => {
      if (token !== null) {
        const i = init || {}
        i.headers = Object.assign(Object.assign({}, i.headers), {
          Authorization: "Bearer " + token,
        })
        onPrepareRequest && onPrepareRequest(input, i)
        return fetch(input, i)
      } else {
        const i = init || {}
        onPrepareRequest && onPrepareRequest(input, i)
        return fetch(input, i)
      }
    },
    [token, onPrepareRequest],
  )
  React.useEffect(() => {
    teamsInitialized
      .then((v) => {
        console.log("teams initialized", v)
        setToken(v)
        onSuccess && onSuccess()
      })
      .catch((e) => {
        console.error("Could not initialize teams", e)
        onError && onError(e)
      })
      .finally(() => setLoading(false))
  }, [])
  if (loading) {
    return null
  }
  return <FetchContext.Provider value={f}>{children}</FetchContext.Provider> // children// React.createElement(context.Provider, { value: f }, children);
}
