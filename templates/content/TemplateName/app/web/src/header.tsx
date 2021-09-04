import * as React from "react"
import { Layout, Menu } from "antd"
import { Link } from "react-router-dom"
import styled from "styled-components"

const { Header: AntHeader } = Layout

export function Header() {
  return (
    <Container style={{}}>
      <div style={{ margin: "0 24px" }}>
        <b>TemplateName</b>
      </div>

      <Menu mode="horizontal" style={{ flex: 1 }}>
        <Menu.Item key="4">
          <Link to={"/person"}>Persons</Link>
        </Menu.Item>
      </Menu>
    </Container>
  )
}

const Container = styled(AntHeader)`
  padding: 0 24px;
  background: white;
  display: flex;
  flex-direction: row;
  align-items: center;
  height: auto;
  box-shadow: 0 5px 8px rgba(0, 0, 0, 0.05), 0 3px 3px rgba(0, 0, 0, 0.09);
`
