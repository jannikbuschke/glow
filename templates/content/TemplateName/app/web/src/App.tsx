import React from "react"
import { Example } from "./example"
import { QueryClient, QueryClientProvider } from "react-query"
import { BrowserRouter as Router, Link } from "react-router-dom"
import { Header } from "./header"
import { Layout } from "antd"
function App() {
  return (
    <Layout
      style={
        {
          // display: "flex", flex: 1
        }
      }
    >
      <Header />
      <div
        style={{
          padding: 48,
          // flex: 1
        }}
      >
        <Example />
      </div>
    </Layout>
  )
}

const client: QueryClient = new QueryClient({})

export function Root() {
  return (
    <Router>
      <QueryClientProvider client={client}>
        <App />
      </QueryClientProvider>
    </Router>
  )
}
