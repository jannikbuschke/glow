import { Input } from "formik-antd"
import * as React from "react"
import { useNavigate, useParams } from "react-router"
import {
  ErrorBanner,
  InternalTable,
  notifyError,
  Space,
  VerticalSpace,
} from "glow-core"
import { Form, SubmitButton } from "formik-antd"
import { Formik } from "formik"
import { useTypedAction, useTypedQuery } from "../../ts-models/api"
import { HighlightableRow } from "glow-core/es/antd/highlightable-row"
import { PromiseButton } from "glow-core/es/buttons/promise-button"
import { defaultMdxViewmodel } from "../../ts-models/Glow.Sample.MdxBundle"
import { MasterDetailView } from "glow-core/es/layouts/master-detail-view"
import { getMDXComponent } from "mdx-bundler/client"
import dayjs from "dayjs"

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
  const { data } = useTypedQuery("/api/mdx/get-single", {
    input: { id: params.id },
    placeholder: defaultMdxViewmodel,
  })
  // todo: add frontmatter
  const [update] = useTypedAction("/api/mdx/update")
  const [transpile] = useTypedAction("/api/mdx/transpile")

  const [code, setCode] = React.useState<string | null>(null)
  const [frontmatter, setFrontmatter] = React.useState<{
    [key: string]: string
  }>({})
  const Component = React.useMemo(() => getMDXComponent(code || ""), [code])

  React.useEffect(() => {
    console.log("EFFECT")

    if (data.content === null) {
      return
    }
    transpile({ source: data.content }).then((v) => {
      if (v.ok) {
        const { code, frontmatter } = v.payload
        setCode(code)
        setFrontmatter(frontmatter)
        // could set Component from here
      } else {
        notifyError(v.error)
      }
    })
  }, [data])
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
          {(f) => (
            <Form>
              <VerticalSpace>
                <Input name="path" />
                <Input.TextArea name="content" rows={25} />
                <Space>
                  <SubmitButton>Save</SubmitButton>
                  <PromiseButton
                    onClick={async () => {
                      transpile({ source: f.values.content }).then((v) => {
                        if (v.ok) {
                          const { code, frontmatter } = v.payload
                          setCode(code)
                          setFrontmatter(frontmatter)
                          // could set Component from here
                        } else {
                          notifyError(v.error)
                        }
                      })
                    }}
                  >
                    Transpile
                  </PromiseButton>
                </Space>
              </VerticalSpace>
            </Form>
          )}
        </Formik>
      </div>
      <div>
        {/* <div>
          {frontmatter &&
            frontmatter["last-update"] &&
            dayjs(frontmatter["last-update"]).format("LLLL")}
        </div> */}
        <div>Author: {frontmatter && frontmatter["author"]}</div>
        <br />
        {Component ? <Component /> : <div>undefined {typeof Component}</div>}
      </div>
    </>
  )
}

function List() {
  const navigate = useNavigate()
  const { data, error } = useTypedQuery("/api/mdx/get-list", {
    input: {},
    placeholder: [],
  })
  const listPath = constants.path
  const path = constants.path
  const [createFile] = useTypedAction("/api/mdx/create")
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
