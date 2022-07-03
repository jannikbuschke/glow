import React from "react"
import { TextInput, RadioGroup, Radio, Chips, Chip } from "mantine-formik"

import {} from "glow-mantine"
import { Center, Space, Button, Text, Container } from "@mantine/core"
import { useId } from "@mantine/hooks"
import { TypedForm, useTypedQuery } from "../ts-models/api"
import { useNavigate } from "react-router"
import { showNotification } from "@mantine/notifications"
import { GameStatus, GameStatusValues } from "../ts-models/Glow.Sample"

const avatare = [
  "⛄",
  "🤴",
  "👳‍♀️",
  "👩‍🎤",
  "👨‍🎤",
  "👩‍🚀",
  "🤸‍♀️",
  "🚴‍♂️",
  "🚴‍♀️",
  "🧚‍♀️",
  "🧚‍♂️",
  "👨‍🔬",
  "👨‍🍳",
  "👨‍🔧",
  "👮‍♀️",
  "👼",
  "👲",
  "🤪",
  "🤡",
  "😸",
  "🙀",
  "🐎",
  "🦧",
  "🦏",
  "🐍",
  "🐧",
  "🐥",
  "🦉",
  "🦚",
  // "🐵",
  // "🙈",
  "😨",
  "🥶",
  "😱",
  "🤯",
  // "😵",
  "🤠",
  // "👹",
  // "👺",
  // "💀",
  // "🤖",
  // "👽",
  // "👻",
  "🦊",
  "🦄",
  // "🐤",
  // "🐧",
  // "🦇",
  // "🦋",
  // "🧛‍♀️",
  // "🧛‍♂️",
  "🧙‍♂️",
  "🧙‍♀️",
]
export function LoginView() {
  const [status, setStatus] = React.useState<null | GameStatus>(null)
  useTypedQuery("/api/ti/get-games", { input: { status }, placeholder: [] })
  const navigate = useNavigate()
  const id = useId()
  React.useEffect(() => {
    localStorage.setItem("user", "")
  }, [])
  return (
    <Center>
      <TypedForm
        actionName="/api/ti/create-player"
        initialValues={{
          name: `Jannik-${Math.floor(Math.random() * avatare.length)}`,
          icon: avatare[Math.floor(Math.random() * avatare.length)]!,
        }}
        onError={(v) => {
          showNotification({
            title: "Error",
            color: "red",
            message: v.detail || v.title,
          })
        }}
        onSuccess={(v) => {
          localStorage.setItem("user", v.id)

          navigate("/game/" + v.gameId)
        }}
      >
        {(f) => (
          <Center>
            <Container>
              <TextInput
                id={id}
                autoFocus={true}
                name="name"
                label="Name"
                size="xl"
              />
              <Space h="xl" />
              {/* <RadioGroup name="icon" label="Avatar">
              {avatare.map((v) => (
                <Radio value={v} name="icon" label={v} />
              ))}
            </RadioGroup>
            <Space h="md" /> */}
              <Text size="xl" weight={500}>
                Avatar
              </Text>
              <Chips name="icon" size="xl" variant="outline">
                {avatare.map((v) => (
                  <Chip key={v} value={v}>
                    {v}
                  </Chip>
                ))}
                {/* <Chip value="react">React</Chip>
              <Chip value="ng">Angular</Chip>
              <Chip value="svelte">Svelte</Chip>
              <Chip value="vue">Vue</Chip> */}
              </Chips>
              <Space h="xl" />

              {/* <TextInput name="icon" label="Icon" />
            <Space h="md" /> */}

              <Button onClick={() => f.submitForm()} size="xl">
                Login
              </Button>
            </Container>
          </Center>
        )}
      </TypedForm>
    </Center>
  )
}
