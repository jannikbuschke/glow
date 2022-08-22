import { Box, Group, Text, Title } from "@mantine/core"
import React from "react"

export type HeaderProps = {
  title: string | React.ReactNode
  actions?: React.ReactNode | React.ReactNode[]
}

export function Header({ title, actions }: HeaderProps) {
  const t =
    typeof title === "string" ? (
      <Title order={3}>
        <Text>{title}</Text>
      </Title>
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
    <Box
      sx={() => ({
        display: "flex",
        flexDirection: "row",
        gap: 4,
      })}
    >
      <Box sx={() => ({ flex: 1 })}>{t}</Box>
      {a ? <Box>{a}</Box> : null}
    </Box>
  )
}
