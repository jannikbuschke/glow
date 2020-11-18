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
  FileOutlined,
  FolderOpenOutlined,
  FormOutlined,
  ProjectFilled,
  SettingOutlined,
  UnorderedListOutlined,
} from "@ant-design/icons"
import { DetailviewExample } from "./detail-view"
import { ListViewExample } from "./list-view"
import { FormExample } from "./form"
import { MasterDetailViewExample } from "./master-detail-view-example"

function App() {
  return (
    <Router>
      <Container>
        <LeftNavigation
          items={[
            {
              key: "portfolios",
              icon: <FolderOpenOutlined />,
              content: "Files",
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
            {
              key: "forms",
              icon: <FormOutlined />,
              content: "Form",
            },
            {
              key: "master-detail",
              icon: <FormOutlined />,
              content: "Masterdetail",
            },
          ]}
        />
        <Content>
          <FilesExample />
          <ConfigurationsExample />
          <NavigationExample />
          <DetailviewExample />
          <ListViewExample />
          <FormExample />
          <MasterDetailViewExample />
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
