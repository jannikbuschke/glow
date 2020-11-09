import React from "react"
import "antd/dist/antd.css"
import { FilesExample } from "./examples/files"
import { BrowserRouter as Router } from "react-router-dom"
import { ConfigurationsExample } from "./examples/configuration"
import styled from "styled-components"
import { NavigationExample } from "./examples/navigation"
import { LeftNavigation } from "glow-react"
import {
  BorderOutlined,
  ClusterOutlined,
  FolderOpenOutlined,
  FormOutlined,
  SettingOutlined,
  UnorderedListOutlined,
  EditOutlined,
} from "@ant-design/icons"
import { MasterDetailViewExample } from "./examples/master-detail-view-example"
import { SelectAsyncExample } from "./examples/select-async-example"
import { DetailviewExample } from "./examples/detail-view"
import { ListViewExample } from "./examples/list-view"
import { FormExample } from "./examples/form"

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
            {
              key: "select-async",
              icon: <EditOutlined />,
              content: "Select Async",
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
          <SelectAsyncExample />
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
