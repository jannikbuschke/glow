import { Button, notification, Table } from "antd"
import { Input } from "formik-antd"
import * as React from "react"
import { Route, Routes, useNavigate, useParams } from "react-router"
import {
  ErrorBanner,
  InternalTable,
  notifyError,
  SelectAsync as RemoteSelect,
  VerticalSpace,
} from "glow-react"
import { Form, FormikDebug, SubmitButton } from "formik-antd"
import { Formik } from "formik"
import { useTypedAction, useTypedQuery } from "../../ts-models/api"
import { HighlightableRow } from "glow-react/es/antd/highlightable-row"
import { PromiseButton } from "glow-react/es/buttons/promise-button"
import { defaultSourceFileViewmodel } from "../../ts-models/Glow.Sample.NodeRuntime"
import { MasterDetailView } from "glow-react/es/layouts/master-detail-view"
import { getMDXComponent } from "mdx-bundler/client"

export const constants = {
  path: "/mdx-bundle/",
  detail: "/mdx-bundle/:id",
}

export function MdxBundleExample() {
  return (
    <MasterDetailView
      masteDetailContainerStyle={{
        gridTemplateColumns: "200px 1fr 1fr",
        gridGap: 16,
      }}
      detail={<Detail />}
      master={<List />}
      path="/mdx-bundle/"
    />
  )
}

function Detail() {
  const params = useParams()
  const { data } = useTypedQuery("/api/source-file/get-single", {
    input: { id: params.id },
    placeholder: defaultSourceFileViewmodel,
  })
  // todo: add frontmatter
  const [update] = useTypedAction("/api/source-file/update")
  const Component = React.useMemo(() => getMDXComponent(data.code || ""), [
    data.code,
  ])
  return (
    <>
      <div>
        <Formik
          initialValues={data}
          enableReinitialize={true}
          onSubmit={async (values) => {
            const response = await update(values)
            if (!response.ok) {
              notifyError(response.error)
            }
          }}
        >
          <Form>
            <VerticalSpace>
              <Input name="path" />
              <Input.TextArea name="content" rows={25} />
              <SubmitButton>Save</SubmitButton>
            </VerticalSpace>
          </Form>
        </Formik>
      </div>
      <div>
        {/* <div>{data.code}</div> */}
        {Component ? <Component /> : <div>undefined {typeof Component}</div>}
      </div>
    </>
  )
}

function List() {
  const navigate = useNavigate()
  const { data, error } = useTypedQuery("/api/source-file/get-list", {
    input: {},
    placeholder: [],
  })
  const listPath = constants.path
  const path = constants.path
  const [createFile] = useTypedAction("/api/source-file/create")
  return (
    <div>
      <ErrorBanner error={error} />
      <PromiseButton
        onClick={async () => {
          const response = await createFile({ content: "", path: "file.mdx" })
          if (response.ok) {
            navigate(path + "/" + response.payload.id)
          } else {
            notifyError(response.error)
          }
        }}
      >
        Create
      </PromiseButton>
      <InternalTable
        showHeader={false}
        pagination={false}
        elevated={true}
        rowKey={(row) => row.id}
        components={{
          body: {
            row: (props: any) => (
              <HighlightableRow path={listPath} {...props} />
            ),
          },
        }}
        onRow={(record) => ({
          onClick: () => navigate(path + "/" + record.id),
        })}
        dataSource={data}
        columns={[
          // { key: "id", dataIndex: "id" },
          { key: "path", dataIndex: "path" },
        ]}
      />
    </div>
  )
}
