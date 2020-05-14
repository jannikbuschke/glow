import * as React from "react"
import { OdataTable, useOdataTable } from "./odata-table"
import { message, Table } from "antd"
import useSWR from "swr"
import { OdataPage } from "./types"
import { fetchJson } from "../http/fetch"
import { ErrorBanner } from "../errors/error-banner"

export default {
  title: "Odata Table",
}

interface Invoice {
  id: string
  netSum: number
  created: string
  dueDate: string
  invoiceId: string
  duration: string
  personResponsible: string
  projects: string
  grossSum: number
  valueAddedTax: number
  client: { name: string }
}

export const UseOdataTableStory = () => {
  const { pagination } = useOdataTable()
  const { data, error } = useSWR<OdataPage<Invoice>>(
    "https://localhost:5001/odata/invoices?api-version=1.0&$count=true",
    fetchJson,
  )

  return (
    <div style={{ margin: 40 }}>
      <ErrorBanner error={error} />
      <Table<any>
        columns={[
          {
            title: "Id",
            dataIndex: "invoiceId",
            onFilter: (value, record) => {
              message.info({ message: "filter " + value })
              return true
            },
            sortDirections: ["descend"],
          },
        ]}
        dataSource={data ? data.value : []}
        pagination={{
          ...pagination,
          total: data ? data.count : 0,
          pageSizeOptions: ["10", "20", "50", "100"],
          showSizeChanger: true,
        }}
      />
    </div>
  )
}

export const OdataTableStory = () => {
  return (
    <div style={{ margin: 40 }}>
      <OdataTable<any>
        url="https://localhost:5001/odata/invoices?api-version=1.0&$count=true"
        columns={[
          {
            title: "Id",
            dataIndex: "invoiceId",
            onFilter: (value, record) => {
              message.info({ message: "filter " + value })
              return true
            },
            sortDirections: ["descend"],
          },
        ]}
        paginate={true}
        pagination={{
          pageSizeOptions: ["10", "20", "50", "100"],
          showSizeChanger: true,
        }}
      />
    </div>
  )
}
