import React from "react"
import { Space } from "antd"
import "antd/dist/antd.css"
import { FilesExample } from "./files-example"
import { BrowserRouter as Router, Link } from "react-router-dom"
import { ConfigurationsExample } from "./configuration-example"
import styled from "styled-components"

function App() {
  return (
    <Router>
      <Container>
        <Space>
          <Link to="/portfolios/">Portfolios</Link>
          <Link to="/configurations/">Configurations</Link>
        </Space>
        <Content>
          <FilesExample />
          <ConfigurationsExample />
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
  justify-content: center;
`

export default App
