import { useField } from "formik"
import { Select, SelectProps } from "formik-antd"
import * as React from "react"

const EnterKeyCode = 13

export type StringArrayMultiSelectProps = Omit<
  SelectProps,
  "mode" | "onKeyDown" | "searchValue" | "onSearch" | "labelInValue"
>

export function StringArrayMultiSelect(props: StringArrayMultiSelectProps) {
  const [search, setSearch] = React.useState("")
  const [field, , form] = useField<any>(props.name)
  const value = field.value
  return (
    <Select
      {...props}
      labelInValue={false}
      mode="multiple"
      onKeyDown={(e) => {
        if (e.keyCode === EnterKeyCode) {
          setSearch("")
          form.setValue([...(value || []), search])
        }
      }}
      searchValue={search}
      onSearch={(v) => {
        setSearch(v)
      }}
      dropdownStyle={{ visibility: "hidden" }}
    />
  )
}
