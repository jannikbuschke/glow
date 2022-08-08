import * as React from "react"
import { Tabs, TabsProps, TabsValue } from "@mantine/core"
import { useNavigate, useMatch } from "react-router"

export type Keys = string[]

export function RoutedTabs({
  matchExpression,
  paramName,
  ...props
}: Omit<TabsProps, "value" | "onTabChange"> & {
  matchExpression: string
  paramName: string
}) {
  const rootMatch = useMatch(matchExpression)
  const navigate = useNavigate()
  const onChange = (value: TabsValue): void => {
    if (value) {
      navigate(value)
    }
  }
  const activeTab = React.useMemo(() => {
    if (rootMatch) {
      const tabKey = rootMatch.params[paramName]
      return tabKey
    }

    return undefined
  }, [rootMatch, paramName])

  return (
    <>
      <pre>
        {JSON.stringify({ rootMatch, matchExpression, paramName }, null, 4)}
      </pre>
      <Tabs {...props} value={activeTab} onTabChange={onChange} />
    </>
  )
}
