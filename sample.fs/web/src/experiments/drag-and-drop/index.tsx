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
import styled from "styled-components"

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
      <div
        style={{
          background: "#ededed",
          padding: 32,
          height: 150,
          margin: "16px 0",
        }}
      >
        <Box />
      </div>
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

const Box = styled.div`
  position: relative;
  display: inline-block;
  width: 100px;
  height: 100px;
  border-radius: 5px;
  background-color: #fff;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease-in-out;
  &:hover {
    transform: scale(1.1, 1.1);

    &:after {
      // transform: scale(1.2);
      opacity: 1;
    }
  }

  &:after {
    content: "asd";
    position: absolute;
    z-index: 1;
    width: 100%;
    height: 100%;
    opacity: 0;
    border-radius: 5px;
    background-color: green;
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
    transition: opacity 0.3s ease-in-out;
  }
`
