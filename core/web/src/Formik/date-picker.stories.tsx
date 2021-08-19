import * as React from "react"
import { Formik } from "formik"
import { Form, FormikDebug } from "formik-antd"
import DatePickerCustom from "./date-picker-custom/date-picker-custom"

export default {
  title: "Date Picker",
}

export const Default = () => (
  <Formik
    initialValues={{
      date: null,
    }}
    onSubmit={() => {}}
  >
    <Form style={{ padding: 120 }}>
      <Form.Item name="date">
        <DatePickerCustom name="date" showTime={true} />
      </Form.Item>

      <FormikDebug />
    </Form>
  </Formik>
)
