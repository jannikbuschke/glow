import {
  ThemeIcon,
  UnstyledButton,
  Group,
  Text,
  MantineSize,
} from "@mantine/core"
import { useMatch, useNavigate } from "react-router-dom"

export type MainLinkProps = {
  icon?: React.ReactNode
  color: string
  label: string
  to: string
  size: MantineSize
  padding?: number
}

export function MainLinks({
  data,
  size = "xs",
  padding = 1,
}: {
  data: MainLinkProps[]
  size?: MantineSize
  padding?: number
}) {
  const links = data.map((link) => (
    <MainLink {...link} key={link.label} size={size} padding={padding} />
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
}: MainLinkProps) {
  const matchPattern = to.endsWith("*")
    ? to
    : to.endsWith("/")
    ? `${to}*`
    : `${to}/*`
  const match = useMatch(matchPattern)
  const navigate = useNavigate()
  return (
    <UnstyledButton
      onClick={() => navigate(to)}
      sx={(theme) => ({
        display: "block",
        width: "100%",
        padding: padding,
        borderRadius: theme.radius.xs,
        color:
          theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.black,
        backgroundColor: match !== null ? theme.colors.blue[1] : undefined,
        borderRight:
          match !== null
            ? `2px solid ${theme.colors.blue[5]}`
            : `2px solid ${
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
