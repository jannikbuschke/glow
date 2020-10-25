import React from "react"
import { Space } from "antd"
import "antd/dist/antd.css"
import { FilesExample } from "./files-example"
import { BrowserRouter as Router, Link } from "react-router-dom"
import { ConfigurationsExample } from "./configuration-example"
import styled from "styled-components"
import { NavigationExample } from "./navigation"

function App() {
  return (
    <Router>
      <Container>
        <Space>
          <Link to="/portfolios/">Portfolios</Link>
          <Link to="/configurations/">Configurations</Link>
          <Link to="/navigation/">Navigation</Link>
        </Space>
        <Content>
          <FilesExample />
          <ConfigurationsExample />
          <NavigationExample />
        </Content>
        {/* <Tabs style={{ margin: 100 }}>
          <Tabs.TabPane tab="Portfolios">
          </Tabs.TabPane>
          <Tabs.TabPane tab="Configurations">
          </Tabs.TabPane>
        </Tabs> */}
      </Container>
    </Router>
  )
}

const Container = styled.div`
  display: flex;
  flex-direction: column;
  height: 100vh;
`

const Content = styled.div`
  display: flex;
  flex: 1;
  padding: 20px;
  // justify-content: center;
`

export default App
