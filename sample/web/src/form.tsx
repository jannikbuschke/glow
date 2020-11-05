import * as React from "react"
import { Route, Routes } from "react-router-dom"
import {} from "@ant-design/icons"
import styled from "styled-components"
import { Formik } from "formik"
import { Form, FormikDebug, Input, SubmitButton } from "formik-antd"
import { useSubmit } from "glow-react/es/Forms/use-submit"
import { defaultCreateUser } from "./ts-models"
import { notification } from "antd"

export function FormExample() {
  return (
    <Routes>
      <Route path="form/*" element={<Sample />} />
    </Routes>
  )
}

function Sample() {
  const [createUser, validateCreateUser] = useSubmit("/api/form/create-user")
  return (
    <Container>
      <Formik
        initialValues={defaultCreateUser}
        validate={validateCreateUser}
        validateOnChange={false}
        validateOnBlur={true}
        onSubmit={async (values, f) => {
          notification.info({ message: "submit" })
          const response = await createUser(values)
          console.log({ response })
          f.setSubmitting(false)
        }}
      >
        <Form
          labelCol={{ xs: 8 }}
          style={{ display: "flex", flexDirection: "column" }}
        >
          <Form.Item name="displayName" label="Displayname" required={true}>
            <Input name="displayName" />
          </Form.Item>
          <Form.Item name="email" label="Email" required={true}>
            <Input name="email" />
          </Form.Item>
          <SubmitButton>Submit</SubmitButton>
          <FormikDebug />
        </Form>
      </Formik>
    </Container>
  )
}

const Container = styled.div`
  width: 800px;
`
