import dayjs from "dayjs"
import { Formik } from "formik"
import { Form, Input, SubmitButton } from "formik-antd"
import { RenderObject, VerticalSpace } from "glow-react"
import * as React from "react"
import { Route, Routes } from "react-router-dom"
import { TypedForm, useTypedQuery } from "../../ts-models/api"

export function AzureDevopsExample() {
  return (
    <Routes>
      <Route path="azdo" element={<VariableGroupView />}>
        {/* <Route path=":configurationId" element={<Detail />} /> */}
      </Route>
    </Routes>
  )
}

export function VariableGroupView() {
  const [projectName, setProjectName] = React.useState("")
  const { data, error, isLoading, refetch } = useTypedQuery(
    "/azdo/get-commits",
    {
      input: { projectName },
      placeholder: [],
      queryOptions: {
        initialData: [],
        enabled: Boolean(projectName),
      },
    },
  )
  return (
    <div
      style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: 10 }}
    >
      <div>
        <TypedForm
          actionName="/azdo/create-library"
          initialValues={{ projectName: "glue" }}
        >
          <VerticalSpace>
            <Input name="projectName" />
            <SubmitButton>create library</SubmitButton>
          </VerticalSpace>
        </TypedForm>
      </div>
      <div>
        <Formik
          initialValues={{ projectName: "glue" }}
          onSubmit={(values, actions) => {
            setProjectName(values.projectName)
            refetch()
            actions.setSubmitting(false)
          }}
        >
          <Form>
            <VerticalSpace>
              <Input name="projectName" />
              <SubmitButton>get commits</SubmitButton>
            </VerticalSpace>
          </Form>
        </Formik>
        {data.map((v) => (
          <div>
            <h1>{v.gitCommit.comment}</h1>
            <RenderObject {...v.changes} />
          </div>
        ))}
      </div>
      <div>
        <TypedForm
          actionName="/azdo/create-commit"
          initialValues={{ projectName: "glue", content: "" }}
        >
          <VerticalSpace>
            <Input name="projectName" placeholder="Project" />
            <Input name="content" placeholder="Content" />
            <SubmitButton>commit</SubmitButton>
          </VerticalSpace>
        </TypedForm>
      </div>
    </div>
  )
}
