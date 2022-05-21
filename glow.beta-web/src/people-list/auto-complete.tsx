import * as React from "react"
import { AutoComplete, Input as $Input } from "antd"
import { Input } from "formik-antd"
import { schema, normalize } from "normalizr"
import { Field, FieldProps } from "formik"
// import debounce from "lodash.debounce"
import { useTranslation } from "react-i18next"
// import useSWR from "swr"
import { InputProps } from "antd/lib/input"
import { AutoCompleteProps } from "antd/lib/auto-complete"

const user = new schema.Entity("users")

interface User {
  displayName: string
  givenName: string | null
  id: string
  mail: string | null
  surname: string | null
  userPrincipalName: string
}

export function UserName({
  path,
  disabled,
  dataTestId,
  ...rest
}: {
  dataTestId: string
  path: string
  disabled?: boolean
} & InputProps) {
  const { t } = useTranslation()
  const [search, setSearch] = React.useState("")
  const [dataSource, setDataSource] = React.useState<
    { value: string; text: string }[]
  >([])
  const [users, setUsers] = React.useState<{ [key: string]: User }>({})

  // const { data, error } = useSWR(
  //   "/api/tops/user-search?api-version=3.0&search=" + search,
  // )
  const data = [] as any

  React.useEffect(() => {
    if (!data) {
      return
    }
    const data2 = data as User[]
    const dataSource = data2.map((v) => ({
      value: v.id,
      text:
        (v.displayName ? v.displayName : v.surname + " " + v.givenName) +
        (v.mail ? " (" + v.mail + ")" : ""),
    }))
    setDataSource(dataSource)
    const result = normalize<User>(data2, [user])
    setUsers(result.entities.users!)
  }, [data])

  // const debouncedSearch = React.useCallback(
  //   debounce<(search: string) => void>((v: any) => {
  //     setSearch(v)
  //   }, 150),
  //   [],
  // )

  return (
    <Field name={path + "name"}>
      {({ field, form }: FieldProps<any>) => (
        <AutoComplete
          data-test-id={dataTestId + "-name"}
          disabled={disabled}
          placeholder={t("userPlaceholder")}
          dataSource={dataSource}
          dropdownMatchSelectWidth={false}
          value={field.value}
          onChange={(value) => {
            const u = users ? users[value.toString()] : null
            if (u) {
              const firstName = u.surname
              const lastName = u.givenName
              if (firstName === null) {
                form.setFieldValue(path + "name", u.displayName)
              } else {
                const name =
                  firstName !== null
                    ? firstName + ", " + lastName
                    : u.displayName
                form.setFieldValue(path + "name", name)
                form.setFieldValue(path + "email", u.mail)
              }
            } else {
              // value is UserId if user clicks an autocomplete option
              form.setFieldValue(path + "name", value)
            }
          }}
          onSelect={(v) => {
            const u = users[v.toString()]
            if (!u) {
              return
            }
            const value = u.surname + ", " + u.givenName
            form.setFieldValue(path + "name", value)
            form.setFieldValue(path + "email", u.mail)
          }}
          onSearch={async (search: any) => {
            // debouncedSearch(search)
          }}
          style={{ width: "100%" }}
        >
          <$Input {...rest} />
        </AutoComplete>
      )}
    </Field>
  )
}

type Props = InputProps & {
  dataTestId: string
  disabled: boolean
  path: string
}

export function Email({ dataTestId, disabled, path, ...rest }: Props) {
  const { t } = useTranslation()

  return (
    <Input
      data-test-id={dataTestId + "-email"}
      disabled={disabled}
      name={path + "email"}
      placeholder={t("emailPlaceholder")}
      {...rest}
    />
  )
}
