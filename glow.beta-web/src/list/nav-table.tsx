import { Input, Table } from "antd"
import { InputProps } from "antd/es/input"
import { ColumnType, TablePaginationConfig, TableProps } from "antd/es/table"
import * as React from "react"
import { useNavigate } from "react-router"
import { HighlightableRow } from "../antd/highlightable-row"
import { ErrorBanner, useGlowContext } from "glow-core"
import { useListContext } from "./list-context"
import styled from "styled-components"
import { Table as MantineTable } from "@mantine/core"
import { NavtableProps } from "./navbar-props"
import { CustomTable } from "./mantine-nav-table"

export function NavTable<RecordType extends { id: string } = any>(
  props: NavtableProps<RecordType>,
) {
  const { componentLibrary } = useGlowContext()

  return componentLibrary === "antd" ? (
    <AntdNavTable {...props} />
  ) : (
    <>
      <MantineNavTable {...props} />
    </>
  )
}

export function MantineNavTable<RecordType extends { id: string } = any>(
  props: NavtableProps<RecordType>,
) {
  const { path, dataSource, listPath, columns, paginate, onSelect } = props
  const navigate = useNavigate()

  return <CustomTable {...props} />
  // return (
  //   <MantineTable>
  //     <thead>
  //       <tr>
  //         {columns.map((v) => (
  //           <th>{v.title}</th>
  //         ))}
  //       </tr>
  //     </thead>
  //     <tbody>{rows}</tbody>
  //   </MantineTable>
  // <ElevatableAntdTable<RecordType>
  //   elevated={true}
  //   rowKey={(row) => row.id}
  //   components={{
  //     body: {
  //       row: (props: any) => (
  //         <HighlightableRow
  //           path={listPath || typeof path === "string" ? path : undefined}
  //           {...props}
  //         />
  //       ),
  //     },
  //   }}
  //   onRow={(record) => ({
  //     onClick: () => {
  //       if (typeof path === "function") {
  //         navigate(path(record))
  //       } else if (path !== null && path !== undefined) {
  //         navigate(path + record.id)
  //       } else {
  //         // do nothing
  //       }
  //       onSelect && onSelect(record)
  //     },
  //   })}
  //   dataSource={dataSource}
  //   pagination={paginate === true ? undefined : paginate}
  //   columns={columns
  //     ?.filter((v) => v.visible === undefined || v.visible === true)
  //     .map(({ title, key, sortable, render, ...rest }) => ({
  //       ...rest,
  //       title,
  //       key,
  //       sorter: sortable,
  //       render: (item, record) => render(record),
  //     }))}
  //   {...props}
  // />
  // )
}

export function AntdNavTable<RecordType extends { id: string } = any>({
  path,
  dataSource,
  listPath,
  columns,
  paginate,
  onSelect,
  ...props
}: NavtableProps<RecordType>) {
  const navigate = useNavigate()

  return (
    <ElevatableAntdTable<RecordType>
      elevated={true}
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
      dataSource={dataSource}
      pagination={paginate === true ? undefined : paginate}
      columns={columns
        ?.filter((v) => v.visible === undefined || v.visible === true)
        .map(({ title, key, sortable, render, ...rest }) => ({
          ...rest,
          title,
          key,
          sorter: sortable,
          render: (item, record) => render(record),
        }))}
      {...props}
    />
  )
}

export const ElevatableAntdTable = styled(Table)<{ elevated: boolean }>`
  ${({ elevated }) =>
    elevated
      ? `& .ant-table-content {
  box-shadow: 0 5px 8px rgba(0, 0, 0, 0.09), 0 3px 3px rgba(0, 0, 0, 0.13);
}`
      : `& .ant-table-content {
        box-shadow: none !important;
      }`}
` as <T>(props: TableProps<T> & { elevated: boolean }) => React.ReactElement
