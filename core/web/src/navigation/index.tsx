import { Tabs } from "antd"
import * as React from "react"
import { useNavigate, useParams } from "react-router"
import { TabsProps } from "antd/lib/tabs"

export function RoutedTabs({
  baseUrl,
  parameter,
  children,
  defaultView,
  ...rest
}: React.PropsWithChildren<
  { baseUrl: string; parameter: string; defaultView?: string } & Omit<
    TabsProps,
    "activeKey" | "onChange" | "defaultActiveKey"
  >
>) {
  const params = useParams()
  const view = params[parameter] || defaultView
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

export * from "./menu"
export * from "./left-navigation"
export * from "./paths-context"
