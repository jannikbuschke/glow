import * as React from "react"
import { useData, ErrorBanner } from "glow-react"
import { Table } from "antd"
import { HighlightableRow } from "gertrud-components"
import { useTranslation } from "react-i18next"
import { useNavigate } from "react-router"
import styled from "styled-components"

interface $1ListDto {
  id: string
}

export function $1ListView() {
  const url = "/api/$1"
  const { data, loading, error } = useData<$1ListDto[]>(url)
  const { t } = useTranslation()
  const navigate = useNavigate()
  return (
    <Container>
      <ErrorBanner error={error} />
      <Table<$1ListDto>
        loading={loading}
        bordered={false}
        style={{ flex: 1 }}
        onRow={(record) => ({
          onClick: () => navigate(`/my-topics/${record.id}`),
        })}
        rowKey={(row) => row.id}
        components={{
          body: {
            row: (props: any) => <HighlightableRow path="/$1/" {...props} />,
          },
        }}
        columns={[
          {
            title: t("id"),
            key: "id",
            render: (record: $1ListDto) => <span>{record.id}</span>,
          },
        ]}
        dataSource={data ? data : []}
        pagination={
          data && data.length > 10
            ? {
                pageSize: 10,
              }
            : false
        }
      />
    </Container>
  )
}

const Container = styled.div``
