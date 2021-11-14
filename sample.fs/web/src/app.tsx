import React from "react"
import "antd/dist/antd.css"
import { FilesExample } from "./experiments/files"
import { BrowserRouter as Router } from "react-router-dom"
import { ConfigurationsExample } from "./examples/configuration"
import styled from "styled-components"
import { NavigationExample } from "./experiments/navigation/navigation"
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
import { MasterDetailViewExample } from "./experiments/maste-detail-view"
import { SelectAsyncExample } from "./experiments/select-async"
import { DetailviewExample } from "./experiments/detail-view"
import { ListViewExample } from "./experiments/list-view"
import { FormExample } from "./examples/form"
import { QueryClient, QueryClientProvider } from "react-query"
import { MdxBundleExample } from "./experiments/mdx-bundle"
import { AzureDevopsExample } from "./experiments/azdo/variable-group"
import { ChakraProvider } from "@chakra-ui/react"

function App() {
  return (
    <Container>
      <LeftNavigation
        items={[
          {
            key: "configurations",
            icon: <SettingOutlined />,
            content: "Configurations",
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
            key: "portfolios",
            icon: <FolderOpenOutlined />,
            content: "Files",
          },
          {
            key: "azdo",
            icon: <FolderOpenOutlined />,
            content: "Azure devops",
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
            key: "select-async",
            icon: <EditOutlined />,
            content: "Select Async",
          },
          {
            key: "mdx-bundle",
            icon: <EditOutlined />,
            content: "MDX Bundle",
          },
        ]}
      />
      <Content>
        <FilesExample />
        <ConfigurationsExample />
        <AzureDevopsExample />
        <NavigationExample />
        <DetailviewExample />
        <ListViewExample />
        <FormExample />
        <MasterDetailViewExample />
        <SelectAsyncExample />
        <MdxBundleExample />
      </Content>
    </Container>
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
const client = new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
    },
  },
})

export function Root() {
  return (
    <ChakraProvider>
      <QueryClientProvider client={client}>
        <Router>
          <App />
        </Router>
      </QueryClientProvider>
    </ChakraProvider>
  )
}
