import * as React from "react"
import styled from "styled-components"

export function ActionBar({
  children,
}: {
  children: React.PropsWithChildren<{}>
}) {
  return <Bar>{children}</Bar>
}

const Bar = styled.div`
  margin-top: 5px;
  margin-bottom: 5px;
  display: flex;
  flex-direction: row-reverse;
  & * {
    margin-left: 5px;
  }
`
