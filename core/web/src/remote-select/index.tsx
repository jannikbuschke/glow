import * as React from "react"
import { Select } from "antd"
import { schema, normalize } from "normalizr"
import { useField } from "formik"
import debounce from "lodash.debounce"
import styled from "styled-components"
import { SelectValue } from "antd/lib/select"

const entitySchema = new schema.Entity(
  "values",
  {},
  {
    idAttribute: "id",
  },
)

export function RemoteSelect<T extends SelectValue>({
  disabled,
  name,
  fetcher,
  setByReference,
  renderItem,
  ...restProps
}: {
  disabled?: boolean
  name: string
  fetcher: (search: string) => Promise<any>
  setByReference: boolean
  renderItem: (item: any, i: number) => JSX.Element
}) {
  // var { disabled, name, fetcher, setByReference, renderItem } = _a, restProps = __rest(_a, ["disabled", "name", "fetcher", "setByReference", "renderItem"]);
  const [dataSource, setDataSource] = React.useState<any[]>([])
  const [users, setUsers] = React.useState<any>({})
  const debouncedSearch = React.useCallback(
    debounce((v: any) => {
      ;(async () => {
        const data = await fetcher(v)
        setDataSource(data)
        const result = normalize(data, [entitySchema])
        setUsers(result.entities.values)
      })()
    }, 150),
    [],
  )
  React.useEffect(() => {
    debouncedSearch("")
  }, [])
  const [field, _, form] = useField(name)
  return (
    <Select<T>
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
      onChange={(value) => {
        if (value === undefined) {
          form.setValue(null)
        }
      }}
      onBlur={() => {
        form.setTouched(true)
      }}
      showSearch={true}
      onSelect={(v, optio) => {
        console.log("selected", { v, optio })
        if (v) {
          const id = v.valueOf() as string
          const user = users[id]
          if (setByReference) {
            form.setValue(id)
          } else {
            form.setValue(user)
            // form.setValue(name + ".id", user.id)
            // form.setValue(name + ".displayName", user.displayName)
            // form.setValue(name + ".email", user.mail)
          }
        }
      }}
      onSearch={(search: string) => {
        debouncedSearch(search)
      }}
      filterOption={false}
      {...restProps}
    >
      {dataSource.map((v, i) => (
        <Select.Option key={v.id} value={v.id}>
          {renderItem(v, i)}
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
