import * as React from "react"
import styled from "styled-components"
import ButtonGroup from "antd/lib/button/button-group"

export function ActionBar({
  children,
}: {
  children: React.PropsWithChildren<{}>
}) {
  return <Bar>{children}</Bar>
}

const Bar = styled(ButtonGroup)`
  margin-top: 5px;
  margin-bottom: 5px;
  display: flex;
  flex-direction: row-reverse;
`
