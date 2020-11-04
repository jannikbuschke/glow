import React from "react"
import { Layout } from "antd"
import "antd/dist/antd.css"
import { FilesExample } from "./files-example"
import { BrowserRouter as Router } from "react-router-dom"
import { ConfigurationsExample } from "./configuration-example"
import styled from "styled-components"
import { NavigationExample } from "./navigation"
import { LeftNavigation } from "glow-react"
import {
  BorderOutlined,
  ClusterOutlined,
  ProjectFilled,
  SettingOutlined,
  UnorderedListOutlined,
} from "@ant-design/icons"
import { DetailviewExample } from "./detail-view"
import { ListViewExample } from "./list-view"

function App() {
  return (
    <Router>
      <Container>
        <LeftNavigation
          items={[
            {
              key: "portfolios",
              icon: <ProjectFilled />,
              content: "Portfolios",
            },
            {
              key: "configurations",
              icon: <SettingOutlined />,
              content: "Configurations",
            },
            {
              key: "navigation",
              icon: <ClusterOutlined />,
              content: "Navigation",
            },
            {
              key: "detail-view",
              icon: <BorderOutlined />,
              content: "Detailview",
            },
            {
              key: "list-view",
              icon: <UnorderedListOutlined />,
              content: "Listview",
            },
          ]}
        />
        <Content>
          <FilesExample />
          <ConfigurationsExample />
          <NavigationExample />
          <DetailviewExample />
          <ListViewExample />
        </Content>
      </Container>
    </Router>
  )
}

const Container = styled.div`
  display: flex;
  // flex-direction: column;
  height: 100vh;
`

const Content = styled.div`
  display: flex;
  flex: 1;
  padding: 20px;
  // justify-content: center;
`

export default App
