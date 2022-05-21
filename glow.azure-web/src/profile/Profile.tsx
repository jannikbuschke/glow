import * as React from "react"
import { defaultProfile, useProfile } from "./use-profile"
import { ErrorBanner } from "glow-core"

export function ProfileName() {
  const { profile, error } = useProfile()
  const value = profile || defaultProfile
  return (
    <div>
      <ErrorBanner error={error} />
      <div>{value.displayName}</div>
    </div>
  )
}
