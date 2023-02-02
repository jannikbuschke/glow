import {
  ThemeIcon,
  UnstyledButton,
  Group,
  Text,
  MantineSize,
  DefaultMantineColor,
} from "@mantine/core"
import { Link, useMatch, useNavigate } from "react-router-dom"

export type MainLinkProps = {
  icon?: React.ReactNode
  color?: DefaultMantineColor
  label: string
  to: string
  size?: MantineSize
  padding?: number
  borderRightSize?: number
  activeLinkIfExactMatch?: boolean
}

export function MainLinks({
  data,
  size = "xs",
  padding = 1,
  iconColor,
  borderRightSize = 4,
}: {
  data: MainLinkProps[]
  size?: MantineSize
  padding?: number
  iconColor?: DefaultMantineColor
  borderRightSize?: number
}) {
  const links = data.map((link) => (
    <MainLink
      {...link}
      color={iconColor}
      size={size}
      padding={padding}
      key={link.label}
      borderRightSize={borderRightSize}
    />
  ))
  return <div>{links}</div>
}

export function MainLink({
  icon,
  color,
  label,
  to,
  size,
  padding,
  borderRightSize,
  activeLinkIfExactMatch,
}: MainLinkProps) {
  const matchPattern = to.endsWith("*")
    ? to
    : to.endsWith("/")
    ? `${to}*`
    : `${to}/*`
  const match = useMatch(matchPattern)
  const matched = activeLinkIfExactMatch
    ? match && match.pathname === to
    : Boolean(match)
  return (
    <UnstyledButton
      component={Link}
      to={to}
      sx={(theme) => ({
        display: "block",
        width: "100%",
        padding: padding,
        borderRadius: theme.radius.xs,
        color:
          theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.black,
        backgroundColor: matched
          ? theme.colorScheme === "dark"
            ? theme.colors.dark[5]
            : theme.colors.blue[1]
          : undefined,
        borderRight: matched
          ? `${borderRightSize}px solid ${theme.colors.blue[5]}`
          : `${borderRightSize}px solid ${
              theme.colorScheme === "dark"
                ? theme.colors.dark[6]
                : theme.colors.gray[2]
            }`,
        "&:hover": {
          backgroundColor:
            theme.colorScheme === "dark"
              ? theme.colors.dark[6]
              : theme.colors.gray[2],
        },
      })}
    >
      <Group>
        <ThemeIcon color={color} variant="light" size={size}>
          {icon}
        </ThemeIcon>
        <Text size={size}>{label}</Text>
      </Group>
    </UnstyledButton>
  )
}
