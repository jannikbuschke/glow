import React from "react"
import { TextInput, RadioGroup, Radio, ChipGroup } from "formik-mantine"
import {
  Center,
  Space,
  Button,
  Text,
  Container,
  Box,
  Chip,
} from "@mantine/core"
import { useId } from "@mantine/hooks"
import { useNavigate } from "react-router"
import { showNotification } from "@mantine/notifications"
import {} from "glow-core/lib/"
import {
  CustomTable,
  usePagination,
} from "glow-beta/lib/list/mantine-nav-table"
import { css } from "@emotion/react"
import { TypedForm, useTypedQuery } from "../client/api"
import { Game, GameStatus } from "../client/TreasureIsland"
import { defaultGuid } from "../client/System"
import { Query } from "../query"

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
  const { data } = useTypedQuery("/api/ti/get-games", {
    input: { status: { Case: "Initializing" } },
    placeholder: [],
  })
  const navigate = useNavigate()
  const id = useId()
  React.useEffect(() => {
    localStorage.setItem("user", "")
  }, [])
  const [selectedGame, setSelectedGame] = React.useState<null | Game>(null)
  const initalRequest = React.useMemo(
    () => ({
      name: `Player-${Math.floor(Math.random() * avatare.length)}`,
      icon: avatare[Math.floor(Math.random() * avatare.length)]!,
    }),
    [],
  )
  return (
    <Center>
      <Box
        css={css`
          display: grid;
          /* grid-template-columns: auto 1fr; */
        `}
      >
        <Query
          name="/api/ti/get-games"
          input={{ status: { Case: "Initializing" } }}
          placeholder={[]}
        >
          {(data) => (
            <CustomTable
              dataSource={data}
              selected={selectedGame || undefined}
              paginate={false}
              onRowClick={(r) => setSelectedGame(r)}
              columns={[
                {
                  key: "id",
                  title: "ID",
                  render: (e) => e.id,
                },
                {
                  key: "status",
                  title: "Status",
                  render: (e) => e.status.Case,
                },
              ]}
            />
          )}
        </Query>
        <Space my="xl" />
        <TypedForm
          actionName="/api/ti/create-player"
          initialValues={{
            gameId: selectedGame?.id || defaultGuid,
            ...initalRequest,
          }}
          onError={(v) => {
            showNotification({
              title: "Error",
              color: "red",
              message: v.detail || v.title,
            })
          }}
          onSuccess={(v) => {
            console.log({ result: v })
            localStorage.setItem("user", v.playerId)
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
                <ChipGroup name="icon">
                  {avatare.map((v) => (
                    <Chip key={v} value={v}>
                      {v}
                    </Chip>
                  ))}
                  {/* <Chip value="react">React</Chip>
              <Chip value="ng">Angular</Chip>
              <Chip value="svelte">Svelte</Chip>
              <Chip value="vue">Vue</Chip> */}
                </ChipGroup>
                <Space h="xl" />

                {/* <TextInput name="icon" label="Icon" />
            <Space h="md" /> */}

                <Center>
                  <Button
                    disabled={selectedGame === null}
                    onClick={() => f.submitForm()}
                    size="xl"
                  >
                    Login
                  </Button>
                </Center>
              </Container>
            </Center>
          )}
        </TypedForm>
      </Box>
    </Center>
  )
}
