import * as React from "react"
import { Select, notification, Avatar } from "antd"
import { schema, normalize } from "normalizr"
import { Field, FieldProps } from "formik"
import debounce from "lodash.debounce"
import { SelectProps } from "antd/lib/select"
import styled from "styled-components"

export interface User {
  id: string | null
  email: string | null
  displayName: string | null
}

const user = new schema.Entity(
  "users",
  {},
  {
    idAttribute: "id",
  },
)

export function UserSelect<T = any>({
  disabled,
  name,
  fetcher,
  setByReference,
  ...restProps
}: {
  disabled?: boolean
  name: string
  setByReference?: boolean
  fetcher: (search: string) => Promise<User[]>
} & SelectProps<T>) {
  const [dataSource, setDataSource] = React.useState<User[]>([])
  const [users, setUsers] = React.useState<{ [key: string]: User }>({})

  const debouncedSearch = React.useCallback(
    debounce<(search: string) => void>((v) => {
      ;(async () => {
        const data = await fetcher(v)
        setDataSource(
          data.map((v) => ({
            displayName: v.displayName || "no displayname",
            id: v.id || "unknown",
            email: v.email!,
          })),
        )
        const result = normalize<User>(data, [user])
        setUsers(result.entities.users)
      })()
    }, 150),
    [],
  )

  React.useEffect(() => {
    debouncedSearch("")
  }, [])

  return (
    <Field name={name}>
      {({ field, form }: FieldProps<any>) => (
        <Select<any>
          disabled={disabled}
          dropdownMatchSelectWidth={false}
          value={
            field.value
              ? setByReference
                ? field.value
                : field.value.id !== null
                ? field.value.id
                : undefined
              : undefined
          }
          onChange={(value, option) => {
            if (value === undefined) {
              form.setFieldValue(name, null, true)
            }
          }}
          onBlur={() => {
            form.setFieldTouched(name)
          }}
          showSearch={true}
          onSelect={(v, optio): void => {
            const id = v.valueOf() as string
            if (!user) {
              notification.error({
                message: "There is an issue. Pls refresh and try again.",
              })
            } else {
              const user = users[id]
              if (setByReference) {
                form.setFieldValue(name, user.id)
              } else {
                form.setFieldValue(name + ".id", user.id, true)
                form.setFieldValue(name + ".displayName", user.displayName)
                form.setFieldValue(name + ".email", user.email)
              }
            }
          }}
          onSearch={async (search: any) => {
            debouncedSearch(search)
          }}
          filterOption={false}
          {...restProps}
        >
          {dataSource.map((v) => (
            <Select.Option key={v.id!} value={v.id!}>
              <Container>
                <Avatar
                  src={"/api/tops/user-avatar/" + v.id + "?api-version=2.0"}
                  size="small"
                />
                <Details>
                  <b>{v.displayName}</b>
                  <Email>{v.email}</Email>
                </Details>
              </Container>
            </Select.Option>
          ))}
        </Select>
      )}
    </Field>
  )
}

const Email = styled.span`
  margin-left: 4px;
`
const Details = styled.span`
  margin-left: 8px;
`
const Container = styled.span`
  display: flex;
  flex-direction: row;
  align-content: center;
  align-items: center;
`
