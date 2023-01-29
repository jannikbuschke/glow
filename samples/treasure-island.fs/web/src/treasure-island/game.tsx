import { RenderObject, useNotify } from "glow-core"
import React from "react"
import {
  HexGrid,
  Layout,
  Hexagon,
  Text,
  Hex,
  HexUtils,
  Pattern,
} from "react-hexgrid"
import "./game.css"
import styled from "styled-components"
import { useMantineTheme } from "@mantine/styles"
import { Button, Space } from "@mantine/core"
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
  defaultError,
  defaultGame,
  Game,
  Position,
  Tile,
} from "../client/TreasureIsland"
import { useTypedAction, useTypedQuery } from "../client/api"
import { Guid } from "../client/System"
import { match } from "ts-pattern"
import { defaultFSharpResult } from "../client/Microsoft_FSharp_Core"
import img1 from "../assets/mountain-and-river.png"
import img2 from "../assets/mountain-and-river-2.png"
import { containsKey, FsMap } from "./fsharp-map"
import { ErrorBoundary } from "glow-core/lib/errors/error-boundary"
// import grass4 from "../assets/mountain-and-river-2.png"
// const images = require.context("../assets/", true)

function positionsAreEqual(p1: Position, p2: Position | null) {
  if (p2 === null) {
    return false
  }
  return p1.q == p2.q && p1.r == p2.r && p1.s == p2.s
}

export function GameView() {
  const { id } = useParams<{ id: Guid }>()
  const {
    data: currentState,
    refetch,
    isFetched,
  } = useTypedQuery("/api/ti/get-game-sate", {
    input: { gameId: id! },
    placeholder: defaultFSharpResult(defaultGame, defaultError),
  })

  if (!isFetched) {
    return <div>Loading</div>
  }
  return match(currentState)
    .with({ Case: "Error" }, (error) => <div>Error {error}</div>)
    .with({ Case: "Ok" }, ({ Fields }) => (
      <RenderGame game={Fields} refetch={refetch} />
    ))
    .exhaustive()
}

function getActiveUnit(game: Game) {
  return game.activeUnit !== null
    ? FsMap.get(game.playerUnits, game.activeUnit)
    : null
}

export function RenderGame({
  game: currentState,
  refetch,
}: {
  game: Game
  refetch: () => void
}) {
  useSubscriptions((name, e) => {
    showNotification({
      message: name,
      color: "gray",
    })
  }, [])

  const [userId, setUserId] = React.useState<Guid | null>(null)

  const [user, activeUnit] = React.useMemo(() => {
    console.log({
      userId,
      players: currentState.players,
      units: currentState.playerUnits,
    })

    if (userId !== null) {
      if (FsMap.containsKey(currentState.players, userId)) {
        console.log("user exists")
        const player = FsMap.get(currentState.players, userId)
        console.log("player", player)
        return [player, null]
      } else {
        console.log("current user does not exist in players")

        return [null, null]
      }
      // const player = currentState.players[userId]
      // console.log({ players: currentState.players })
      // return player
      // const unit = currentState.playerUnits.find((v) => v[1].playerId == userId)
      // return unit
    } else {
      return [null, null]
    }
  }, [userId, currentState])
  React.useEffect(() => {
    let user = localStorage.getItem("user") as Guid | null
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
        console.log("unit clicked", e, h, cell)

        const userPosition = null as any // user?.position
        if (userPosition) {
          const distance = HexUtils.distance(cell, userPosition)
          const direction = HexUtils.subtract(cell, userPosition)
          if (distance === 1) {
            // showNotification({ message: "move", color: "green" })
            // TODO: FIX
            movePlayer({
              unitId: userId!,
              direction: direction,
              gameId: currentState.id,
            })
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

  const [viewbox0, setViewbox0] = React.useState(-25)
  const [viewbox1, setViewbox1] = React.useState(-30)
  const [viewbox2, setViewbox2] = React.useState(70)
  const [viewbox3, setViewbox3] = React.useState(70)
  const [xy, setXY] = React.useState(5.0)
  const [spacing, setSpacing] = React.useState(1.0)

  function colorForTile(tile: Tile) {
    const color = tile.color || "dark"

    return theme.colors[color]?.[7] || tile.color || "dark"
  }

  const img = [
    "grass-4",
    "grass-and-river",
    "grass-and-road",
    "grass-and-stones",
    "grass-and-water",
    "mountain-1",
    "mountain-2",
    "mountain-3",
    "mountain-4",
    "mountain-5",
    "mountain-6",
    "mountain-and-river-2",
    "mountain-and-river",
    "mountain-and-sea",
    "mountain-or-stone-3",
    "mountain-wood-river",
    "river-1",
    "river-2",
    "river-3",
    "river-6",
    "river-and-grass",
    "river",
    "road-1",
    "stones",
    "unclear",
    "water",
    "wood-1",
  ]
  const characters = [
    "diamant",
    "ghost",
    "hexe",
    "long-beard",
    "old-man",
    "warrior-highlight",
    "young-wizard",
  ]
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
          <ErrorBoundary>
            {user && <RenderPlayer player={user} currentState={currentState} />}
          </ErrorBoundary>
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
        <Space my="xs" />

        <Button
          onClick={() => {
            refetch
          }}
        >
          Refresh
        </Button>
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
          <div
            style={{
              // background: "white",
              opacity: 0.5,
            }}
          >
            <RenderObject
              {...{
                players: currentState.players,
                activeUnit: currentState.activeUnit,
                units: currentState.playerUnits,
              }}
            />
          </div>
          {currentState.players.map((v, i) => (
            <div key={i}>{v[0]}</div>
          ))}
          {/* {currentState &&
            Object.keys(currentState.units)
              .map((v) => currentState.units[v]!)
              .map((v) => (
                <RenderPlayer player={v} currentState={currentState} />
              ))} */}
        </Stack>
      </div>

      <div
        style={{
          width: "100vw",
          height: "calc(100vh - 40px)",
          position: "absolute",

          // backgroundImage: images("./wooden-background-tile.png"),
        }}
        css={css`
          /* position: absolute; */
        `}
      >
        {/* <RenderObject c={currentState} /> */}
      </div>
      <Center
        style={{
          width: "100vw",
          height: "calc(100vh - 40px)",
          position: "absolute",
          background: "#D0A97A",

          // backgroundImage: `url(${images("./wooden-background-tile.png")})`,
        }}
      >
        <HexGrid
          width={"100%"}
          height={"100%"}
          viewBox={`${viewbox0} ${viewbox1} ${viewbox2} ${viewbox3}`}
        >
          <Layout size={{ x: xy, y: xy }} spacing={spacing}>
            <>
              {currentState.field.fields.map(
                ({ position: hex, tile, items }, i) => {
                  const inWalkingRange = Boolean(
                    currentState?.activeUnit === user?.id &&
                      user &&
                      HexUtils.distance(hex, user.position) > 0 &&
                      HexUtils.distance(hex, user.position) < 2,
                  )
                  return (
                    <Hexagon
                      key={"" + i}
                      fill={`./${
                        tile.assetIds[0] ||
                        img[Math.floor(Math.random() * img.length)]
                      }.png`}
                      css={css`
                        polygon {
                          /* fill: ${colorForTile(tile)}; */
                          fill-opacity: 0.78;
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
                        {/* {hex.q} / {hex.r} / {hex.s} / */}
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
              {currentState.playerUnits.map(
                ([id, { icon, position, assetId }]) => (
                  <Hexagon
                    key={id}
                    fill={`./${assetId}`}
                    css={css`
                      polygon {
                        fill-opacity: 0.9;
                        transition: fill-opacity 0.2s;
                        stroke: ${currentState.activeUnit === id
                          ? "var(--mantine-color-white)"
                          : "transparent"};
                        stroke-width: 0.5;
                      }
                      &:hover {
                        polygon {
                          cursor: pointer;
                          transform: scale(1.05);
                          fill-opacity: 1;
                          transition: fill-opacity 0.9s;
                        }
                      }
                      /* &:hover {
                        polygon {
                          cursor: pointer;
                          stroke: var(--mantine-color-white);
                        }
                      } */

                      stroke-opacity: 0.4;
                    `}
                    // css={css`
                    //   &:hover {
                    //     polygon {
                    //       cursor: pointer;
                    //       stroke: var(--mantine-color-white);
                    //     }
                    //   }

                    //   stroke-opacity: 0.4;
                    // `}
                    onClick={handleClick}
                    q={position.q}
                    r={position.r}
                    s={position.s}
                  >
                    <UnitText color="white">
                      {/* {icon} */}
                      {/* {position.q} / {position.r} / {position.s} */}
                    </UnitText>
                  </Hexagon>
                ),
              )}

              {[...img].map((v, i) => (
                <Pattern
                  key={i}
                  id={`./${v}.png`}
                  link={`./${v}.png`}
                  size={{ x: 5, y: 5 }}
                />
              ))}
              {[, ...characters].map((v, i) => (
                <Pattern
                  key={i}
                  id={`./${v}.png`}
                  link={`./${v}.png`}
                  dx={1.4}
                  dy={0.9}
                  size={{ x: 3.5, y: 3.5 }}
                />
              ))}
            </>
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
