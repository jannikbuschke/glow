import * as React from "react"
import { Formik } from "formik"
import { Form, FormikDebug } from "formik-antd"
import { SortableTable } from "./sortable-table"

export default {
  title: "Sortable table",
}

export const Default = () => (
  <Formik
    initialValues={{
      values: [
        { sortIndex: 0, id: "0", displayName: "hello" },
        { sortIndex: 1, id: "1", displayName: "world" },
        { sortIndex: 2, id: "2", displayName: "xxx" },
        { sortIndex: 3, id: "3", displayName: "yyyy" },
      ],
    }}
    onSubmit={() => {}}
  >
    <Form style={{ padding: 120 }}>
      <SortableTable
        name="values"
        rowKey={(row) => row.id}
        sortProperty="sortIndex"
        columns={[
          { key: "name", dataIndex: "displayName" },
          { key: "all", render: (row, record) => JSON.stringify(record) },
        ]}
      />
      <FormikDebug />
    </Form>
  </Formik>
)
