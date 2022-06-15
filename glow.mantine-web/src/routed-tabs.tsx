import * as React from "react"
import { Tabs, TabsProps } from "@mantine/core"
import { useNavigate, useMatch } from "react-router"
import { TabControlProps } from "@mantine/core/lib/components/Tabs/TabControl/TabControl"

export function RoutedTabs(
  props: Omit<TabsProps, "active" | "onTabChange" | "children"> & {
    tabs: Omit<TabControlProps, "children">[]
  },
) {
  const rootMatch = useMatch(":tabKey")
  const nestedMatch = useMatch(":tabKey/*")
  const navigate = useNavigate()
  const onChange = (active: number, tabKey: string) => {
    navigate(tabKey)
  }
  const activeTab = React.useMemo(() => {
    if (rootMatch) {
      const tabKey = rootMatch.params.tabKey
      const activeTab = props.tabs.findIndex((v) => v.tabKey === tabKey)
      return activeTab
    }
    if (nestedMatch) {
      const tabKey = nestedMatch.params.tabKey
      const activeTab = props.tabs.findIndex((v) => v.tabKey === tabKey)
      return activeTab
    }
    return 0
  }, [rootMatch, nestedMatch, props.tabs])

  return (
    <>
      {/* <pre>{JSON.stringify({ nestedMatch, rootMatch }, null, 4)}</pre> */}
      <Tabs {...props} active={activeTab} onTabChange={onChange}>
        {props.tabs.map((v, i) => (
          <Tabs.Tab key={i} {...v} />
        ))}
      </Tabs>
    </>
  )
}
