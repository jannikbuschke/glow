import * as React from "react"
import { useData } from "../query/use-data"
import { ErrorBanner } from "glow-core"
import { Tabs, Table } from "antd"
// import { Table } from "../antd/styled-components"
import { useNavigate } from "react-router"
import styled from "styled-components"
import { HighlightableRow } from "../antd/highlightable-row"
import { constants } from "./constants"
// import "../utils"
import dayjs from "dayjs"

// var localizedFormat = require("dayjs/plugin/localizedFormat")
// dayjs.extend(localizedFormat)

interface AllConfigurationsListDto {
  id: string
  version: number
  name: string
  created: string
}

export function AllConfigurationsListView() {
  return (
    <Tabs>
      <Tabs.TabPane key="latest" tab="Latest" style={{}}>
        <List url={constants.api.list} showVersion={false} />
      </Tabs.TabPane>
      <Tabs.TabPane key="all" tab="All" style={{}}>
        <List url={constants.api.all} showVersion={true} />
      </Tabs.TabPane>
    </Tabs>
  )
}

function List({ url, showVersion }: { url: string; showVersion: boolean }) {
  const { data, loading, error } = useData<AllConfigurationsListDto[]>(url, [])
  const navigate = useNavigate()
  return (
    <>
      <ErrorBanner error={error} />
      <Table<AllConfigurationsListDto>
        loading={loading}
        size="small"
        bordered={false}
        style={{ flex: 1 }}
        onRow={(row) => ({
          onClick: () =>
            navigate(
              constants.paths.single(
                encodeURI(
                  JSON.stringify({
                    id: row.id,
                    name: row.name,
                    version: row.version,
                  }),
                ),
              ),
            ),
        })}
        rowKey={(row) =>
          encodeURI(
            JSON.stringify({
              id: row.id,
              name: row.name,
              version: row.version,
            }),
          )
        }
        components={{
          body: {
            row: (props: any) => (
              <HighlightableRow path={constants.paths.list} {...props} />
            ),
          },
        }}
        columns={[
          {
            title: "Created",
            key: "created",
            width: 210,
            render: (record: AllConfigurationsListDto) => (
              <span>{dayjs(record.created).format("lll")}</span>
            ),
          },
          {
            title: "Id",
            key: "id",
            width: "1fr",
            render: (record: AllConfigurationsListDto) => (
              <span>{record.id}</span>
            ),
          },
          {
            title: "Name",
            key: "name",
            width: "1fr",
            render: (record: AllConfigurationsListDto) => (
              <span>{record.name !== null ? record.name : <i>N/A</i>}</span>
            ),
          },
          ...(showVersion
            ? [
                {
                  title: "Version",
                  key: "version",
                  width: 150,
                  render: (record: AllConfigurationsListDto) => (
                    <span>{record.version}</span>
                  ),
                },
              ]
            : []),
        ]}
        dataSource={data ? data : []}
        pagination={false}
      />
    </>
  )
}

const Pre = styled.pre`
  white-space: pre-wrap;
  word-wrap: break-word;
`

const Container = styled.div`
  padding: 50px;
`
