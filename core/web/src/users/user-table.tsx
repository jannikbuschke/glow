import * as React from "react"
import { Table, AddRowButton, RemoveRowButton } from "formik-antd"
import { User, UserSelect } from "./user-select"
import styled from "styled-components"

export function UserTable({
  name,
  fetcher,
}: {
  name: string
  fetcher: (search: string) => Promise<User[]>
}) {
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
                <UserSelect name={`${name}[${i}]`} fetcher={fetcher} />
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

const Row = styled.div`
  display: flex;

  > *:not(:first-child) {
    margin-left: 1rem;
  }
`
