import * as React from "react"

export function RenderObject(props: any) {
  return <pre>{JSON.stringify(props, null, 2)}</pre>
}
