import { Button, PageHeader, Tabs, Tag } from "antd"
import * as React from "react"
import { Route, Routes } from "react-router"
import { RoutedTabs, List } from "glow-react"

export function ListViewExample() {
  return (
    <Routes>
      <Route path="list-view/*" element={<Sample />} />
    </Routes>
  )
}

function Sample() {
  return (
    <div style={{ flex: 1 }}>
      <PageHeader
        title="Listview"
        extra={[<Button>Save</Button>, <Button>Delete</Button>]}
        subTitle="subtitle"
        tags={[<Tag>foo</Tag>]}
        footer={
          <RoutedTabs baseUrl="/detail-view" parameter="sub" size="large">
            <Tabs.TabPane key="key1" tab="Tab1">
              <List<any>
                baseUrl="/api/list-view/data"
                path="/list-view/"
                style={{ marginTop: 5 }}
                size="small"
                columns={[
                  {
                    title: "Name",
                    render: (row) => <span>{row.displayName}</span>,
                  },
                ]}
              />
            </Tabs.TabPane>
          </RoutedTabs>
        }
      ></PageHeader>
    </div>
  )
}
