import { Tabs } from "antd"
import * as React from "react"
import { Route, Routes, useNavigate, useParams, useMatch } from "react-router"
import { RoutedTabs, RenderObject } from "glow-react"

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
  const match1 = useMatch("navigation/:view")
  const match2 = useMatch("navigation/:view/sub")
  const match3 = useMatch("navigation/:view/:id")
  return (
    <div>
      <RenderObject {...{ match1, match2, match3 }} />
      <RoutedTabs baseUrl="/navigation" parameter="view">
        <Tabs.TabPane key="tab1" tab="tab1">
          <div>tab1</div>
        </Tabs.TabPane>
        <Tabs.TabPane key="tab2" tab="tab2">
          <div>tab2</div>
        </Tabs.TabPane>
        <Tabs.TabPane key="tab3" tab="tab3">
          <div>tab3</div>
        </Tabs.TabPane>
      </RoutedTabs>
    </div>
  )
}
