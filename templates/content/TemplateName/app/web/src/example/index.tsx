import * as React from "react"
import { useTypedAction, useTypedQuery } from "../ts-models/api"
import { Table, Button, PageHeader } from "antd"
import { MasterDetailView } from "glow-react/es/layouts/master-detail-view"
import { HighlightableRow } from "glow-react/es/antd/highlightable-row"
import { Card } from "glow-react/es/antd/styled-components"
import { useNavigate, useParams } from "react-router"
import { ErrorBanner, notifyError, notifySuccess, Space } from "glow-react"
import { PromisePopconfirm } from "glow-react/es/buttons/promise-popconfirm"
import { Formik } from "formik"
import { Form, Input, SubmitButton } from "formik-antd"
import { defaultPerson } from "../ts-models/TemplateName.Example"
import styled from "styled-components"

export function Example() {
  return (
    <MasterDetailView
      path="/person/"
      create={<Create />}
      master={<Master />}
      detail={<Detail />}
      masteDetailContainerStyle={{ gridGap: 10 }}
    />
  )
}

function Create() {
  const [create, validate] = useTypedAction("/api/person/create")
  const navigate = useNavigate()
  return (
    <div>
      <Formik
        initialValues={{ name: "" }}
        enableReinitialize={true}
        validate={validate}
        onSubmit={async (values) => {
          const response = await create(values)
          if (response.ok) {
            notifySuccess("Success")
            navigate(`/person/${response.payload.id}`, { replace: true })
          } else {
            notifyError(response.error)
          }
        }}
      >
        <Form>
          <Card>
            <Header
              title="Create person"
              extra={
                <Space>
                  <SubmitButton>Create</SubmitButton>
                </Space>
              }
            />
            <br />
            <Input name="name" placeholder="Name" />
          </Card>
        </Form>
      </Formik>
    </div>
  )
}

const Header = styled(PageHeader)`
  padding: 0;
`

function Detail() {
  const { id } = useParams()
  const [update, validate] = useTypedAction("/api/person/update")
  const [deletePerson] = useTypedAction("/api/person/delete")
  const { data } = useTypedQuery("/api/person/get-single", {
    input: { id },
    placeholder: defaultPerson,
  })
  const navigate = useNavigate()

  return (
    <Formik
      initialValues={data}
      enableReinitialize={true}
      validate={validate}
      onSubmit={async (values) => {
        const response = await update(values)
        if (response.ok) {
          notifySuccess("Success")
        } else {
          notifyError(response.error)
        }
      }}
    >
      <Form>
        <Card
          title="Update"
          extra={
            <Space>
              <PromisePopconfirm
                title="Delete person"
                key="delete"
                onConfirm={async () => {
                  const response = await deletePerson({ id })
                  if (response.ok) {
                    notifySuccess("Success")
                    navigate("..", { replace: true })
                  } else {
                    notifyError(response.error)
                  }
                }}
              >
                <Button>Delete</Button>
              </PromisePopconfirm>
              <SubmitButton key="update">Update</SubmitButton>
            </Space>
          }
        >
          <Input name="name" placeholder="Name" />
        </Card>
      </Form>
    </Formik>
  )
}

export function Master() {
  const { data, error, loading, refetch } = useTypedQuery(
    "/api/person/get-list",
    {
      input: {},
      placeholder: [],
    },
  )
  const navigate = useNavigate()
  return (
    <div>
      <ErrorBanner error={error} />
      <Card
        title="Persons"
        extra={
          <Space>
            <Button onClick={() => navigate("/person/create")}>Create</Button>
            <Button
              onClick={() => {
                refetch()
              }}
            >
              Reload
            </Button>
          </Space>
        }
      >
        <Table
          loading={loading}
          size="small"
          pagination={false}
          dataSource={data}
          rowKey={(row) => row.id!}
          onRow={(row) => ({
            onClick: () => {
              navigate(`/person/${row.id}`)
            },
          })}
          components={{
            body: {
              row: (props: any) => (
                <HighlightableRow path="/person/" {...props} />
              ),
            },
          }}
          columns={[
            // {
            //   key: "id",
            //   dataIndex: "name",
            //   title: "Id",
            // },
            { key: "name", dataIndex: "name", title: "Name" },
          ]}
        />
      </Card>
    </div>
  )
}
