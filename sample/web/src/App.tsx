import React from "react"
import logo from "./logo.svg"
import "./App.css"
import { Tabs } from "antd"
import { ApplicationLayout } from "glow-react/es/Layout/Layout"
import "antd/dist/antd.css"
import { FilesExample } from "./files-example"
import { BrowserRouter as Router, Link } from "react-router-dom"

function App() {
  return (
    <Router>
      <ApplicationLayout Header={null}>
        <Link to="/portfolios/">Portfolios</Link>
        <Tabs style={{ margin: 100 }}>
          <Tabs.TabPane tab="Portfolios">
            <FilesExample />
          </Tabs.TabPane>
        </Tabs>
      </ApplicationLayout>
    </Router>
  )
}

export default App
