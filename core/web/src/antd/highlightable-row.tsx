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

export function HighlightableRow(props: any & { path: string }) {
  return (
    // <Popover trigger={"contextMenu"} content={<div>hello world</div>}>
    <Tr
      {...props}
      highlight={window.location.pathname.startsWith(
        `${props.path}${props["data-row-key"]}`,
      )}
    />
    // </Popover>
  )
}
