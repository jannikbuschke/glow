import {
  Button,
  Descriptions,
  Divider,
  PageHeader,
  Row,
  Statistic,
  Tabs,
  Tag,
} from "antd"
import * as React from "react"
import { Route, Routes, useNavigate } from "react-router"
import { RoutedTabs } from "glow-react"

export function DetailviewExample() {
  return (
    <Routes>
      <Route path="detail-view/*" element={<Sample />} />
    </Routes>
  )
}

function Sample() {
  const navigate = useNavigate()
  return (
    <div style={{ flex: 1 }}>
      <PageHeader
        title="Title"
        onBack={() => navigate("..")}
        extra={[<Button>Save</Button>, <Button>Delete</Button>]}
        subTitle="subtitle"
        tags={[<Tag>foo</Tag>]}
        footer={
          <RoutedTabs baseUrl="/detail-view" parameter="sub" size="large">
            <Tabs.TabPane key="key1" tab="Tab1">
              <div>hello world</div>
            </Tabs.TabPane>
            <Tabs.TabPane key="key2" tab="Tab 2">
              <div>hello world!!!!</div>
            </Tabs.TabPane>
          </RoutedTabs>
        }
      >
        <Divider />

        <Row>
          <Statistic title="Status" value="Pending" />
          <Statistic
            title="Price"
            prefix="$"
            value={568.08}
            style={{
              margin: "0 32px",
            }}
          />
          <Statistic title="Balance" prefix="$" value={3345.08} />
        </Row>
        <Divider />
        <Descriptions size="small" column={3}>
          <Descriptions.Item label="Created">Lili Qu</Descriptions.Item>
          <Descriptions.Item label="Association">
            <div>421421</div>
          </Descriptions.Item>
          <Descriptions.Item label="Creation Time">
            2017-01-10
          </Descriptions.Item>
          <Descriptions.Item label="Effective Time">
            2017-10-10
          </Descriptions.Item>
          <Descriptions.Item label="Remarks">
            Gonghu Road, Xihu District, Hangzhou, Zhejiang, China
          </Descriptions.Item>
        </Descriptions>
        <Divider />
      </PageHeader>
    </div>
  )
}
