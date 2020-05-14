import * as React from "react"

export interface Profile {
  displayname: string | null
  identityName: string | null
  isAuthenticated: boolean
  scopes: string | null[]
}

export function useProfile() {
  const [error, setError] = React.useState("")
  const [profile, setProfile] = React.useState<Profile | null>(null)

  React.useEffect(() => {
    ;(async () => {
      const response = await fetch("/api/profile/me")
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
