import * as React from "react"
import * as microsoftTeams from "@microsoft/teams-js"
import { notification } from "antd"

type FetchType = (input: RequestInfo, init?: RequestInit) => Promise<Response>

const context = React.createContext(fetch)

export function useFetch(): FetchType {
  return React.useContext(context)
}

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
      console.log("no teams context found (timeout)")
      reject("timeout")
    }
  }, 1500)
})

export function TeamsFetchContextProvider({
  children,
}: React.PropsWithChildren<{}>): JSX.Element | null {
  const [loading, setLoading] = React.useState(true)
  const [token, setToken] = React.useState<string | null>(null)
  const f = React.useCallback(
    (input, init) => {
      if (token !== null) {
        const i = init || {}
        i.headers = Object.assign(Object.assign({}, i.headers), {
          Authorization: "Bearer " + token,
        })
        return fetch(input, i)
      } else {
        return fetch(input, init)
      }
    },
    [token],
  )
  React.useEffect(() => {
    teamsInitialized
      .then((v) => {
        console.log("teams initialized", v)
        setToken(v)
        notification.success({
          message: "Logged in",
        })
      })
      .catch((e) => {
        console.error("Could not initialize teams", e)
      })
      .finally(() => setLoading(false))
  }, [])
  if (loading) {
    return null
  }
  return <context.Provider value={f}>{children}</context.Provider> // children// React.createElement(context.Provider, { value: f }, children);
}
//# sourceMappingURL=fetch-context.js.map
