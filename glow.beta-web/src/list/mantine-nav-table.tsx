import React from "react"
import {
  Table,
  ActionIcon,
  Paper,
  Text,
  Group,
  Box,
  LoadingOverlay,
} from "@mantine/core"
import {
  ColumnDef,
  createTable,
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  PaginationState,
  useReactTable,
} from "@tanstack/react-table"
import {
  ChevronLeftIcon,
  ChevronRightIcon,
  DoubleArrowLeftIcon,
  DoubleArrowRightIcon,
} from "@radix-ui/react-icons"
import { NavtableProps, Pagination } from "./navbar-props"
import { useMatch, useNavigate } from "react-router"
import { useTranslation } from "react-i18next"

export function usePagination(): Pagination {
  const paginationState = React.useState<PaginationState>({
    pageIndex: 0,
    pageSize: 10,
  })
  return { state: paginationState }
}

export function CustomTable<RecordType extends { id: string } = any>(
  props: NavtableProps<RecordType> & {
    selected?: RecordType
    onRowClick?: (record: RecordType) => void
  },
) {
  const [
    { pageIndex, pageSize },
    setPagination,
  ] = React.useState<PaginationState>({
    pageIndex: 0,
    pageSize: 10,
  })
  const itemCount = props.dataSource?.length || 0
  const pageCount = Math.floor((itemCount + 10) / pageSize)
  const canPreviousPage = pageIndex > 0
  const canNextPage = pageIndex < pageCount - 1
  function previousPage() {
    setPagination((v) => ({ ...v, pageIndex: v.pageIndex - 1 }))
  }
  function nextPage() {
    setPagination((v) => ({ ...v, pageIndex: v.pageIndex + 1 }))
  }
  const matchPattern =
    props.highlightMatchPattern ||
    props.listPath ||
    (props.path === "string" ? props.path : "") ||
    ""
  const match = useMatch(
    matchPattern.endsWith("/") ? matchPattern + ":id" : matchPattern,
  )
  const instance = useReactTable({
    data: props.dataSource || [],
    columns: props.columns
      .filter((v) => v.visible === undefined || v.visible === true)
      .map(
        (v, i) =>
          ({
            id: "" + i,
            header: (props) => "" + v.title,
            cell: (props) => v.render(props.row.original!),
            meta: { width: v.width },
          } as ColumnDef<RecordType>),
      ),
    getCoreRowModel: getCoreRowModel(),
    state:
      props.paginate === false
        ? undefined
        : {
            pagination: { pageIndex, pageSize },
          },
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    onPaginationChange: setPagination,
  })
  const navigate = useNavigate()
  const { navigateOnClickTo } = props
  const { t } = useTranslation()
  return (
    <Box sx={(theme) => ({ position: "relative" })}>
      <LoadingOverlay visible={props.loading || false} />
      <Paper withBorder={false} shadow="xs">
        <Table>
          <thead>
            {instance.getHeaderGroups().map((headerGroup) => {
              return (
                <tr
                  key={headerGroup.id}
                  style={props.responsive ? { display: "flex" } : undefined}
                >
                  {headerGroup.headers.map((header) => {
                    const meta = header.column.columnDef.meta as any
                    const { width } = meta
                    return (
                      <th
                        key={header.id}
                        colSpan={header.colSpan}
                        style={{ width }}
                        // width={width}
                      >
                        {/* <RenderObject meta={header.column.columnDef.meta} /> */}
                        {header.isPlaceholder
                          ? null
                          : flexRender(
                              header.column.columnDef.header,
                              header.getContext(),
                            )}
                      </th>
                    )
                  })}
                </tr>
              )
            })}
          </thead>
          <tbody>
            {instance.getRowModel().rows.map((row) => (
              <Box
                component="tr"
                key={row.id}
                onClick={
                  navigateOnClickTo || props.onRowClick
                    ? () => {
                        props.onRowClick && props.onRowClick(row.original!)
                        navigateOnClickTo &&
                          navigate(navigateOnClickTo(row.original!))
                      }
                    : undefined
                }
                sx={(theme) => ({
                  display: props.responsive ? "flex" : undefined,
                  cursor:
                    props.navigateOnClickTo || props.onRowClick
                      ? "pointer"
                      : undefined,
                  backgroundColor:
                    row.original?.id === match?.params.id
                      ? theme.colorScheme === "dark"
                        ? theme.colors.dark[3]
                        : theme.colors.gray[3]
                      : undefined,
                })}
              >
                {row.getVisibleCells().map((cell) => {
                  const meta = cell.column.columnDef.meta as any
                  const { width } = meta
                  return (
                    <td key={cell.id} width={width}>
                      {/* <RenderObject
                        column={cell.column}
                      /> */}
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext(),
                      )}
                    </td>
                  )
                })}
              </Box>
            ))}
          </tbody>

          {/* <tfoot>
          {instance.getFooterGroups().map((footerGroup) => (
            <tr key={footerGroup.id}>
              {footerGroup.headers.map((header) => (
                <th key={header.id} colSpan={header.colSpan}>
                  {header.isPlaceholder ? null : header.renderFooter()}
                </th>
              ))}
            </tr>
          ))}
        </tfoot> */}
        </Table>
        {/* <div className="h-4" /> */}
        {/* <button onClick={() => rerender()} className="border p-2">
        Rerender
      </button> */}
      </Paper>
      {props.paginate === false ? null : (
        <Group
          spacing="xs"
          m="xs"
          style={{
            display: "flex",
            flex: 1,
            justifyContent: "flex-end",
          }}
        >
          <ActionIcon
            onClick={() => setPagination((v) => ({ ...v, pageIndex: 0 }))}
            disabled={!canPreviousPage}
          >
            <DoubleArrowLeftIcon />
          </ActionIcon>
          <ActionIcon
            onClick={() => previousPage()}
            disabled={!canPreviousPage}
          >
            <ChevronLeftIcon />
          </ActionIcon>
          <ActionIcon onClick={() => nextPage()} disabled={!canNextPage}>
            <ChevronRightIcon />
          </ActionIcon>
          <ActionIcon
            onClick={() =>
              setPagination((v) => ({
                ...v,
                pageIndex: pageCount - 1,
              }))
            }
            disabled={!canNextPage}
          >
            <DoubleArrowRightIcon />
          </ActionIcon>
          <div>
            <Text>
              {t("Page")}{" "}
              <b>
                {pageIndex + 1} {t("of")} {pageCount}
              </b>
            </Text>
          </div>
        </Group>
      )}
    </Box>
  )
}

// export default function App() {
//   const [results, setResults] = React.useState([])
//   const [loading, setLoading] = React.useState(true)

//   const dataRows = React.useMemo(() => {
//     return results.map(
//       ({ name_first, name_last, birth_country, position, team_full }) => {
//         return {
//           firstName: name_first,
//           lastName: name_last,
//           birthCountry: birth_country,
//           position: position,
//           team: team_full,
//         }
//       },
//     )
//   }, [results])

//   const columns = React.useMemo(
//     () => [
//       {
//         Header: "First Name",
//         accessor: "firstName", // accessor is the "key" in the data
//       },
//       {
//         Header: "Last Name",
//         accessor: "lastName",
//       },
//       {
//         Header: "Country",
//         accessor: "birthCountry",
//       },
//       {
//         Header: "Position",
//         accessor: "position",
//       },
//       {
//         Header: "Team",
//         accessor: "team",
//       },
//     ],
//     [],
//   )

//   return (
//     <Container>
//       <LoadingOverlay visible={loading} />
//       <CustomTable columns={columns} data={dataRows} />
//     </Container>
//   )
// }
