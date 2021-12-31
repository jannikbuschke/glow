import React from "react"
import "antd/dist/antd.css"
import { BrowserRouter as Router } from "react-router-dom"
import styled from "styled-components"
import { LeftNavigation } from "glow-react"
import { FolderOpenOutlined, EditOutlined } from "@ant-design/icons"
import { QueryClient, QueryClientProvider } from "react-query"
import { Box, ChakraProvider } from "@chakra-ui/react"
import { Routes, Route, Outlet } from "react-router"
import { DndRoutes } from "./experiments/drag-and-drop/index"

function App() {
  return (
    <Container>
      <Routes>
        <Route
          path="/*"
          element={
            <>
              <LeftNavigation
                items={[
                  {
                    key: "mdx-bundle",
                    icon: <EditOutlined />,
                    content: "MDX Bundle",
                  },
                  {
                    key: "dnd",
                    icon: <EditOutlined />,
                    content: "Drag and drop",
                  },
                ]}
              />
              <Content>
                <DndRoutes />
              </Content>
            </>
          }
        />
      </Routes>
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
