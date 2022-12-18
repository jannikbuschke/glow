import * as React from "react"
import { useFetch } from "glow-core"

export function useProfile(expandScopes: boolean = true) {
  const [error, setError] = React.useState("")
  const [profile, setProfile] = React.useState<Profile | null>(null)
  const fetch = useFetch()
  React.useEffect(() => {
    ;(async () => {
      const response = await fetch(`/glow/profile?expandScopes=${expandScopes}`)
      if (!response.ok) {
        setError(response.statusText + " " + (await response.text()))
      } else {
        const profile = (await response.json()) as Profile
        setProfile(profile)
      }
    })()
  }, [])

  return { profile, error }
}

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
