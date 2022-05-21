import * as React from "react"
import {} from "@ant-design/icons"
import { Formik } from "formik"
import { Checkbox, Form } from "formik-antd"
import { useSubmit } from "glow-core"
import { Button, Modal, notification, PageHeader } from "antd"
import styled from "styled-components"
import { Card } from "../antd/styled-components"

interface ResetDatabase {
  deleteDatabase: boolean
  iKnowWhatIAmDoing: boolean
}

export function ResetDatabaseDetailView() {
  const [send, validate] = useSubmit("/api/glow/db/reset-database")
  return (
    <Container>
      <Formik<ResetDatabase>
        initialValues={{ deleteDatabase: false, iKnowWhatIAmDoing: false }}
        validate={validate}
        onSubmit={async (values) => {
          const response = await send(values)
          if (response.ok) {
            notification.success({ message: "success" })
          } else {
            notification.error({
              message: "error: " + response.error.title + response.error.detail,
            })
          }
        }}
      >
        {(f) => (
          <Form>
            <PageHeader title="Reset DB" style={{ padding: 0 }} />

            <Card>
              <Form.Item name="deleteDatabase" label="Delete database">
                <Checkbox name="deleteDatabase" />
              </Form.Item>
              <Form.Item
                name="iKnowWhatIAmDoing"
                label="I know what I am doing"
              >
                <Checkbox name="iKnowWhatIAmDoing" />
              </Form.Item>
              <Button
                loading={f.isSubmitting}
                onClick={() => {
                  Modal.confirm({
                    title: "Do you really want to reset the database?",
                    content: f.values.deleteDatabase
                      ? "The database will be deleted"
                      : "The database will not be deleted",
                    onOk: () => f.submitForm(),
                  })
                }}
              >
                {f.isSubmitting
                  ? "Resetting database...."
                  : "Verify and Delete database"}
              </Button>
            </Card>
          </Form>
        )}
      </Formik>
    </Container>
  )
}

const Container = styled.div``
