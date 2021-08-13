import { Table as AntTable } from "antd"
import { MenuOutlined } from "@ant-design/icons"
import {
  closestCenter,
  DndContext,
  DragOverlay,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core"
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable"
import * as React from "react"
import { useField } from "formik"
import { TableProps } from "antd/lib/table/Table"
import styled from "styled-components"

type Props<RecordType extends object = any> = TableProps<RecordType> & {
  name: string
  sortProperty: string
}

export function SortableTable({ name, columns, sortProperty, ...rest }: Props) {
  const [{ value }, , { setValue }] = useField(name)
  const tableColumns = [
    {
      key: "dragHandle",
      dataIndex: "dragHandle",
      title: "Drag",
      width: 30,
      render: () => <MenuOutlined />,
    },
    ...(columns ? columns : []),
  ]

  const [activeId, setActiveId] = React.useState(null)

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  )

  function handleDragStart(event: any) {
    const { active } = event
    setActiveId(active.id)
  }

  function getKey(item: any) {
    return rest.rowKey
      ? typeof rest.rowKey === "function"
        ? rest.rowKey(item)
        : item[rest.rowKey!]
      : item.id
  }

  function handleDragEnd(event: any) {
    const { active, over } = event
    console.log("drag end", { active, over })
    if (active?.id !== over?.id) {
      const editedArray = [...value]
      console.log({ active: active?.id, over: over?.id })

      const oldIndex = editedArray.findIndex(
        (item) => getKey(item) === active.id,
      )
      const newIndex = editedArray.findIndex((item) => getKey(item) === over.id)
      const editedArray2 = arrayMove(editedArray, oldIndex, newIndex)
      for (let i = 0; i < editedArray.length; i++) {
        editedArray2[i][sortProperty] = i
      }
      setValue(editedArray2)
    }
    // Stop overlay.
    setActiveId(null)
  }
  if (!Array.isArray) {
    return <div>Field '{name}' is not an array</div>
  }
  return (
    <DndContext
      sensors={sensors}
      collisionDetection={closestCenter}
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
    >
      <Table
        {...rest}
        columns={tableColumns}
        dataSource={value}
        components={{
          body: {
            wrapper: DraggableWrapper,
            row: DraggableRow,
          },
        }}
      />
      {/* Render overlay component. */}
      <DragOverlay>
        <div>{activeId ? activeId : null}</div>
      </DragOverlay>
    </DndContext>
  )

  function DraggableWrapper(props: any) {
    const { children, ...restProps } = props
    return (
      <SortableContext
        // `children[1]` is `dataSource`.
        items={children[1].map((child: any) => child.key)}
        strategy={verticalListSortingStrategy}
        {...restProps}
      >
        <tbody {...restProps}>
          {
            // This invokes `Table.components.body.row` for each element of `children`.
            children
          }
        </tbody>
      </SortableContext>
    )
  }

  function DraggableRow(props: any) {
    const {
      attributes,
      listeners,
      setNodeRef,
      isDragging,
      overIndex,
      index,
    } = useSortable({
      id: props["data-row-key"],
    })
    const isOver = overIndex === index
    const { children, ...restProps } = props
    return (
      <tr
        ref={setNodeRef}
        {...attributes}
        {...restProps}
        style={
          isDragging
            ? { background: "#80808038" }
            : isOver
            ? {
                borderTop: "5px solid #ec161638",
                borderCollapse: "collapse",
              }
            : undefined
        }
      >
        {children.map((child: any) => {
          const { children, key, ...restProps } = child
          return key === "dragHandle" ? (
            <td {...listeners} {...restProps}>
              {child}
            </td>
          ) : (
            <td {...restProps}>{child}</td>
          )
        })}
      </tr>
    )
  }
}

const Table = styled(AntTable)`
  .ant-table table {
    border-collapse: collapse;
  }
`
