import * as React from "react"
import { Route, useNavigate, Routes } from "react-router-dom"
import { useParams } from "react-router"
import { PageHeader } from "antd"
import { useTranslation } from "react-i18next"
import {} from "@ant-design/icons"
import styled from "styled-components"
import { Formik } from "formik"
import { Form } from "formik-antd"
import { useSubmit } from "glow-react/es/Forms/use-submit"
import { useData } from "glow-react/es/query/use-data"

interface $1Dto {}

export function $1DetailView() {
  const { id } = useParams()
  const { data, status, error } = useData<$1Dto>(`/api/$1/${id}`, {})
  // const [update, validate] = useSubmit(\"/api/$1/update")
  const navigate = useNavigate()
  return <Container>{/* ... */}</Container>
}

const Header = styled(PageHeader)``

const Container = styled.div``