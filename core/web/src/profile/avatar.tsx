import * as React from "react"
import { useProfile } from "./use-profile"
import { Avatar as AntAvatar } from "antd"
import styled from "styled-components"
import { ErrorBanner } from "../errors/error-banner"

export function CurrentUserAvatar({
  showDisplayName,
}: {
  showDisplayName?: boolean
}) {
  const { error, profile } = useProfile()
  if (error) {
    return <ErrorBanner error={error} />
  }
  if (profile === null) {
    return null
  }
  return (
    <HeaderProfile>
      <AntAvatar src={`/api/user/${profile.userId}/avatar`} />
      {showDisplayName && <span>{profile?.displayName || "N/A"}</span>}
    </HeaderProfile>
  )
}

const HeaderProfile = styled.div`
  display: flex;
  align-items: center;
  margin-right: 5px;
  & > * {
    margin-right: 5px;
  }
`
