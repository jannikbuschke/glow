import * as React from "react"
import { Menu, PageHeader, Tabs } from "antd"
import { RemoteSelect } from "./RemoteSelect"
import styled from "styled-components"
import { Formik } from "formik"

export default {
  title: "RemoteSelect",
}

export const text = () => (
  <Formik initialValues={{ value: "123" }} onSubmit={() => {}}>
    <RemoteSelect
      name="value"
      fetch={() => {
        return Promise.resolve([
          { id: 1, title: "hello world" },
          { id: 123, title: "hello 123" },
        ])
      }}
    />
  </Formik>
)
