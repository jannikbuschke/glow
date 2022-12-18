import * as React from "react"
import { notification } from "antd"
import { useFetch } from "../actions/fetch-context"
import { useQuery } from "react-query"
// import {
//   defaultProfile,
//   Profile,
// } from "../../../glow.azure-web/src/profile/use-profile"

// copied from glow.azure
// due to unclear dependencies, this needs to be duplicated here for the time beeing
/* eslint-disable prettier/prettier */
export interface Profile {
  displayName: string | null
  id: string | null
  email: string | null
  upn: string | null
  identityName: string | null
  isAuthenticated: boolean
  objectId: string | null
  userId: string | null
  scopes: (string | null)[]
  claims: { key: any; value: any }[]
  authenticationType: string | null
}

export const defaultProfile: Profile = {
  displayName: null,
  id: null,
  email: null,
  upn: null,
  identityName: null,
  isAuthenticated: false,
  objectId: null,
  userId: null,
  scopes: [],
  claims: [],
  authenticationType: null,
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

export function VnextAuthenticationProvider(
  props: React.PropsWithChildren<{}>,
) {
  const fetch = useFetch()

  const { data, isLoading, error } = useQuery("/glow/profile/get-profile", {
    queryFn: async (ctx) => {
      const response = await fetch(ctx.queryKey, {
        method: "POST",
        headers: {
          "Content-type": "application/json",
          "x-submit-intent": "execute",
        },
        credentials: "same-origin",
        body: JSON.stringify({}),
      })
      if (response.ok) {
        const data = await response.json()
        return data as Profile
      } else {
        notification.error({
          message: "Could not check for profile information",
        })
        throw new Error("could not get profile")
      }
    },
  })
  const profile = data ? data : defaultProfile

  const status =
    profile && profile.isAuthenticated ? Status.loggedIn : Status.loggedOut

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

//maybe rename to ProfileProvider
export function AuthenticationProvider(props: React.PropsWithChildren<{}>) {
  const [status, setStatus] = React.useState<Status>(Status.checking)
  const fetch = useFetch()
  const [profile, setProfile] = React.useState<Profile>(defaultProfile)

  React.useEffect(() => {
    setStatus(Status.checking)
    fetch("/glow/profile", {
      credentials: "same-origin",
    })
      .then((v) => {
        if (v.ok) {
          return v.json() as Promise<Profile>
        } else if (v.status === 403) {
          return defaultProfile
        } else {
          throw new Error("" + v.statusText + v.status)
        }
      })
      .then((data) => {
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
