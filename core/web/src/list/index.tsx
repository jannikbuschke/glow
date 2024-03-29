import { Input, Table } from "antd"
import { InputProps } from "antd/lib/input"
import { ColumnType, TablePaginationConfig, TableProps } from "antd/lib/table"
import * as React from "react"
import { useNavigate } from "react-router"
import { HighlightableRow } from "../antd/highlightable-row"
import { ErrorBanner } from "../errors/error-banner"
import { useListContext } from "./list-context"
import styled from "styled-components"

export function ListSearch(props: Omit<InputProps, "value" | "onChange">) {
  const ctx = useListContext()
  const [{ search, setSearch }] = ctx.glowQuery

  return (
    <Input
      {...props}
      value={search || ""}
      onChange={(v) => setSearch(v.target.value)}
    />
  )
}

export function ListError() {
  const { glowQuery } = useListContext()
  const [, { error }] = glowQuery
  return error ? <ErrorBanner error={error} /> : null
}

// interface ListProps<T> {
//   path?: string | ((v: T) => string)
//   listPath?: string
//   columns: (Omit<ColumnType<T>, "render"> & {
//     title: string
//     key: string
//     render: (item: T) => React.ReactNode
//     sortable?: boolean
//   })[]
//   paginate?: boolean
// }

export type ListProps<RecordType extends { id: string } = any> = {
  path?: string | ((v: RecordType) => string)
  listPath?: string
  onSelect?: (v: RecordType) => void
  columns: (Omit<ColumnType<RecordType>, "render"> & {
    title: string
    key: string
    render: (item: RecordType) => React.ReactNode
    sortable?: boolean
  })[]
  paginate?: boolean
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

export function List<RecordType extends { id: string } = any>({
  path,
  listPath,
  columns,
  onSelect,
  paginate = true,
  ...props
}: ListProps<RecordType>) {
  const navigate = useNavigate()

  const ctx = useListContext()
  const [
    { result: data, setOrderBy, skip, take, setSkip, setTake },
    { isLoading },
  ] = ctx.glowQuery

  React.useEffect(() => {
    if (!paginate) {
      setTake(null)
    }
  }, [paginate])

  const pageSize = take || 0
  const currentPage = pageSize == 0 ? 1 : skip / pageSize + 1

  const pagination: false | TablePaginationConfig | undefined = paginate
    ? {
        current: currentPage,
        pageSize: pageSize || 10,
        onChange: (page, size) => {
          if (size) {
            setTake(size)
          }
          setSkip((page - 1) * pageSize)
        },
        onShowSizeChange: (page, size) => {
          setSkip(size)
        },
        total: data.count || 10,
      }
    : false

  return (
    <InternalTable<RecordType>
      elevated={true}
      loading={isLoading}
      rowKey={(row) => row.id}
      components={{
        body: {
          row: (props: any) => (
            <HighlightableRow
              path={listPath || typeof path === "string" ? path : undefined}
              {...props}
            />
          ),
        },
      }}
      onRow={(record) => ({
        onClick: () => {
          if (typeof path === "function") {
            navigate(path(record))
          } else if (path !== null && path !== undefined) {
            navigate(path + record.id)
          } else {
            // do nothing
          }
          onSelect && onSelect(record)
        },
      })}
      dataSource={data.value}
      onChange={(pagination, filters, sorter) => {
        if (Array.isArray(sorter)) {
          // not implemented
        } else {
          const { columnKey: property, order } = sorter
          if (!order) {
            setOrderBy(null)
          } else {
            setOrderBy({
              property: property as string,
              direction:
                order === "ascend"
                  ? "Asc"
                  : order === "descend"
                  ? "Desc"
                  : "Asc",
            })
          }
        }
      }}
      pagination={pagination}
      columns={columns?.map(({ title, key, sortable, render, ...rest }) => ({
        ...rest,
        title,
        key,
        sorter: sortable,
        render: (item, record) => render(record),
        // ...v,
        // filterDropdown: true,
        // filters: [
        //   {
        //     text: "Joe",
        //     value: "Joe",
        //   },
        // ],
        // filterDropdown: () => <div>hello world</div>,
        // filterDropdownVisible: true,
        // onFilter: (value, record) => false,
        // sorter: true,
      }))}
      {...props}
    />
  )
}

List.Error = ListError

List.Search = ListSearch

export const InternalTable = styled(Table)<{ elevated: boolean }>`
  ${({ elevated }) =>
    elevated
      ? `& .ant-table-content {
  box-shadow: 0 5px 8px rgba(0, 0, 0, 0.09), 0 3px 3px rgba(0, 0, 0, 0.13);
}`
      : `& .ant-table-content {
        box-shadow: none !important;
      }`}
` as <T>(props: TableProps<T> & { elevated: boolean }) => React.ReactElement

export * from "./list-context"
