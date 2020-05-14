import * as React from "react"
import { IEntityItem } from "./types"
import { Routes, Route } from "react-router-dom"

interface Props {
  children: React.ReactNode
  templateColumns?: string
}

export const HorizontalSplit = ({
  templateColumns = "1fr 1fr",
  children,
}: Props) => (
  <div
    style={{
      display: "grid",
      gridGap: "20px",
      gridTemplateColumns: templateColumns,
    }}
  >
    {children}
  </div>
)

export const MasterDetailView = ({ item }: { item: IEntityItem }) => (
  <Routes>
    <Route path={item.path} element={<item.list />}>
      {/* <Route path={"create"} element={<item.create />} /> */}
      <Route path={"new"} element={<item.create />} />
      <Route path=":id" element={<item.detail />} />
    </Route>
    {/* <Route path={item.path} element={<item.list />} /> */}
  </Routes>
)
