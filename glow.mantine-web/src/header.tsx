import { Box, Group, Text, Title } from "@mantine/core"
import React from "react"

export type HeaderProps = {
  title: string | React.ReactNode
  actions?: React.ReactNode | React.ReactNode[]
}

export function Header({ title, actions }: HeaderProps) {
  const t =
    typeof title === "string" ? (
      <Text size="xl" color="dimmed" weight={700}>
        {title}
      </Text>
    ) : (
      title
    )
  const a =
    typeof actions === "object" ? (
      Array.isArray(actions) ? (
        <Group>{actions}</Group>
      ) : (
        actions
      )
    ) : null
  return (
    <Box sx={() => ({ display: "flex", flexDirection: "row" })}>
      <Box sx={() => ({ flex: 1 })}>{t}</Box>
      {a ? <Box>{a}</Box> : null}
    </Box>
  )
}
// export const Header = styled(PageHeader)`
//   padding: 0px;
//   .ant-page-header-heading {
//     justify-items: flex-start;
//   }
// `
