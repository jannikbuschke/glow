import { Input, SubmitButton } from "formik-antd"
import * as React from "react"
import styled from "styled-components"
import { DefaultForm } from "../Forms/default-form"
import { VerticalSpace } from "../Layout"

export function UpdateOpenidConnectOptionsView() {
  return (
    <Container>
      <DefaultForm
        actionName="/api/glow/set-openid-connect-options"
        initialValues={{ clientId: "", clientSecret: "", tenantId: "" }}
        onSuccess={(payload) => {
          console.log({ payload })
        }}
      >
        <VerticalSpace>
          <Input name="clientId" placeholder="ClientId" />
          <Input name="tenantId" placeholder="TenantId" />
          <Input.Password name="clientSecret" placeholder="ClientSecret" />
          <SubmitButton>Submit</SubmitButton>
        </VerticalSpace>
      </DefaultForm>
    </Container>
  )
}

const Container = styled.div`
  padding: 40px;
`
