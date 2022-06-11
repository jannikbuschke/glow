import { RenderObject, useNotification } from "glow-core"
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

import { CurrentGameState } from "../ts-models/Glow.Sample.TreasureIsland.Projections"
import { showNotification } from "@mantine/notifications"
import { useTypedAction } from "../ts-models/api"
import { Position, Tile } from "../ts-models/Glow.Sample.TreasureIsland.Domain"
import { HexagonMouseEventHandler } from "react-hexgrid/lib/Hexagon/Hexagon"

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

const hexagons = GridGenerator.orientedRectangle(18, 10)
// const hexagons = GridGenerator.rectangle(18, 10)
export function GameView() {
  const [currentState, setCurrentState] =
    React.useState<CurrentGameState | null>(null)
  useNotification<CurrentGameState>(
    "Glow.Sample.TreasureIsland.Projections.CurrentGameState",
    (v) => {
      console.log({ v })
      setCurrentState(v)
    },
  )
  useNotification<any>("*", (v, x) => {
    console.log({ wildcard: v, x })
  })
  const [userId, setUserId] = React.useState<string | null>(null)
  const user = React.useMemo(
    () =>
      userId && currentState
        ? currentState?.players.find((v) => v.id == userId)
        : null,
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
            showNotification({ message: "move", color: "green" })
            movePlayer({ id: userId!, direction: direction })
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

  const showInDistance =
    playerIsSelected && user
      ? HexUtils.DIRECTIONS.map(
          (v) =>
            new Hex(
              user.position.q + v.q,
              user.position.r + v.r,
              user.position.s + v.s,
            ),
        ).filter((v) => HexUtils.distance(v, new Hex(0, 0, 0)) < 5)
      : []

  if (currentState === null) {
    return <div>Loading</div>
  }

  function colorForTile(tile: Tile) {
    const color = tile.color || "dark"
    // console.log({
    //   color,
    //   colors: theme.colors,
    //   selectedColor: theme.colors[color]?.[0],
    // })
    return theme.colors[color]?.[6] || tile.color || "dark"
  }

  return (
    <div>
      <div>
        {/* {user?.id}
        <br /> */}
        {user?.icon} {/* <br /> */}
        {user?.name}{" "}
        {user?.position && <CoordinatesView hex={user?.position} />}
        <br />
        {/* {JSON.stringify(user?.position, null, 2)} */}
        {JSON.stringify(
          user?.items?.map((v) => v.icon),
          null,
          2,
        )}
      </div>
      Status = {currentState.game.status}
      <HexGrid width={1100} height={900} viewBox="-30 -50 100 100">
        <Layout
          size={{ x: 4.9, y: 4.9 }}
          //  space={0.75}
        >
          <>
            {currentState.game?.field?.fields.map(
              ({ position: hex, tile }, i) => {
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
                      inWalkingRange && tile.walkable ? handleClick : undefined
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
            {currentState !== null
              ? currentState.currentItems
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
                  ))
              : null}
          </>
          <>
            {currentState !== null
              ? currentState.players
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
                  ))
              : null}
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
    </div>
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
