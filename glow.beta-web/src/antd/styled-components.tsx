import * as React from "react"
import styled from "styled-components"
import { Card as AntdCard, Table as AntdTable } from "antd"

export const Card = styled(AntdCard)`
  box-shadow: 0 5px 8px rgba(0, 0, 0, 0.09), 0 3px 3px rgba(0, 0, 0, 0.13);
`

export const Table = styled(AntdTable)`
  & .ant-table-content {
    box-shadow: 0 5px 8px rgba(0, 0, 0, 0.09), 0 3px 3px rgba(0, 0, 0, 0.13);
    background: white;
  }
`
