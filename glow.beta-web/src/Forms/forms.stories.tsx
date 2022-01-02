import * as React from "react"
import { Formik } from "formik"
import { Form, Input, FormItem, DatePicker } from "formik-antd"

export default {
  title: "RemoteSelect",
}

const path = ""
const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
}

export const text = () => (
  <Formik initialValues={{ value: "123" }} onSubmit={() => {}}>
    <Form style={{ maxWidth: 1000 }} {...layout}>
      <div
        style={{
          margin: 120,
          display: "grid",
          gridGap: "10px 15px",
          gridTemplateColumns: "1fr 1fr",
        }}
      >
        <Form.Item name="value" label="Invoice">
          <Input name="value" />
        </Form.Item>
        <Form.Item name="value" label="Invoice">
          <Input name="value" />
        </Form.Item>
        <FormItem name={path + "invoiceId"} label="Invoice">
          <Input name={path + "invoiceId"} placeholder="Invoice Id" />
        </FormItem>
        <FormItem name={path + "projects"} label="Projects">
          <Input name={path + "projects"} placeholder="Projects" />
        </FormItem>
        <FormItem name={path + "personResponsible"} label="Responsible">
          <Input name={path + "personResponsible"} placeholder="Responsible" />
        </FormItem>
        <FormItem name={path + "duration"} label="Duration">
          <Input name={path + "duration"} placeholder="Duration" />
        </FormItem>

        <FormItem name={path + "created"} label="Created">
          <DatePicker name={path + "created"} style={{ width: "100%" }} />
        </FormItem>
      </div>
    </Form>
  </Formik>
)
