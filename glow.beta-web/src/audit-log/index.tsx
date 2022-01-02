import { Modal, Spin, Table, Timeline } from "antd"
import * as React from "react"
import { ErrorBanner } from "glow-core/es/errors/error-banner"
import { useData } from "../query/use-data"
import dayjs from "dayjs"
import { RenderObject } from "glow-core/es/debugging"

export type AuditItem = {
  id: string
  createdBy: string
  createdByDisplayName: string
  createdAt: string
  entityId: string
  entityDisplayName: string
  entityType: string
  operationType: string
  changes: Change[]
}

export type Change = {
  fullTypeName: string
  key: string
  state: "Added" | "Updated" | "Deleted"
  displayName: string | null
  changedProperties: ChangedProperty[]
}
export type ChangedProperty = {
  newValue: string | null
  originalValue: string | null
  propertyName: string
}

export function useAuditLog(url: string) {
  const value = useData<AuditItem[]>(url, [])
  return value
}

export function AuditTable({ url }: { url: string }) {
  const { data, loading, error } = useAuditLog(url)
  return (
    <div>
      <ErrorBanner error={error} />
      <Table<AuditItem>
        size="small"
        pagination={false}
        loading={loading}
        dataSource={data}
        rowKey={(row) => row.id}
        expandable={{
          expandedRowRender: (record) => (
            <div>
              <RenderObject {...record.changes} />
            </div>
          ),
        }}
        columns={[
          {
            title: "Id",
            render: (v, record) => <div>{record.id}</div>,
          },
          {
            title: "Created by",
            render: (v, record) => <div>{record.createdByDisplayName}</div>,
          },
          {
            title: "Time",
            render: (v, record) => (
              <div>{dayjs(record.createdAt).format("L LT")}</div>
            ),
          },
          {
            title: "Operation",
            render: (v, record) => <div>{record.operationType}</div>,
          },
        ]}
      />
    </div>
  )
}

export function AuditTimeline({ url }: { url: string }) {
  const { data, loading, error } = useAuditLog(url)
  // const { translate } = useTranslation()
  return (
    <Spin spinning={loading} delay={500}>
      <ErrorBanner error={error} />
      <Timeline mode="left">
        {data.map((v) => (
          <Timeline.Item
            key={v.id}
            label={
              <div
                onClick={() => {
                  Modal.info({
                    content: (
                      <div>
                        {/* <pre>{JSON.stringify(v.changes, null, 1)}</pre> */}
                        {v.changes.map((v) => (
                          <div>
                            <div>
                              {v.displayName || v.fullTypeName} ({v.key})
                            </div>
                            <div>
                              {v.changedProperties.map((v) => (
                                <div>
                                  {v.propertyName}: {v.originalValue} {"=> "}
                                  {v.newValue}
                                </div>
                              ))}
                            </div>
                          </div>
                        ))}
                      </div>
                    ),
                  })
                }}
              >
                <div>{dayjs(v.createdAt).format("L")}</div>
                <div>{v.createdByDisplayName}</div>
              </div>
            }
          >
            {v.operationType}
          </Timeline.Item>
        ))}
      </Timeline>
    </Spin>
  )
}
