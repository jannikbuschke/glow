import * as React from "react"
import styled from "styled-components"

const Tr = styled.tr<{ highlight: boolean }>`
  transition: all;
  transition-duration: 1s;
  &:hover {
    background: rgb(210, 245, 255);
    cursor: pointer;
  }
  ${(props) => `background: ${props.highlight ? "#efefef" : undefined}`}
`

export function HighlightableRow({ path, ...props }: { path: string }) {
  const rowKey = props["data-row-key"]
  const highlight = window.location.pathname.startsWith(`${path}${rowKey}`)
  return <Tr {...props} highlight={highlight} />
}
