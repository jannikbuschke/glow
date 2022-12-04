import { useNotify } from "glow-core"
import React from "react"
import { HexGrid, Layout, Hexagon, Text, Hex, HexUtils } from "react-hexgrid"
import "./game.css"
import styled from "styled-components"
import { useMantineTheme } from "@mantine/styles"
import { Space } from "@mantine/core"
import { showNotification } from "@mantine/notifications"
import { HexagonMouseEventHandler } from "react-hexgrid/lib/Hexagon/Hexagon"
import { Center, Stack } from "@mantine/core"
import { css } from "@emotion/react"
import { useParams } from "react-router-dom"
import { ViewPanel } from "./game/view-panel"
import { RenderPlayer } from "./game/player-pannel"
import { GamePanel } from "./game/game-panel"
import { useSubscription, useSubscriptions } from "../client/subscriptions"
import {
  defaultCurrentGameState,
  Position,
  Tile,
} from "../client/TreasureIsland"
import { useTypedAction, useTypedQuery } from "../client/api"

function positionsAreEqual(p1: Position, p2: Position | null) {
  if (p2 === null) {
    return false
  }
  return p1.q == p2.q && p1.r == p2.r && p1.s == p2.s
}

export function GameView() {
  const { id } = useParams<{ id: string }>()
  const {
    data: currentState,
    refetch,
    isFetched,
  } = useTypedQuery("/api/ti/get-game-sate", {
    input: { gameId: id! },
    placeholder: defaultCurrentGameState,
  })

  useSubscriptions((name, e) => {
    showNotification({
      message: name,
      color: "gray",
    })
  }, [])
  useSubscriptions((e, ev) => {
    console.log({ e, ev })
  }, [])
  useSubscription("TreasureIsland.ItemPicked", (e) => {}, [])
  useSubscription(
    "TreasureIsland.UnitAttacked",
    (e) => {
      const src = currentState?.units[e.attackingUnit]!
      const target = currentState?.units[e.targetUnit]!

      showNotification({
        title: "Player attacked",
        color: "red",
        message: `${src.name} ${src.icon} attacked ${target.name} ${target.icon} and placed ${e.damage} damage`,
      })
    },
    [currentState],
  )

  useSubscription(
    "TreasureIsland.CurrentGameState",
    (e) => {
      refetch()
    },
    [currentState],
  )
  useSubscription(
    "TreasureIsland.UnitMoved",
    (e) => {
      const player = currentState?.units[e.unitId]
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
    () => (userId && currentState ? currentState?.units[userId] : null),
    [userId, currentState],
  )
  React.useEffect(() => {
    let user = localStorage.getItem("user")
    setUserId(user)
  }, [])
  const [movePlayer] = useTypedAction("/api/move-player")

  const [selected, setSelected] = React.useState<null | Position>(null)
  const { notifyError } = useNotify()
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

  const [viewbox0, setViewbox0] = React.useState(-50)
  const [viewbox1, setViewbox1] = React.useState(-50)
  const [viewbox2, setViewbox2] = React.useState(100)
  const [viewbox3, setViewbox3] = React.useState(100)
  const [xy, setXY] = React.useState(5.0)
  const [spacing, setSpacing] = React.useState(1.02)

  function colorForTile(tile: Tile) {
    const color = tile.color || "dark"

    return theme.colors[color]?.[7] || tile.color || "dark"
  }

  if (!isFetched) {
    return <div>Loading</div>
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
        <div>
          {user && <RenderPlayer player={user} currentState={currentState} />}
        </div>
        <Space my="xs" />
        <ViewPanel
          viewbox0={viewbox0}
          viewbox1={viewbox1}
          viewbox2={viewbox2}
          viewbox3={viewbox3}
          spacing={spacing}
          setSpacing={setSpacing}
          setViewbox0={setViewbox0}
          setViewbox1={setViewbox1}
          setViewbox2={setViewbox2}
          setViewbox3={setViewbox3}
          xy={xy}
          setXY={setXY}
        />
        <Space my="xs" />
        <GamePanel state={currentState} />
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
            Object.keys(currentState.units)
              .map((v) => currentState.units[v]!)
              .map((v) => (
                <RenderPlayer player={v} currentState={currentState} />
              ))}
        </Stack>
      </div>

      <Center
        style={{
          width: "100vw",
          height: "calc(100vh - 40px)",
          position: "absolute",
        }}
      >
        <HexGrid
          width={"100%"}
          height={"100%"}
          viewBox={`${viewbox0} ${viewbox1} ${viewbox2} ${viewbox3}`}
        >
          <Layout size={{ x: xy, y: xy }} spacing={spacing}>
            <>
              {currentState.game?.field?.fields.map(
                ({ position: hex, tile, items }, i) => {
                  const inWalkingRange = Boolean(
                    currentState?.game?.activeUnit === user?.id &&
                      user &&
                      HexUtils.distance(hex, user.position) > 0 &&
                      HexUtils.distance(hex, user.position) < 2,
                  )
                  return (
                    <Hexagon
                      css={css`
                        polygon {
                          fill: ${colorForTile(tile)};
                          fill-opacity: 0.92;
                          transition: fill-opacity 0.2s;
                          stroke: transparent;
                          stroke-width: 0.2;
                          border: ${inWalkingRange
                            ? "1px solid black"
                            : undefined};
                        }
                        &:hover {
                          polygon {
                            // transform: scale(1.05);
                            fill-opacity: 1;
                            transition: fill-opacity 0.9s;
                          }
                        }
                      `}
                      // tile={tile}
                      // fillColor={colorForTile(tile)}
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
                          <circle
                            r="3.5"
                            css={css`
                              transition: fill 4s;
                            `}
                            fill={"#f8f9fa52"}
                            cursor="pointer"
                          />
                        </g>
                      ) : //  <Text>In distance</Text>
                      null}
                      <Text>
                        {hex.q} / {hex.r} / {hex.s} /
                        {items.length > 0 ? "items" : ""}
                      </Text>
                    </Hexagon>
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
                <Hexagon
                  css={css`
                    fill-opacity: 0;
                    stroke: var(--mantine-color-white);
                    stroke-width: 0.2;
                    stroke-opacity: 1;
                  `}
                  q={selected.q}
                  r={selected.r}
                  s={selected.s}
                />
              ) : null}
            </>
            {/* <>
              {currentState.game.items
                .filter((v) => v.position !== null)
                .map((v, i) => (
                  <Hexagon
                    css={css`
                      fill-opacity: 0;
                      &:hover {
                        polygon {
                          cursor: pointer;
                          stroke: var(--mantine-color-white);
                        }
                      }


                      stroke-opacity: 0.4;
                    `}
                    onClick={handleClick}

                    key={v.item.name + "" + i}
                    q={v.position.q}
                    r={v.position.r}
                    s={v.position.s}
                  >
                    <ItemText>
                      {v.item.icon!}
                    </ItemText>
                  </Hexagon>
                ))}
            </> */}
            <>
              {Object.keys(currentState.units)
                .map((v) => currentState.units[v]!)
                .filter((v) => v.position !== null)
                .map((v) => (
                  <Hexagon
                    css={css`
                      fill-opacity: 0;
                      &:hover {
                        polygon {
                          cursor: pointer;
                          stroke: var(--mantine-color-white);
                        }
                      }

                      // stroke-width: 0.2;
                      stroke-opacity: 0.4;
                    `}
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
                  </Hexagon>
                ))}
            </>

            {/* <Pattern
              id="pat-1"
              link="http://lorempixel.com/400/400/cats/1/"
              size={hexagonSize}
            />
            <Pattern
              id="pat-2"
              link="http://lorempixel.com/400/400/cats/2/"
              size={hexagonSize}
            /> */}
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
