import * as React from "react"
import { Profile } from "../models"

export function useProfile() {
  const [error, setError] = React.useState("")
  const [profile, setProfile] = React.useState<Profile | null>(null)

  React.useEffect(() => {
    ;(async () => {
      const response = await fetch("/glow/profile")
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
