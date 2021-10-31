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
  masteDetailContainerStyle,
}: {
  path: string
  master: React.ReactElement
  detail: React.ReactElement
  create?: React.ReactElement
  masteDetailContainerStyle?: React.CSSProperties
}) {
  return (
    <Routes>
      {create !== undefined && (
        <Route path={path + "create"} element={create} />
      )}
      <Route
        path={path + ":id"}
        element={
          <MasterDetailContainer style={masteDetailContainerStyle}>
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
  gap: 10px;
  grid-gap: 10xp;
  flex: 1;
`
