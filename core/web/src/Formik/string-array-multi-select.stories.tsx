import * as React from "react"
import { Menu, PageHeader, Tabs } from "antd"
import { RemoteSelect } from "./RemoteSelect"
import styled from "styled-components"
import { Formik } from "formik"
import { StringArrayMultiSelect } from "./string-array-multi-select"
import { Form, FormikDebug } from "formik-antd"

export default {
  title: "StringArrayMultiSelect",
}

export const Default = () => (
  <Formik initialValues={{ values: ["hello", "world"] }} onSubmit={() => {}}>
    <Form>
      <StringArrayMultiSelect name="values" style={{ width: "100%" }} />
      <FormikDebug />
    </Form>
  </Formik>
)
