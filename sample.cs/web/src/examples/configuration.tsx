import * as React from "react"
import { ErrorBanner } from "glow-core"
import { Table } from "antd"
import { Routes, Route, useParams, Outlet, useNavigate } from "react-router-dom"
import { useData } from "glow-core/lib/query/use-data"
import { HighlightableRow } from "glow-core/lib/antd/highlightable-row"
import styled from "styled-components"
import { StronglyTypedOptions } from "glow-core/lib/configuration/strongly-typed-options"
import { Input } from "formik-antd"
import { IConfigurationMeta } from "../ts-models"

export function ConfigurationsExample() {
  return (
    <Routes>
      <Route path="configurations" element={<MasterDetail />}>
        <Route path=":configurationId" element={<Detail />} />
      </Route>
    </Routes>
  )
}

function Detail() {
  const { configurationId } = useParams()
  const url = `/api/configurations/${configurationId}`

  return (
    <div style={{ maxWidth: 500 }}>
      <div style={{ padding: 5 }}>
        <StronglyTypedOptions
          url={url}
          title="Sample (recommended)"
          configurationId={configurationId}
          overrideEditors={{
            nested: (
              <div>
                <Input name="nested.value" placeholder="Nested value" />
              </div>
            ),
          }}
        />
      </div>
      <div style={{ padding: 5 }}>
        <StronglyTypedOptions
          url={`${url}/from-options`}
          title="Sample From Options 2"
          configurationId={configurationId}
          allowEdit={false}
        />
      </div>
    </div>
  )
}

function MasterDetail() {
  return (
    <MasterDetailContainer>
      <List />
      <Outlet />
    </MasterDetailContainer>
  )
}

const MasterDetailContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 30;
  padding: 20;
`

function List() {
  const url = "/api/configuration-schemas"
  const { data, error } = useData<IConfigurationMeta[]>(url, [])
  const navigate = useNavigate()
  return (
    <div>
      <ErrorBanner error={error} />
      <Table
        dataSource={data}
        loading={!Boolean(data)}
        showHeader={false}
        size="small"
        pagination={false}
        rowKey={(row) => row.route!}
        onRow={(row) => ({
          onClick: () => {
            navigate(`${row.id}`)
          },
        })}
        components={{
          body: {
            row: (props: any) => (
              <HighlightableRow path="/configurations/" {...props} />
            ),
          },
        }}
        columns={[
          {
            dataIndex: "title",
          },
        ]}
      />
    </div>
  )
}
