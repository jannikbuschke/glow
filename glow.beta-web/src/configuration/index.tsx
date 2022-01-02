import * as React from "react"
import { Route, useNavigate, Routes } from "react-router-dom"
import { Button, PageHeader } from "antd"
// import { Card } from "../antd/styled-components"
import {} from "@ant-design/icons"
import styled from "styled-components"
import {} from "@ant-design/icons"
import { AllConfigurationsListView } from "./list"
import { constants } from "./constants"
import { AllConfigurationsDetailView } from "./detail"

const { paths } = constants

export function AllConfigurationMasterDetailView() {
  return (
    <Routes>
      <Route
        path={paths.id}
        element={
          <Container>
            <Header
              title="Configuration"
              extra={[]}
              style={{ paddingLeft: 0, paddingTop: 0, marginTop: 0 }}
            />
            <MasterDetailContainer>
              <AllConfigurationsListView />
              <AllConfigurationsDetailView />
            </MasterDetailContainer>
          </Container>
        }
      />
      <Route
        path={paths.list}
        element={
          <Container>
            <Header
              title="Configuration"
              extra={[]}
              style={{ paddingLeft: 0, paddingTop: 0, marginTop: 0 }}
            />
            <AllConfigurationsListView />
          </Container>
        }
      />
    </Routes>
  )
}

const Header = styled(PageHeader)``

const MasterDetailContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 10;
  flex: 1;
`

const Container = styled.div`
  padding: 50px;
`
