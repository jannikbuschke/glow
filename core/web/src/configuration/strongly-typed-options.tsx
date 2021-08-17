import * as React from "react"
import { Formik } from "formik"
import { message, PageHeader, Button, Card, Tooltip, ColProps } from "antd"
import {
  Input,
  Switch,
  InputNumber,
  SubmitButton,
  Form,
  Table,
  AddRowButton,
  RemoveRowButton,
  FormikDebug,
} from "formik-antd"
import { useActions, badRequestResponseToFormikErrors } from "./validation"
import { useData } from "../query/use-data"
import { ErrorBanner } from "../errors/error-banner"
import styled from "styled-components"

function toType(
  type: string,
  name: string,
  isArray: boolean,
  disabled?: boolean,
) {
  switch (type) {
    case "string":
      return <Input fast={true} name={name} disabled={disabled} />
    case "number":
      return <InputNumber fast={true} name={name} disabled={disabled} />
    case "boolean":
      return <Switch fast={true} name={name} disabled={disabled} />
    case "object": {
      if (isArray) {
        return (
          <div>
            <AddRowButton
              name={name}
              style={{ marginBottom: 12 }}
              createNewRow={() => ""}
            >
              Add
            </AddRowButton>
            <Table
              name={name}
              columns={[
                {
                  render: (text, record, i) => (
                    <Row>
                      <Input
                        fast={true}
                        name={`${name}[${i}]`}
                        disabled={disabled}
                        // style={{ flex: 1 }}
                      />
                      <RemoveRowButton name={`${name}`} index={i}>
                        remove
                      </RemoveRowButton>
                    </Row>
                  ),
                },
              ]}
              size="small"
              showHeader={false}
              pagination={false}
              bordered={false}
              style={{ width: 600 }}
            />
          </div>
        )
      }
      return <ErrorBanner error={`Type '${type}' not supported`}></ErrorBanner>
    }
    default:
      return <ErrorBanner error={`Type '${type}' not supported`}></ErrorBanner>
  }
}

const Row = styled.div`
  display: flex;

  > *:not(:first-child) {
    margin-left: 1rem;
  }
`

type BaseProps = {
  title: string
  url: string
  configurationId: string
  allowEdit?: boolean
  name?: string
  disabled?: boolean
  containerStyles?: React.CSSProperties | undefined
  labelCol?: ColProps
  formikDebug?: boolean
}

type WithChildren = BaseProps & { type: "children"; children: React.ReactNode }
type WithEditors = BaseProps & {
  type?: "editors"
  overrideEditors?: { [key: string]: React.ReactNode }
}
type Props = WithChildren | WithEditors

export function StronglyTypedOptions({
  containerStyles,
  labelCol,
  title,
  url,
  configurationId,
  allowEdit = true,
  name = "",
  disabled,
  formikDebug,
  ...rest
}: Props) {
  const { submit } = useActions(url)
  const { data, error, refetch } = useData<any>(
    name ? url + "/" + name : url,
    {},
  )
  const overrideEditors =
    rest.type === "editors" || rest.type === undefined
      ? rest.overrideEditors
      : undefined
  const children = rest.type === "children" ? rest.children : undefined
  return (
    <Card>
      <ErrorBanner error={error} />
      <Formik
        initialValues={data}
        enableReinitialize={true}
        onSubmit={async (values, actions) => {
          actions.setSubmitting(true)
          const r = await submit({ configurationId, value: values, name })
          actions.setSubmitting(false)
          if (r.ok) {
            refetch()
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
        {(f) => (
          <Form labelAlign="left" labelCol={{ xs: 4 }}>
            <PageHeader
              title={title}
              extra={[
                allowEdit && (
                  <SubmitButton disabled={!f.dirty} key="submit">
                    Save
                  </SubmitButton>
                ),
                <Button
                  key="refresh"
                  onClick={() => {
                    f.resetForm()
                    refetch()
                  }}
                >
                  Refresh
                </Button>,
              ]}
            >
              <br />
              <br />
              <div style={containerStyles ? containerStyles : undefined}>
                {children
                  ? children
                  : data &&
                    Object.keys(data).map((v) => (
                      <Form.Item
                        key={v}
                        name={v}
                        labelCol={labelCol ? labelCol : undefined}
                        htmlFor={name}
                        label={<b>{prettify(v)}</b>}
                        colon={false}
                        style={{ marginBottom: 5 }}
                      >
                        {overrideEditors && Boolean(overrideEditors[v]) ? (
                          <>{overrideEditors[v]}</>
                        ) : (
                          <>
                            {toType(
                              typeof data[v],
                              v,
                              Array.isArray(data[v]),
                              disabled,
                            )}
                          </>
                        )}
                      </Form.Item>
                    ))}
              </div>
            </PageHeader>
            {formikDebug && <FormikDebug />}
          </Form>
        )}
      </Formik>
    </Card>
  )
}

function prettify(val: string) {
  return capitalize(val.replace(/([a-z])([A-Z])/g, "$1 $2"))
}

function capitalize(s: string) {
  if (typeof s !== "string") return ""
  return s.charAt(0).toUpperCase() + s.slice(1)
}
