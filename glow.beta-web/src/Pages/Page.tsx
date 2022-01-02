import * as React from "react"
import { PageContainer, PageHeader } from "../Formik/Layout"

interface IPageProps {
  title?: string
  children: any
}

export const Page = (props: IPageProps) => (
  <PageContainer>
    {props.title && <PageHeader>{props.title}</PageHeader>}
    {props.children}
  </PageContainer>
)
