import * as React from "react"
import { useFetch } from "../http/fetch-context"
import { Profile } from "../ts-models/Glow.Core.Profiles"

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
