import React, { useCallback, useEffect, useRef, useState } from "react"
import { createPortal, unstable_batchedUpdates } from "react-dom"
import {
  CancelDrop,
  closestCenter,
  rectIntersection,
  CollisionDetection,
  DndContext,
  DragOverlay,
  DropAnimation,
  defaultDropAnimation,
  KeyboardSensor,
  MouseSensor,
  TouchSensor,
  Modifiers,
  useDroppable,
  UniqueIdentifier,
  useSensors,
  useSensor,
  MeasuringStrategy,
} from "@dnd-kit/core"
import {
  AnimateLayoutChanges,
  SortableContext,
  useSortable,
  arrayMove,
  defaultAnimateLayoutChanges,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
  SortingStrategy,
  horizontalListSortingStrategy,
} from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities"
import { Container, ContainerProps } from "./container/index"
import { Item } from "./item/index"
import { MeetingItem } from "../../ts-models/Sample.Fs.Agenda"
import { useToast } from "@chakra-ui/react"
import { RenderObject } from "glow-react"
import { useTypedAction } from "../../ts-models/api"

const defaultInitializer = (index: number) => index

export function createRange<T = number>(
  length: number,
  initializer: (index: number) => any = defaultInitializer,
): T[] {
  return [...new Array(length)].map((_, index) => initializer(index))
}

const animateLayoutChanges: AnimateLayoutChanges = (args) =>
  args.isSorting || args.wasDragging ? defaultAnimateLayoutChanges(args) : true

function DroppableContainer({
  children,
  columns = 1,
  disabled,
  id,
  items,
  style,
  ...props
}: ContainerProps & {
  disabled?: boolean
  id: string
  items: string[]
  style?: React.CSSProperties
}) {
  const {
    active,
    attributes,
    isDragging,
    listeners,
    over,
    setNodeRef,
    transition,
    transform,
  } = useSortable({
    id,
    data: {
      type: "container",
    },
    animateLayoutChanges,
  })
  const isOverContainer = over
    ? (id === over.id && active?.data.current?.type !== "container") ||
      items.includes(over.id)
    : false

  return (
    <Container
      ref={disabled ? undefined : setNodeRef}
      style={{
        ...style,
        transition,
        transform: CSS.Translate.toString(transform),
        opacity: isDragging ? 0.5 : undefined,
      }}
      hover={isOverContainer}
      handleProps={{
        ...attributes,
        ...listeners,
      }}
      columns={columns}
      {...props}
    >
      {children}
    </Container>
  )
}

const dropAnimation: DropAnimation = {
  ...defaultDropAnimation,
  dragSourceOpacity: 0.5,
}

type Items = Record<string, MeetingItem[]>
// type Items = Record<string, string[]>

interface Props {
  meetingId: string
  adjustScale?: boolean
  cancelDrop?: CancelDrop
  columns?: number
  containerStyle?: React.CSSProperties
  getItemStyles?(args: {
    value: UniqueIdentifier
    index: number
    overIndex: number
    isDragging: boolean
    containerId: UniqueIdentifier
    isSorting: boolean
    isDragOverlay: boolean
  }): React.CSSProperties
  wrapperStyle?(args: { index: number }): React.CSSProperties
  itemCount?: number
  items?: Items
  handle?: boolean
  renderItem?: any
  strategy?: SortingStrategy
  modifiers?: Modifiers
  minimal?: boolean
  trashable?: boolean
  scrollable?: boolean
  vertical?: boolean
}

export const TRASH_ID = "void"
const PLACEHOLDER_ID = "placeholder"
const empty: UniqueIdentifier[] = []

export function AgendaView({
  meetingId,
  adjustScale = false,
  itemCount = 3,
  cancelDrop,
  columns,
  handle = false,
  items: initialItems,
  containerStyle,
  getItemStyles = () => ({}),
  wrapperStyle = () => ({}),
  minimal = false,
  modifiers,
  renderItem,
  strategy = verticalListSortingStrategy,
  trashable = false,
  vertical = false,
  scrollable,
}: Props) {
  const [items, setItems] = useState<Items>(
    () =>
      initialItems ?? {
        A: createRange(
          itemCount,
          (index) =>
            ({
              id: `A${index + 1}`,
            } as MeetingItem),
        ),
        // B: createRange(itemCount, (index) => {
        //   id: `B${index + 1}`
        // }),
        // C: createRange(itemCount, (index) => {
        //   id: `C${index + 1}`
        // }),
        // D: createRange(itemCount, (index) => {
        //   id: `D${index + 1}`
        // }),
      },
  )
  const [containers, setContainers] = useState(Object.keys(items))
  const [activeId, setActiveId] = useState<string | null>(null)
  const [activeItem, setActiveItem] = useState<MeetingItem | null>(null)
  const lastOverId = useRef<UniqueIdentifier | null>(null)
  const recentlyMovedToNewContainer = useRef(false)
  const isSortingContainer = activeId ? containers.includes(activeId) : false
  // Custom collision detection strategy optimized for multiple containers
  const collisionDetectionStrategy: CollisionDetection = useCallback(
    (args) => {
      // Start by finding any intersecting droppable
      let overId = rectIntersection(args)

      if (activeId && activeId in items) {
        return closestCenter({
          ...args,
          droppableContainers: args.droppableContainers.filter(
            (container) => container.id in items,
          ),
        })
      }

      if (overId != null) {
        if (overId in items) {
          const containerItems = items[overId]

          // If a container is matched and it contains items (columns 'A', 'B', 'C')
          if (containerItems.length > 0) {
            // Return the closest droppable within that container
            overId = closestCenter({
              ...args,
              droppableContainers: args.droppableContainers.filter(
                (container) =>
                  container.id !== overId &&
                  containerItems.some((v) => v.id === container.id),
                // containerItems.includes(container.id),
              ),
            })
          }
        }

        lastOverId.current = overId

        return overId
      }

      // When a draggable item moves to a new container, the layout may shift
      // and the `overId` may become `null`. We manually set the cached `lastOverId`
      // to the id of the draggable item that was moved to the new container, otherwise
      // the previous `overId` will be returned which can cause items to incorrectly shift positions
      if (recentlyMovedToNewContainer.current) {
        lastOverId.current = activeId
      }

      // If no droppable is matched, return the last match
      return lastOverId.current
    },
    [activeId, items],
  )
  const [move] = useTypedAction("/api/reorder-agenda-items")
  const [clonedItems, setClonedItems] = useState<Items | null>(null)
  const sensors = useSensors(
    useSensor(MouseSensor),
    useSensor(TouchSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  )
  const findContainer = (id: string) => {
    if (id in items) {
      return id
    }

    return Object.keys(items).find((key) => items[key].some((v) => v.id === id))
  }
  const findItem = (id: string) => {
    const container = findContainer(id)
    if (container) {
      return items[container].find((v) => v.id === id)
    }
    return undefined
    // return Object.keys(items).find((key) => items[key].some((v) => v.id === id))
  }
  const getIndex = (id: string) => {
    // const getIndex = (id: MeetingItem) => {
    const container = findContainer(id)

    if (!container) {
      return -1
    }

    const index = items[container].findIndex((v) => v.id === id)
    // const index = items[container].indexOf(id)

    return index
  }

  const onDragCancel = () => {
    if (clonedItems) {
      // Reset items to their original state in case items have been
      // Dragged across containrs
      setItems(clonedItems)
    }

    setActiveId(null)
    setActiveItem(null)
    setClonedItems(null)
  }

  useEffect(() => {
    requestAnimationFrame(() => {
      recentlyMovedToNewContainer.current = false
    })
  }, [items])
  const toast = useToast()
  return (
    <DndContext
      sensors={sensors}
      collisionDetection={collisionDetectionStrategy}
      measuring={{
        droppable: {
          strategy: MeasuringStrategy.Always,
        },
      }}
      onDragStart={({ active }) => {
        setActiveId(active.id)
        setActiveItem(findItem(active.id) || null)
        setClonedItems(items)
      }}
      onDragOver={({ active, over }) => {
        const overId = over?.id

        if (!overId || overId === TRASH_ID || active.id in items) {
          return
        }

        const overContainer = findContainer(overId)
        const activeContainer = findContainer(active.id)

        if (!overContainer || !activeContainer) {
          return
        }

        if (activeContainer !== overContainer) {
          setItems((items) => {
            const activeItems = items[activeContainer]
            const overItems = items[overContainer]
            //
            // const overIndex = overItems.indexOf(overId)
            const overIndex = overItems.findIndex((v) => v.id === overId)
            // const activeIndex = activeItems.indexOf(active.id)
            const activeIndex = activeItems.findIndex((v) => v.id === active.id)

            let newIndex: number

            if (overId in items) {
              newIndex = overItems.length + 1
            } else {
              const isBelowOverItem =
                over &&
                active.rect.current.translated &&
                active.rect.current.translated.offsetTop >
                  over.rect.offsetTop + over.rect.height

              const modifier = isBelowOverItem ? 1 : 0

              newIndex =
                overIndex >= 0 ? overIndex + modifier : overItems.length + 1
            }

            recentlyMovedToNewContainer.current = true

            return {
              ...items,
              [activeContainer]: items[activeContainer].filter(
                (item) => item.id !== active.id,
              ),
              [overContainer]: [
                ...items[overContainer].slice(0, newIndex),
                items[activeContainer][activeIndex],
                ...items[overContainer].slice(
                  newIndex,
                  items[overContainer].length,
                ),
              ],
            }
          })
        }
      }}
      onDragEnd={({ active, over }) => {
        // toast({ status: "info", title: "drag end" })

        if (active.id in items && over?.id) {
          // toast({ status: "success", title: "array  move (containers)" })

          setContainers((containers) => {
            const activeIndex = containers.indexOf(active.id)
            const overIndex = containers.indexOf(over.id)
            // toast({ status: "success", title: "array  move (containers)" })

            return arrayMove(containers, activeIndex, overIndex)
          })
        }

        const activeContainer = findContainer(active.id)

        if (!activeContainer) {
          // toast({ status: "success", title: "no active container" })

          setActiveId(null)
          return
        }

        const overId = over?.id

        if (!overId) {
          // toast({ status: "success", title: "fooo1" })

          setActiveId(null)
          // toast({
          //   status: "success",
          //   title: "set active id = null / no active id",
          // })

          return
        }

        if (overId === TRASH_ID) {
          // toast({ status: "success", title: "only set items (trash)" })

          setItems((items) => ({
            ...items,
            [activeContainer]: items[activeContainer].filter(
              ({ id }) => id !== activeId,
            ),
          }))
          setActiveId(null)
          return
        }

        if (overId === PLACEHOLDER_ID) {
          const newContainerId = getNextContainerId()

          unstable_batchedUpdates(() => {
            setContainers((containers) => [...containers, newContainerId])
            const activeItem = findItem(active.id)!
            // toast({ status: "success", title: "only set items" })

            setItems((items) => ({
              ...items,
              [activeContainer]: items[activeContainer].filter(
                ({ id }) => id !== activeId,
              ),
              [newContainerId]: [activeItem], // [active.id],
            }))
            setActiveId(null)
          })
          return
        }

        const overContainer = findContainer(overId)

        if (overContainer) {
          // const activeIndex = items[activeContainer].indexOf(active.id)
          const activeIndex = items[activeContainer].findIndex(
            (v) => v.id === active.id,
          )
          // const overIndex = items[overContainer].indexOf(overId)
          const overIndex = items[overContainer].findIndex(
            (v) => v.id === overId,
          )

          if (activeIndex !== overIndex) {
            move({ meetingId, oldIndex: activeIndex, newIndex: overIndex })
            // toast({
            //   status: "info",
            //   title:
            //     "set items / array move" +
            //     `active index ${activeIndex} over index ${overIndex}`,
            //   description: `active index ${activeIndex} over index ${overIndex}`,
            // })
            //
            setItems((items) => ({
              ...items,
              [overContainer]: arrayMove(
                items[overContainer],
                activeIndex,
                overIndex,
              ),
            }))
          } else {
            // toast({ status: "success", title: "do nothing" })
          }
        }

        setActiveId(null)
      }}
      cancelDrop={cancelDrop}
      onDragCancel={onDragCancel}
      modifiers={modifiers}
    >
      <div
        style={{
          display: "inline-grid",
          boxSizing: "border-box",
          padding: 20,
          gridAutoFlow: vertical ? "row" : "column",
        }}
      >
        {/* <RenderObject items={items} /> */}
        <SortableContext
          items={[...containers, PLACEHOLDER_ID]}
          strategy={
            vertical
              ? verticalListSortingStrategy
              : horizontalListSortingStrategy
          }
        >
          {containers.map((containerId) => (
            <DroppableContainer
              key={containerId}
              id={containerId}
              label={minimal ? undefined : `Column ${containerId}`}
              columns={columns}
              // todo
              items={items[containerId].map((v) => v.id)}
              scrollable={scrollable}
              style={containerStyle}
              unstyled={minimal}
              onRemove={() => handleRemove(containerId)}
            >
              <SortableContext items={items[containerId]} strategy={strategy}>
                {items[containerId].map((value, index) => {
                  return (
                    <SortableItem
                      disabled={isSortingContainer}
                      key={value.id}
                      id={value.id}
                      item={value}
                      index={index}
                      handle={handle}
                      style={getItemStyles}
                      wrapperStyle={wrapperStyle}
                      renderItem={renderItem}
                      containerId={containerId}
                      getIndex={getIndex}
                    />
                  )
                })}
              </SortableContext>
            </DroppableContainer>
          ))}
          {minimal ? undefined : (
            <DroppableContainer
              id={PLACEHOLDER_ID}
              disabled={isSortingContainer}
              items={empty}
              onClick={handleAddColumn}
              placeholder
            >
              + Add column
            </DroppableContainer>
          )}
        </SortableContext>
      </div>
      {createPortal(
        <DragOverlay adjustScale={adjustScale} dropAnimation={dropAnimation}>
          {activeId
            ? containers.includes(activeId)
              ? renderContainerDragOverlay(activeId)
              : renderSortableItemDragOverlay(
                  activeId,
                  activeItem?.displayName || "",
                )
            : null}
        </DragOverlay>,
        document.body,
      )}
      {trashable && activeId && !containers.includes(activeId) ? (
        <Trash id={TRASH_ID} />
      ) : null}
    </DndContext>
  )

  function renderSortableItemDragOverlay(id: string, title: string) {
    return (
      <Item
        value={<div>{title}</div>}
        handle={handle}
        style={getItemStyles({
          containerId: findContainer(id) as string,
          overIndex: -1,
          index: getIndex(id),
          value: id,
          isSorting: true,
          isDragging: true,
          isDragOverlay: true,
        })}
        color={getColor(id)}
        wrapperStyle={wrapperStyle({ index: 0 })}
        renderItem={renderItem}
        dragOverlay
      />
    )
  }

  function renderContainerDragOverlay(containerId: string) {
    return (
      <Container
        label={`Column ${containerId}`}
        columns={columns}
        style={{
          height: "100%",
        }}
        shadow
        unstyled={false}
      >
        {items[containerId].map((item, index) => (
          <Item
            key={item.id}
            value={item}
            handle={handle}
            style={getItemStyles({
              containerId,
              overIndex: -1,
              index: getIndex(item.id),
              value: item.id,
              isDragging: false,
              isSorting: false,
              isDragOverlay: false,
            })}
            color={getColor(item.id)}
            wrapperStyle={wrapperStyle({ index })}
            renderItem={renderItem}
          />
        ))}
      </Container>
    )
  }

  function handleRemove(containerID: UniqueIdentifier) {
    setContainers((containers) => containers.filter((id) => id !== containerID))
  }

  function handleAddColumn() {
    const newContainerId = getNextContainerId()

    unstable_batchedUpdates(() => {
      setContainers((containers) => [...containers, newContainerId])
      setItems((items) => ({
        ...items,
        [newContainerId]: [],
      }))
    })
  }

  function getNextContainerId() {
    const containeIds = Object.keys(items)
    const lastContaineId = containeIds[containeIds.length - 1]

    return String.fromCharCode(lastContaineId.charCodeAt(0) + 1)
  }
}

function getColor(id: string) {
  return "#7193f1"
  switch (id[0]) {
    case "A":
      return "#7193f1"
    case "B":
      return "#ffda6c"
    case "C":
      return "#00bcd4"
    case "D":
      return "#ef769f"
  }

  return undefined
}

function Trash({ id }: { id: UniqueIdentifier }) {
  const { setNodeRef, isOver } = useDroppable({
    id,
  })

  return (
    <div
      ref={setNodeRef}
      style={{
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        position: "fixed",
        left: "50%",
        marginLeft: -150,
        bottom: 20,
        width: 300,
        height: 60,
        borderRadius: 5,
        border: "1px solid",
        borderColor: isOver ? "red" : "#DDD",
      }}
    >
      Drop here to delete
    </div>
  )
}

interface SortableItemProps {
  containerId: string
  id: string
  item: MeetingItem
  index: number
  handle: boolean
  disabled?: boolean
  style(args: any): React.CSSProperties
  getIndex(id: string): number
  renderItem(): React.ReactElement
  wrapperStyle({ index }: { index: number }): React.CSSProperties
}

function SortableItem({
  disabled,
  id,
  item,
  index,
  handle,
  renderItem,
  style,
  containerId,
  getIndex,
  wrapperStyle,
}: SortableItemProps) {
  const {
    setNodeRef,
    listeners,
    isDragging,
    isSorting,
    over,
    overIndex,
    transform,
    transition,
  } = useSortable({
    id,
  })
  const mounted = useMountStatus()
  const mountedWhileDragging = isDragging && !mounted

  return (
    <Item
      ref={disabled ? undefined : setNodeRef}
      value={<div>{item.displayName}</div>}
      dragging={isDragging}
      sorting={isSorting}
      handle={handle}
      index={index}
      wrapperStyle={wrapperStyle({ index })}
      style={style({
        index,
        value: id,
        isDragging,
        isSorting,
        overIndex: over ? getIndex(over.id) : overIndex,
        containerId,
      })}
      color={getColor(id)}
      transition={transition}
      transform={transform}
      fadeIn={mountedWhileDragging}
      listeners={listeners}
      renderItem={renderItem}
    />
  )
}

function useMountStatus() {
  const [isMounted, setIsMounted] = useState(false)

  useEffect(() => {
    const timeout = setTimeout(() => setIsMounted(true), 500)

    return () => clearTimeout(timeout)
  }, [])

  return isMounted
}
