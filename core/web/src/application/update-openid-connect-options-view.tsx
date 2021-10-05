import { Formik } from "formik"
import { Input, SubmitButton, Form } from "formik-antd"
import * as React from "react"
import styled from "styled-components"
import { notifyError, notifySuccess } from "../errors/error-banner"
import { VerticalSpace } from "../Layout"
import { useTypedAction } from "../ts-models/api"

export function UpdateOpenidConnectOptionsView() {
  const [setSecret, validate] = useTypedAction(
    "/api/glow/set-openid-connect-options",
  )
  return (
    <Container>
      <Formik
        validate={validate}
        validateOnChange={false}
        validateOnBlur={true}
        initialValues={{ tenantId: "", clientId: "", clientSecret: "" }}
        onSubmit={async (values) => {
          const response = await setSecret(values)
          if (response.ok) {
            notifySuccess()
          } else {
            notifyError(response.error)
          }
        }}
      >
        <Form>
          <VerticalSpace>
            <Input name="clientId" placeholder="ClientId" />
            <Input name="tenantId" placeholder="TenantId" />
            <Input.Password name="clientSecret" placeholder="ClientSecret" />
            <SubmitButton>Submit</SubmitButton>
          </VerticalSpace>
        </Form>
      </Formik>
    </Container>
  )
}

const Container = styled.div`
  padding: 40px;
`
