import { Table } from "antd"
import { TableProps } from "antd/es/table"
import * as React from "react"
import { useNavigate } from "react-router"
import styled from "styled-components"
import { NavtableProps } from "./navbar-props"
import { CustomTable } from "./mantine-nav-table"

export function NavTable<RecordType extends { id: string } = any>(
  props: NavtableProps<RecordType> & { loading?: boolean },
) {
  return <MantineNavTable {...props} />
}

export function MantineNavTable<RecordType extends { id: string } = any>(
  props: NavtableProps<RecordType> & { loading?: boolean },
) {
  return <CustomTable {...props} />
}
