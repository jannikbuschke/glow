import { Card, Table } from "antd"
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
  const take = 10
  const skip = 0
  const navigate = useNavigate()
  const { data, loading } = useData<RecordType[]>(
    `${baseUrl}?take=${take}&$skip=${skip}`,
    [],
  )

  if (data.length === 0 && loading) {
    return <Card loading={true} />
  }

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
      {...props}
    />
  )
}
