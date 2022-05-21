import * as React from "react"
import { Select, notification, Avatar } from "antd"
import { schema, normalize } from "normalizr"
import { useField, useFormikContext } from "formik"
import debounce from "lodash.debounce"
import { SelectProps } from "antd/lib/select"
import styled from "styled-components"
import { fetchJson } from "glow-core"

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
  const [field, ,] = useField(name)
  const form = useFormikContext()

  React.useEffect(() => {
    if (field.value) {
      const userId = setByReference ? field.value : field.value.id
      fetchJson<User>(`/api/user/${userId}`).then((v) => {
        setCurrentSelectedUser(v)
      })
    } else {
      setCurrentSelectedUser(null)
    }
  }, [field.value])

  const [dataSource, setDataSource] = React.useState<User[]>([])
  const [users, setUsers] = React.useState<{ [key: string]: User }>({})

  const [
    currentSelectedUser,
    setCurrentSelectedUser,
  ] = React.useState<User | null>(null)
  const extendedDataSource = React.useMemo(() => {
    if (currentSelectedUser !== null) {
      if (dataSource.some((v) => v.id === currentSelectedUser.id)) {
        return dataSource
      } else {
        return [
          ...dataSource,
          {
            displayName: currentSelectedUser.displayName || "no displayname",
            id: currentSelectedUser.id || "unknown",
            email: currentSelectedUser.email!,
          },
        ]
      }
    } else {
      return dataSource
    }
  }, [currentSelectedUser, dataSource])

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
        if (result.entities.users) {
          setUsers(result.entities.users)
        } else {
          console.error("could not normalize user data")
        }
      })()
    }, 150),
    [],
  )

  React.useEffect(() => {
    debouncedSearch("")
  }, [])

  return (
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
          if (!user) {
            return
          }
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
      {extendedDataSource.map((v) => (
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
