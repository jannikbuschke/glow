import { PaginationState } from "@tanstack/react-table"
import { ColumnType, TableProps } from "antd/es/table"
import * as React from "react"

export type Pagination = {
  state: [
    PaginationState,
    React.Dispatch<React.SetStateAction<PaginationState>>,
  ]
}

export type NavtableProps<RecordType extends { id: string } = any> = {
  dataSource?: RecordType[]
  navigateOnClickTo?: (v: RecordType) => string
  path?: string | ((v: RecordType) => string)
  highlightMatchPattern?: string
  listPath?: string
  onSelect?: (v: RecordType) => void
  columns: (Omit<ColumnType<RecordType>, "render"> & {
    title: string
    key: string
    render: (item: RecordType) => React.ReactNode
    sortable?: boolean
    visible?: boolean
  })[]
  usePagination?: Pagination
  paginate?: boolean | undefined
} & Omit<
  TableProps<RecordType>,
  | "columns"
  | "loading"
  | "onRow"
  | "rowKey"
  | "components"
  | "dataSource"
  | "onChange"
  | "pagination"
>
