import { Menu, Tabs } from "antd"
import * as React from "react"
import { Route, Routes, useNavigate, useParams, useMatch } from "react-router"
import { RoutedTabs, RoutedMenu, RenderObject } from "glow-react"

export function NavigationExample() {
  return (
    <Routes>
      <Route path="navigation/" element={<Sample />} />
      <Route path="navigation/:view/*" element={<Sample />} />
      <Route path="navigation/:view" element={<Sample />}>
        {/* <Route path=":navigationId" element={<Detail />} /> */}
      </Route>
    </Routes>
  )
}

function Sample() {
  return (
    <div>
      <RoutedTabsSample />
      <RoutedMenuSample />
    </div>
  )
}

function RoutedTabsSample() {
  const match1 = useMatch("navigation/:view")
  const match2 = useMatch("navigation/:view/sub")
  const match3 = useMatch("navigation/:view/:id")
  return (
    <div>
      <RenderObject {...{ match1, match2, match3 }} />
      <RoutedTabs baseUrl="/navigation" parameter="view">
        <Tabs.TabPane key="view1" tab="tab1">
          <div>tab1</div>
        </Tabs.TabPane>
        <Tabs.TabPane key="view2" tab="tab2">
          <div>tab2</div>
        </Tabs.TabPane>
        <Tabs.TabPane key="view3" tab="tab3">
          <div>tab3</div>
        </Tabs.TabPane>
        <Tabs.TabPane key="view4" tab="tab4">
          <div>tab4</div>
        </Tabs.TabPane>
      </RoutedTabs>
    </div>
  )
}

function RoutedMenuSample() {
  return (
    <div>
      <RoutedMenu baseUrl="/navigation" parameter="view" mode="inline">
        <Menu.Item key="view1">item 1</Menu.Item>
        <Menu.Item key="view2">item 2</Menu.Item>
        <Menu.Item key="view3">item 3</Menu.Item>
        <Menu.SubMenu title="submenu">
          <Menu.Item key="view4">item 4</Menu.Item>
        </Menu.SubMenu>
      </RoutedMenu>
    </div>
  )
}
