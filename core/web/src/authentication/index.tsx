import * as React from "react"
import { notification } from "antd"
import { useFetch } from "../http/fetch-context"

export interface Profile {
  displayName: string | null
  identityName: string | null
  email: string | null
  objectId: string | null
  userId: string | null
  isAuthenticated: boolean
  scopes: string[]
  claims: any[]
}

export enum Status {
  checking = "cheking",
  loggedIn = "loggedIn",
  loggedOut = "loggedOut",
}

interface IAuthenticationContext {
  login: (params?: any) => void
  status: Status
  profile: Profile
  userIsAuthenticated: boolean
}

const AuthenticationContext = React.createContext<IAuthenticationContext>(
  undefined as any,
)

export function useAuthentication() {
  const ctx = React.useContext(AuthenticationContext)
  if (ctx === null || ctx === undefined) {
    throw new Error(
      "Cannot use authentication outside of AuthenticationProvider",
    )
  }
  return ctx
}

export function AuthenticationProvider(props: React.PropsWithChildren<{}>) {
  const [status, setStatus] = React.useState<Status>(Status.checking)
  const fetch = useFetch()
  const [profile, setProfile] = React.useState<Profile>({
    claims: [],
    displayName: null,
    email: null,
    userId: null,
    identityName: null,
    isAuthenticated: false,
    objectId: null,
    scopes: [],
  })

  React.useEffect(() => {
    setStatus(Status.checking)
    fetch("/glow/profile", {
      credentials: "same-origin",
    })
      .then((v) => {
        if (v.ok) {
          return v.json()
        } else {
          throw new Error("" + v.statusText + v.status)
        }
      })
      .then((data: Profile) => {
        setStatus(data.isAuthenticated ? Status.loggedIn : Status.loggedOut)
        setProfile(data)
      })
      .catch((e) => {
        notification.error({
          message: "Could not check for profile information: " + e.toString(),
        })
        setStatus(Status.loggedOut)
      })
  }, [])
  const value = React.useMemo(
    () =>
      ({
        login: (req?: any) => {
          window.location.replace(
            `/Account/SignIn?redirectUrl=${window.location.pathname}`,
          )
        },
        status,
        profile,
        userIsAuthenticated: status === "loggedIn",
      } as IAuthenticationContext),
    [status, profile],
  )

  return (
    <AuthenticationContext.Provider value={value}>
      {props.children}
    </AuthenticationContext.Provider>
  )
}
