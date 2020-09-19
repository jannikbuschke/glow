import * as React from "react"
import { Table, notification } from "antd"
import { TableProps, TablePaginationConfig } from "antd/lib/table"
import { useOdata } from "./useOdata"
import useSWR from "swr"
import { fetchJson } from "../http/fetch"
import { ErrorBanner } from "../errors/error-banner"
import { OdataPage } from "./types"
import { useQuery } from "react-query"

export function useOdataTable() {
  const odata = useOdata({})
  const pagination: Omit<
    TablePaginationConfig,
    "total" | "pageSizeOptions" | "showSizeChanger"
  > = {
    current: Math.floor(odata.page + 1),
    pageSize: odata.top,
    onChange: (page, size) => {
      // notification.info({
      //   message: "page " + page + " top " + page * odata.top,
      // })
      odata.setSkip((page - 1) * odata.top)
    },
    onShowSizeChange: (page, size) => {
      odata.setTop(size)
    },
  }
  return { ...odata, pagination }
}

export function OdataTable<T extends object = any>(
  props: TableProps<T> & {
    url: string
    paginate: boolean
    expand?: string
    nonOdataPayload?: boolean
  },
) {
  const { query, pagination } = useOdataTable()
  const url = props.url + query
  const { data, error } = useQuery<OdataPage<T>, string>(url, fetchJson)
  const { nonOdataPayload } = props
  const dataSource = data
    ? nonOdataPayload
      ? ((data as any) as T[])
      : data.value
    : []
  return (
    <>
      <ErrorBanner error={error} />
      <Table<T>
        {...props}
        pagination={
          props.paginate === false
            ? false
            : props.paginate && data && data.count
            ? {
                ...pagination,
                total: data.count,
              }
            : undefined
        }
        dataSource={dataSource}
      />
    </>
  )
}
