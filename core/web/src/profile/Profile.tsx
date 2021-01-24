import * as React from "react"
import { useProfile } from "./use-profile"
import { ErrorBanner } from "../errors/error-banner"
import { Profile } from "../models"

export function ProfileName() {
  const { profile, error } = useProfile()
  const value =
    profile ||
    ({
      displayName: "",
      identityName: "",
      isAuthenticated: false,
      userId: "",
      claims: [],
      displayname: "",
      email: "",
      objectId: "",
      scopes: [],
    } as Profile)
  return (
    <div>
      <ErrorBanner error={error} />
      <div>{value.displayName}</div>
    </div>
  )
}
