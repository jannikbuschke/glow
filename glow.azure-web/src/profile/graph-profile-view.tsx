import { Button } from "antd"
import * as React from "react"
import { useData } from "glow-core/es/actions/use-data"
import { CurrentUserAvatar } from "./avatar"

export function GraphProfileView() {
  const { data, refetch } = useData<{ displayName: string; id: string }>(
    "/graph/profile",
    { displayName: "", id: "" },
  )

  return (
    <div>
      <CurrentUserAvatar />
      <Button onClick={() => refetch()}>reload</Button>
      <pre>{JSON.stringify(data, null, 4)}</pre>
    </div>
  )
}
