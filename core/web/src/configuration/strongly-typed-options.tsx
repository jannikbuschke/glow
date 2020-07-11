import * as React from "react"
import { Formik } from "formik"
import { message, PageHeader, Button, Card } from "antd"
import { Input, Switch, InputNumber, SubmitButton, Form } from "formik-antd"
import { useActions, badRequestResponseToFormikErrors } from "./validation"
import { useData } from "../query/use-data"
import { ErrorBanner } from "../errors/error-banner"

function toType(type: string, name: string) {
  switch (type) {
    case "string":
      return <Input fast={true} name={name} />
    case "number":
      return <InputNumber fast={true} name={name} />
    case "boolean":
      return <Switch fast={true} name={name} />
    default:
      return <Input fast={true} name={name} />
  }
}

interface Props {
  title: string
  url: string
  configurationId: string
  allowEdit?: boolean
}

export function StronglyTypedOptions({
  title,
  url,
  configurationId,
  allowEdit = true,
}: Props) {
  const { submit } = useActions(url)
  const { data, error, refetch } = useData<any>(url, {})
  return (
    <Card>
      <ErrorBanner error={error} />
      <Formik
        initialValues={data}
        enableReinitialize={true}
        onSubmit={async (values, actions) => {
          console.log("submit")
          actions.setSubmitting(true)
          const r = await submit({ configurationId, value: values })
          actions.setSubmitting(false)
          if (r.ok) {
            message.success("success")
          } else {
            if (r.status === 400) {
              const errors = await r.json()
              actions.setErrors(
                (badRequestResponseToFormikErrors(errors) as any).value,
              )
            } else {
              message.error(r.statusText)
            }
          }
        }}
      >
        <Form>
          <PageHeader
            title={title}
            extra={[
              allowEdit && <SubmitButton key="submit">Save</SubmitButton>,
              <Button
                key="refresh"
                onClick={() => {
                  refetch()
                }}
              >
                Refresh
              </Button>,
            ]}
          >
            <div
              style={{
                display: "grid",
                gridTemplateColumns: "140px auto",
              }}
            >
              {data &&
                Object.keys(data).map((v) => (
                  <>
                    <label
                      style={{
                        marginTop: 10,
                        marginRight: 10,
                        textAlign: "right",
                      }}
                    >
                      {v}
                    </label>
                    <Form.Item name={v}>{toType(typeof data[v], v)}</Form.Item>
                  </>
                ))}
            </div>
          </PageHeader>
        </Form>
      </Formik>
    </Card>
  )
}
