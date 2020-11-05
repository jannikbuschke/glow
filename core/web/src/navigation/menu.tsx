import { Menu, Tabs } from "antd"
import * as React from "react"
import { useNavigate, useParams } from "react-router"
import { TabsProps } from "antd/lib/tabs"
import { MenuProps } from "antd/lib/menu"

export function RoutedTabs({
  baseUrl,
  parameter,
  children,
  ...rest
}: React.PropsWithChildren<
  { baseUrl: string; parameter: string } & Omit<
    TabsProps,
    "activeKey" | "onChange" | "defaultActiveKey"
  >
>) {
  const params = useParams()
  const view = params[parameter]
  const navigate = useNavigate()

  return (
    <Tabs
      {...rest}
      activeKey={view}
      onChange={(v) => {
        navigate(baseUrl + "/" + v)
      }}
    >
      {children}
    </Tabs>
  )
}

export function RoutedMenu({
  baseUrl,
  parameter,
  children,
  ...rest
}: React.PropsWithChildren<
  { baseUrl: string; parameter: string } & MenuProps
>) {
  const params = useParams()
  const view = params[parameter]
  const navigate = useNavigate()

  return (
    <Menu
      {...rest}
      onClick={(e) => {
        navigate(baseUrl + "/" + e.key)
      }}
      selectedKeys={[view]}
      activeKey={view}
    >
      {children}
    </Menu>
  )
}
