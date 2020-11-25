import { Table } from "antd"
import { TableProps } from "antd/lib/table"
import * as React from "react"
import { useNavigate } from "react-router"
import { HighlightableRow } from "../antd/highlightable-row"
import { useData } from "../query/use-data"
interface ListProps {
  baseUrl: string
  path: string
}

export function List<RecordType extends { id: string } = any>({
  baseUrl,
  path,
  ...props
}: ListProps & TableProps<RecordType>) {
  const [currentPage, setCurrentPage] = React.useState(1)
  const [pageSize, setPageSize] = React.useState(10)
  const skip = (currentPage - 1) * pageSize
  const navigate = useNavigate()
  const { data, loading } = useData<RecordType[]>(
    `${baseUrl}?$take=${pageSize}&$skip=${skip}`,
    [],
  )

  const {
    data: { count },
  } = useData<{ count: number }>(`${baseUrl}?$count=true`, {
    count: pageSize,
  })

  return (
    <Table<RecordType>
      loading={loading}
      rowKey={(row) => row.id}
      components={{
        body: {
          row: (props: any) => <HighlightableRow path={path} {...props} />,
        },
      }}
      onRow={(record) => ({
        onClick: () => navigate(path + record.id),
      })}
      dataSource={data}
      pagination={{
        current: currentPage,
        pageSize: pageSize,
        onChange: (page, size) => {
          setCurrentPage(page)
        },
        onShowSizeChange: (page, size) => {
          setPageSize(size)
        },
        total: count,
      }}
      {...props}
    />
  )
}
