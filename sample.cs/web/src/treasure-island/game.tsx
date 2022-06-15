import { notifyError, RenderObject, useNotification } from "glow-core"
import React from "react"
import {
  HexGrid,
  Layout,
  Hexagon,
  Text,
  Pattern,
  Path,
  Hex,
  GridGenerator,
  HexUtils,
} from "react-hexgrid"
import "./game.css"
import styled from "styled-components"
import {
  MANTINE_COLORS,
  useMantineColorScheme,
  useMantineTheme,
} from "@mantine/styles"

import { ActionIcon, Grid, Space } from "@mantine/core"
import {
  Adjustments,
  ArrowUp,
  ArrowDown,
  ArrowLeft,
  ArrowRight,
  ZoomIn,
  ZoomOut,
  X,
  Plus,
  Minus,
  AxisY,
} from "tabler-icons-react"

import { showNotification } from "@mantine/notifications"
import { useTypedAction } from "../ts-models/api"
import { HexagonMouseEventHandler } from "react-hexgrid/lib/Hexagon/Hexagon"
import { useSubscription, useSubscriptions } from "../ts-models/subscriptions"
import { LayersIcon } from "@modulz/radix-icons"
import { Center, Container, NumberInput, Paper, Stack } from "@mantine/core"
import {
  CurrentGameState,
  Player,
  Position,
  Tile,
} from "../ts-models/Glow.Sample"

function CoordinatesView({ hex: { q, r, s } }: { hex: Hex }) {
  return (
    <div>
      {q} {r} {s}
    </div>
  )
}

function positionsAreEqual(p1: Position, p2: Position | null) {
  if (p2 === null) {
    return false
  }
  return p1.q == p2.q && p1.r == p2.r && p1.s == p2.s
}

function RenderPlayer({ player }: { player: Player }) {
  return (
    <Paper
      shadow="xs"
      withBorder={true}
      p="xs"
      style={{ background: "var(--mantine-color-dark-6)" }}
    >
      <div>
        <div>
          <b>
            {player.icon} {player.name}
          </b>
          <div>❤ {player.health}</div>
          <div>⚔ {player.baseAttack}</div>
          <div>🛡 {player.baseProtection}</div>
          <div>
            <span style={{ color: "green" }}>➕</span> {player.regenRate}
          </div>
          {/* 💎💍👑🧿🔮♟🗿⚱ */}
        </div>
        <div>{player?.items?.map((v) => v.icon).join(" ")}</div>
        <CoordinatesView hex={player.position} />
      </div>
    </Paper>
  )
}

const hexagons = GridGenerator.orientedRectangle(18, 10)
// const hexagons = GridGenerator.rectangle(18, 10)
export function GameView() {
  const [currentState, setCurrentState] =
    React.useState<CurrentGameState | null>(null)

  useSubscription("Glow.Sample.ItemPicked", (e) => {})
  useSubscription(
    "Glow.Sample.PlayerAttacked",
    (e) => {
      const src = currentState?.players[e.attackingPlayer]!
      const target = currentState?.players[e.targetPlayer]!
      showNotification({
        title: "Player attacked",
        color: "red",
        message:
          src.name +
          " " +
          src.icon +
          " attacked " +
          target.name +
          " " +
          target.icon,
      })
    },
    [currentState],
  )

  useSubscription(
    "Glow.Sample.CurrentGameState",
    (e) => {
      setCurrentState(e)
    },
    [currentState],
  )
  useSubscription(
    "Glow.Sample.PlayerMoved",
    (e) => {
      const player = currentState?.players[e.playerId]
      if (player) {
        showNotification({
          title: "Player moved",
          message: player.name + " " + player.icon,
        })
      }
    },
    [currentState],
  )

  const [userId, setUserId] = React.useState<string | null>(null)
  const user = React.useMemo(
    () => (userId && currentState ? currentState?.players[userId] : null),
    [userId, currentState],
  )
  React.useEffect(() => {
    let user = localStorage.getItem("user")
    setUserId(user)
  })
  const [movePlayer] = useTypedAction("/api/ti/move-player")
  const hexagonSize = { x: 10, y: 10 }
  const handleMouseOver = React.useCallback(
    (e: React.SyntheticEvent, h) => {
      console.log({ h })
      const cell: Hex = h.state.hex
      const userPosition = user?.position

      console.log("calc distance", { cell, userPosition })
      const distance = HexUtils.distance(cell, userPosition)
      console.log({ distance })
    },
    [user, movePlayer],
  )

  const [selected, setSelected] = React.useState<null | Position>(null)

  const handleClick: HexagonMouseEventHandler<SVGGElement> | undefined =
    React.useCallback(
      (e, h) => {
        e.preventDefault()
        e.stopPropagation()
        const cell: Hex = h.state.hex
        const userPosition = user?.position
        if (userPosition) {
          const distance = HexUtils.distance(cell, userPosition)
          const direction = HexUtils.subtract(cell, userPosition)
          // const dir = HexUtils.direction(direction)
          // console.log({ direction, direction2 })
          if (distance === 1) {
            // showNotification({ message: "move", color: "green" })
            movePlayer({ id: userId!, direction: direction })
              .then((v) => {
                if (!v.ok) {
                  notifyError(v.error)
                }
              })
              .catch((e) => {})
          } else {
            // showNotification({
            //   message: "distance > 1 " + distance,
            //   color: "red",
            // })

            const position: Position = h.state.hex
            if (selected === null) {
              setSelected(position)
            } else if (!positionsAreEqual(position, selected)) {
              setSelected(position)
            } else {
              setSelected(null)
            }
          }
        }
      },
      [user, selected, setSelected],
    )

  const theme = useMantineTheme()
  // const colorScheme = useMantineColorScheme()

  // console.log({ theme })
  // console.log({ colorScheme })

  const playerIsSelected = user !== null

  // const showInDistance =
  //   playerIsSelected && user
  //     ? HexUtils.DIRECTIONS.map(
  //         (v) =>
  //           new Hex(
  //             user.position.q + v.q,
  //             user.position.r + v.r,
  //             user.position.s + v.s,
  //           ),
  //       ).filter((v) => HexUtils.distance(v, new Hex(0, 0, 0)) < 5)
  //     : []

  //"-50 -50 100 100"
  const [viewbox0, setViewbox0] = React.useState(-50)
  const [viewbox1, setViewbox1] = React.useState(-50)
  const [viewbox2, setViewbox2] = React.useState(100)
  const [viewbox3, setViewbox3] = React.useState(100)
  const [xy, setXY] = React.useState(5.0)
  const [spacing, setSpacing] = React.useState(1.02)

  if (currentState === null) {
    return <div>Loading</div>
  }

  function colorForTile(tile: Tile) {
    const color = tile.color || "dark"

    return theme.colors[color]?.[7] || tile.color || "dark"
  }

  return (
    <>
      <div
        style={{
          position: "absolute",
          // background: "#4868a84d",
          // background: "black",
          // background: "var(--mantine-color-dark-6)",
          zIndex: 10,
          left: 10,
          top: 60,
        }}
      >
        <div>{user && <RenderPlayer player={user} />}</div>
        {/* <div>
          <NumberInput
            value={viewbox0}
            onChange={(v) => setViewbox0(v!)}
            step={10}
          />
          <NumberInput
            value={viewbox1}
            onChange={(v) => setViewbox1(v!)}
            step={10}
          />
          <NumberInput
            value={viewbox2}
            onChange={(v) => setViewbox2(v!)}
            step={10}
          />
          <NumberInput
            value={viewbox3}
            onChange={(v) => setViewbox3(v!)}
            step={50}
          />
        </div> */}
        <Space my="xs" />
        <Paper
          shadow="xs"
          withBorder={true}
          p="xs"
          style={{ background: "var(--mantine-color-dark-6)" }}
        >
          <div>Status = {currentState.game.status}</div>

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
      </div>

      <div
        style={{
          position: "absolute",
          zIndex: 10,
          right: 10,
          top: 60,
        }}
      >
        <Stack>
          {currentState &&
            Object.keys(currentState.players)
              .map((v) => currentState.players[v]!)
              .map((v) => <RenderPlayer player={v} />)}
        </Stack>
      </div>

      <Center
        style={{
          width: "100vw",
          height: "calc(100vh - 40px)",
          position: "absolute",
        }}
      >
        {/* <Container>Default container</Container>
      <Container size="xs" px="xs">
        xs container with xs horizontal padding
      </Container>
      <Container size={200} px={0}>
        200px container with 0px horizontal padding
      </Container> */}
        <HexGrid
          width={"100%"}
          height={"100%"}
          viewBox={`${viewbox0} ${viewbox1} ${viewbox2} ${viewbox3}`}
        >
          <Layout size={{ x: xy, y: xy }} spacing={spacing}>
            <>
              {currentState.game?.field?.fields.map(
                ({ position: hex, tile }, i) => {
                  // console.log({ hex, user })
                  const inWalkingRange = Boolean(
                    user &&
                      HexUtils.distance(hex, user.position) > 0 &&
                      HexUtils.distance(hex, user.position) < 2,
                  )
                  return (
                    <BackgroundTile
                      tile={tile}
                      fillColor={colorForTile(tile)}
                      // draggable="true"
                      onDragStart={() => {}}
                      key={i}
                      q={hex.q}
                      r={hex.r}
                      s={hex.s}
                      // inWalkingRange={inWalkingRange && tile.walkable}
                      // onMouseOver={handleMouseOver}
                      // walkable={tile.walkable}
                      // className={tile.walkable ?  "blocked" : undefined}
                      // fill={hex.image ? HexUtils.getID(hex) : undefined}
                      data={hex}
                      onClick={
                        inWalkingRange && tile.walkable
                          ? handleClick
                          : undefined
                      }
                      // renderItem={(v) => <div>{v.q}</div>}
                      // onDragStart={(e, h) => this.onDragStart(e, h)}
                      // onDragEnd={(e, h, s) => this.onDragEnd(e, h, s)}
                      // onDrop={(e, h, t) => this.onDrop(e, h, t)}
                      // onDragOver={(e, h) => this.onDragOver(e, h)}
                    >
                      {inWalkingRange && tile.walkable ? (
                        <g>
                          {/* <circle cx="50" cy="0" r="10" />
                          <circle cx="50" cy="10" r="8" /> */}
                          <circle r="3.5" fill={"#f8f9fa52"} cursor="pointer" />
                        </g>
                      ) : //  <Text>In distance</Text>
                      null}
                      {/* <Text>
                      {hex.q} / {hex.r} / {hex.s}
                    </Text> */}
                    </BackgroundTile>
                  )
                },
              )}
              {/* {user && (
              <>
                {HexUtils.DIRECTIONS.map(
                  (v) =>
                    new Hex(
                      user.position.q + v.q,
                      user.position.r + v.r,
                      user.position.s + v.s,
                    ),
                ).map((v) => (
                  <SelectedTile q={v.q} r={v.r} s={v.s}>
                  </SelectedTile>
                ))}
              </>
            )} */}
              {selected !== null ? (
                <SelectedTile q={selected.q} r={selected.r} s={selected.s} />
              ) : null}
            </>
            <>
              {currentState.game.items
                .filter((v) => v.position !== null)
                .map((v, i) => (
                  <TransparentTile
                    onClick={handleClick}
                    // onClick={() =>
                    //   showNotification({
                    //     message: v.item.name + " " + v.item.icon,
                    //   })
                    // }
                    key={v.item.name + "" + i}
                    q={v.position.q}
                    r={v.position.r}
                    s={v.position.s}
                  >
                    <ItemText>
                      {v.item.icon!}
                      {/* {v.position.q} / {v.position.r} / {v.position.s} */}
                    </ItemText>
                  </TransparentTile>
                ))}
            </>
            <>
              {Object.keys(currentState.players)
                .map((v) => currentState.players[v]!)
                .filter((v) => v.position !== null)
                .map((v) => (
                  <TransparentTile
                    onClick={handleClick}
                    key={v.id}
                    q={v.position.q}
                    r={v.position.r}
                    s={v.position.s}
                  >
                    <UnitText>
                      {v.icon!} <br />
                      {/* {v.position.q} / {v.position.r} / {v.position.s} */}
                    </UnitText>
                  </TransparentTile>
                ))}
            </>

            <Pattern
              id="pat-1"
              link="http://lorempixel.com/400/400/cats/1/"
              size={hexagonSize}
            />
            <Pattern
              id="pat-2"
              link="http://lorempixel.com/400/400/cats/2/"
              size={hexagonSize}
            />
          </Layout>
        </HexGrid>
        {/* <RenderObject {...currentState.game?.field?.positions} /> */}
      </Center>
    </>
  )
}

const UnitText = styled(Text)`
  cursor: pointer;
  // svg g text {
  font-size: 0.1;
  fill: #252525;
  fill-opacity: 0.9;
  transition: fill-opacity 0.2s;
  // }
`

const ItemText = styled(Text)`
  cursor: pointer;
  // svg g text {
  font-size: 0.1;
  fill: #252525;
  fill-opacity: 0.9;
  transition: fill-opacity 0.2s;
  // }
`

const SelectedTile = styled(Hexagon)`
  fill-opacity: 0;
  stroke: var(--mantine-color-white);
  stroke-width: 0.2;
  stroke-opacity: 1;
  drop-shadow
`

const TransparentTile = styled(Hexagon)`
  fill-opacity: 0;
  &:hover {
    polygon {
      cursor: pointer;
      stroke: var(--mantine-color-white);
    }
  }

  // stroke-width: 0.2;
  stroke-opacity: 0.4;
`

const BackgroundTile = styled(Hexagon)<{
  tile?: Tile
  walkable: boolean
  fillColor: string
  inWalkingRange?: boolean
}>`
  polygon {
    fill-opacity: 0.92;
    transition: fill-opacity 0.2s;
    fill: ${({ fillColor }) => fillColor};
    stroke: transparent;
    stroke-width: 0.2;
  }
  &:hover {
    polygon {
      // transform: scale(1.05);
      fill-opacity: 1;
      transition: fill-opacity 0.9s;
      cursor: ${({ walkable }) => (walkable ? "pointer" : "curstor")};
    }
  }
`
