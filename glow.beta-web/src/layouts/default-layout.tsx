import * as React from "react"
import { Layout, Menu, Row } from "antd"
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
    // defaultSelectedKeys={[window.location.pathname]}
  >
    {props.children}
  </Menu>
)

export function DefaultHeader(props: { children: any }) {
  return (
    <div>
      <Menu mode="horizontal" defaultSelectedKeys={[window.location.pathname]}>
        {props.children}
      </Menu>
    </div>
  )
}

const MyHeader = styled.div`
  grid-column: 2/3;
`

const SideBarContainer = styled.div`
  grid-row: 1/3;
  //   background: #001529;
  background: white;
  display: flex;
  flex-direction: column;
`

const RootLayout = styled(Layout)`
  display: grid;
  height: 100vh;
  grid-template-columns: auto 1fr;
  grid-template-rows: auto 1fr;
`

const Logo = styled.div`
  width: 100%;
  height: 120px;
  padding: 16px;
`

export const ApplicationLayout = ({
  Header,
  SideBar,
  children,
  footer,
}: Props) => (
  <RootLayout>
    <MyHeader>{Header}</MyHeader>
    {SideBar && (
      <SideBarContainer>
        <Logo>
          <div
            style={{
              background: "rgba(255, 255, 255, 0.2)",
              height: "100%",
              width: "100%",
            }}
          />
        </Logo>
        <Sider width={200} style={{ width: 200 }} theme="light">
          {SideBar}
        </Sider>
      </SideBarContainer>
    )}
    <Layout style={{ flex: 1 }}>
      <Layout>
        <Content>
          <div style={{ flex: 1 }}>{children}</div>
        </Content>
        {footer && <Row>{footer}</Row>}
      </Layout>
    </Layout>
  </RootLayout>
)
