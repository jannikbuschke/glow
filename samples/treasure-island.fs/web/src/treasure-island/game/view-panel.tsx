import { ActionIcon, Grid } from "@mantine/core"
import {
  ArrowUp,
  ArrowDown,
  ArrowLeft,
  ArrowRight,
  ZoomIn,
  ZoomOut,
  Plus,
  Minus,
} from "tabler-icons-react"

import { Center, Paper } from "@mantine/core"

export function ViewPanel({
  setViewbox0,
  setViewbox1,
  setViewbox2,
  setViewbox3,
  viewbox0,
  viewbox1,
  viewbox2,
  viewbox3,
  xy,
  setXY,
  setSpacing,
  spacing,
}: {
  viewbox0: number
  viewbox1: number
  viewbox2: number
  viewbox3: number
  xy: number
  spacing: number
  setViewbox0: React.Dispatch<React.SetStateAction<number>>
  setViewbox1: React.Dispatch<React.SetStateAction<number>>
  setViewbox2: React.Dispatch<React.SetStateAction<number>>
  setViewbox3: React.Dispatch<React.SetStateAction<number>>
  setSpacing: React.Dispatch<React.SetStateAction<number>>
  setXY: React.Dispatch<React.SetStateAction<number>>
}) {
  return (
    <Paper
      shadow="xs"
      withBorder={true}
      p="xs"
      style={{ background: "var(--mantine-color-dark-2)" }}
    >
      <Grid>
        <Grid.Col span={4}>
          <Center>
            <ActionIcon onClick={() => setViewbox0(viewbox0 - 10)}>
              <ArrowLeft />
            </ActionIcon>
          </Center>
        </Grid.Col>
        <Grid.Col span={4}>
          <Center>
            <ActionIcon onClick={() => setViewbox1(viewbox1 + 10)}>
              <ArrowDown />
            </ActionIcon>
            <ActionIcon onClick={() => setViewbox1(viewbox1 - 10)}>
              <ArrowUp />
            </ActionIcon>
          </Center>
        </Grid.Col>
        <Grid.Col span={4}>
          <Center>
            <ActionIcon onClick={() => setViewbox0(viewbox0 + 10)}>
              <ArrowRight />
            </ActionIcon>
          </Center>
        </Grid.Col>
      </Grid>
      <Grid>
        <Grid.Col span={4}>
          <Center>Size</Center>
          <Center>
            <ActionIcon
              onClick={() => {
                setXY(xy - 0.1)
              }}
            >
              <Minus />
            </ActionIcon>
            <ActionIcon
              onClick={() => {
                setXY(xy + 0.1)
              }}
            >
              <Plus />
            </ActionIcon>
          </Center>
        </Grid.Col>
        <Grid.Col span={4}>
          <Center>Zoom</Center>
          <Center>
            <ActionIcon
              onClick={() => {
                setViewbox3(viewbox3 + 10)
                setViewbox1(viewbox1 - 5)
              }}
            >
              <ZoomOut />
            </ActionIcon>
            <ActionIcon
              onClick={() => {
                setViewbox3(viewbox3 - 10)
                setViewbox1(viewbox1 + 5)
              }}
            >
              <ZoomIn />
            </ActionIcon>
          </Center>
        </Grid.Col>
        <Grid.Col span={4}>
          <Center>Abstand</Center>
          <Center>
            <ActionIcon
              onClick={() => {
                setSpacing(spacing + 0.01)
              }}
            >
              <Minus />
            </ActionIcon>
            <ActionIcon
              onClick={() => {
                setSpacing(spacing - 0.01)
              }}
            >
              <Plus />
            </ActionIcon>
          </Center>
        </Grid.Col>
      </Grid>
    </Paper>
  )
}
