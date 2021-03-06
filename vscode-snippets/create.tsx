import * as React from "react"
import { Route, useNavigate, Routes } from "react-router-dom"
import { PageHeader } from "antd"
import { useTranslation } from "react-i18next"
import {} from "@ant-design/icons"
import styled from "styled-components"
import { Formik } from "formik"
import { Form } from "formik-antd"
import { useSubmit } from "glow-react/es/Forms/use-submit"

interface Create$1 {}

export function $1CreateView() {
  const [submit, validate] = useSubmit("/api/$1/create")
  const navigate = useNavigate()
  return (
    <Container>
      <Formik<Create$1>
        initialValues={{}}
        validate={validate}
        onSubmit={submit}
      >
        <Form>{/* controls */}</Form>
      </Formik>
    </Container>
  )
}

const Header = styled(PageHeader)``

const Container = styled.div``