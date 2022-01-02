import {
  Button,
  Input,
  NumberDecrementStepper,
  NumberIncrementStepper,
  NumberInput,
  NumberInputField,
  NumberInputStepper,
  Stack,
} from "@chakra-ui/react"
import * as React from "react"
import { Routes, Route } from "react-router"
import { createDeflateRaw } from "zlib"
import { useTypedAction, useTypedQuery } from "../../ts-models/api"
import { defaultMeeting } from "../../ts-models/Sample.Fs.Agenda"
import { Item } from "./item/Item"
import { MultipleContainers } from "./multiple-containers"
import { AgendaView } from "./agenda-view"

export function DndRoutes() {
  return (
    <Routes>
      <Route path="dnd" element={<Example />} />
    </Routes>
  )
}

export function Example() {
  const [createMeeting] = useTypedAction("/api/meeting/create")
  const [upsertMeetingItem] = useTypedAction("/api/upsert-meeting-item")
  const [meetingId, setMeetingId] = React.useState("")
  const [version, setVersion] = React.useState<number>(0)
  const { data: meeting, isFetched, dataUpdatedAt } = useTypedQuery(
    "/api/get-meeting",
    {
      input: { meetingId, version },
      placeholder: defaultMeeting,
      queryOptions: { enabled: Boolean(meetingId) },
    },
  )
  const [creating, setCreating] = React.useState(false)
  async function createData() {
    setCreating(true)
    const result = await createMeeting({})
    if (result.ok) {
      const meeting = result.payload
      for (let index = 0; index < 9; index++) {
        const meetingItem0 = await upsertMeetingItem({
          meetingId: meeting.id,
          meetingItem: {
            displayName: `TOP ${index + 1}`,
            duration: 0,
            id: `00000000-0000-0000-0000-00000000000${index + 1}`,
          },
        })
      }
      setMeetingId(meeting.id)
    }
    setCreating(false)
  }

  return (
    <div>
      <Stack direction="row" spacing={2}>
        <Button
          size="md"
          isLoading={creating}
          onClick={async () => {
            await createData()
          }}
        >
          create data
        </Button>
        <NumberInput
          value={"" + version}
          onChange={(v) => {
            const version = parseInt(v)
            setVersion(parseInt(v))
          }}
        >
          <NumberInputField />
          <NumberInputStepper>
            <NumberIncrementStepper />
            <NumberDecrementStepper />
          </NumberInputStepper>
        </NumberInput>
      </Stack>
      <div key={dataUpdatedAt}>
        {/* is fetched: {JSON.stringify(isFetched)} */}
        {/* <MultipleContainers vertical={true} /> */}
        {/* {isFetched && ( */}
        <AgendaView
          meetingId={meetingId}
          vertical={true}
          items={{ A: meeting.items }}
        />
        {/* )} */}
      </div>
      {/* <RenderObject {...{ meeting, meetingId }} /> */}
    </div>
  )
}
