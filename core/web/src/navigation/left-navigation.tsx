import * as React from "react"
import { RoutedMenu } from "./menu"
import { Layout, Menu } from "antd"
import { Route, Routes } from "react-router-dom"
import { Link } from "react-router-dom"

interface LeftNavigationProps {
  items: { key: string; content: React.ReactNode; icon: React.ReactNode }[]
}

const { Sider } = Layout

export function LeftNavigation({ items }: LeftNavigationProps) {
  return (
    <Routes>
      <Route path={""} element={<_LeftNavigation items={items} />} />
      <Route path={"/"} element={<_LeftNavigation items={items} />} />
      <Route path={":view/*"} element={<_LeftNavigation items={items} />} />
    </Routes>
  )
}

export function _LeftNavigation({ items }: LeftNavigationProps) {
  const [collapsed, setCollapsed] = React.useState(false)
  return (
    <Sider
      collapsible={true}
      collapsed={collapsed}
      onCollapse={(v) => setCollapsed(v)}
    >
      <RoutedMenu
        title="test"
        baseUrl={"/"}
        parameter={"view"}
        theme="dark"
        mode="inline"
        // style={{ width: 200 }}
      >
        {items.map((v) => (
          <Menu.Item key={v.key} icon={v.icon}>
            <Link to={`/${v.key}`}>{v.content}</Link>
          </Menu.Item>
        ))}
      </RoutedMenu>
    </Sider>
  )
}
