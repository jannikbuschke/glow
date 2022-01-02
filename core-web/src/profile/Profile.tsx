import * as React from "react"
import { useProfile } from "./use-profile"
import { ErrorBanner } from "../errors/error-banner"
import { defaultProfile } from "../ts-models/Glow.Core.Profiles"

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
