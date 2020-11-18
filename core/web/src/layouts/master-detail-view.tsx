import * as React from "react"
import { Route, useNavigate, Routes } from "react-router-dom"
import { PageHeader } from "antd"
import {} from "@ant-design/icons"
import styled from "styled-components"

export function MasterDetailView({
  master,
  detail,
  path,
  create,
}: {
  path: string
  master: React.ReactElement
  detail: React.ReactElement
  create?: React.ReactElement
}) {
  return (
    <Routes>
      {create !== undefined && (
        <Route path={path + "create"} element={create} />
      )}
      <Route
        path={path + ":id"}
        element={
          <MasterDetailContainer>
            {master}
            {detail}
          </MasterDetailContainer>
        }
      />
      <Route path={path} element={master} />
    </Routes>
  )
}

const MasterDetailContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 10;
  flex: 1;
`
