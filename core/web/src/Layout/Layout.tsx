import { Layout, Menu, Row } from "antd"
import * as React from "react"
import styled from "styled-components"

const { Header, Sider, Content } = Layout

interface HeaderProps {
  Left?: any
  Center?: any
  Right?: any
}

interface Props {
  Header: any
  SideBar?: any
  children: any
  footer?: React.ReactNode
}

export const DefaultApplicationLayout = (props: { children: any }) => (
  <Layout style={{ minHeight: "100vh" }}>{props.children}</Layout>
)

DefaultApplicationLayout.Header = (props: { children: any }) => (
  <Header
    style={{
      height: "auto",
      display: "flex",
      justifyContent: "space-between",
    }}
  >
    {props.children}
  </Header>
)

DefaultApplicationLayout.HeaderMenu = (props: { children: any }) => (
  <Menu
    theme="dark"
    mode="horizontal"
    defaultSelectedKeys={[window.location.pathname]}
  >
    {props.children}
  </Menu>
)

export const ApplicationLayout = ({
  Header,
  SideBar,
  children,
  footer,
}: Props) => (
  <Layout style={{ minHeight: "100vh" }}>
    {Header}
    <Layout style={{ flex: 1 }}>
      {SideBar && (
        <Sider
          width={200}
          style={{
            background: "#fff",
          }}
        >
          {SideBar}
        </Sider>
      )}
      <Layout>
        <Content style={{ flex: 1 }}>{children}</Content>
        {footer && <Row>{footer}</Row>}
      </Layout>
    </Layout>
  </Layout>
)

export const Root = styled(Layout)`
  height: 100vh;
`
