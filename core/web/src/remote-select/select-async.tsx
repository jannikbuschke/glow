import * as React from "react"
import { Select } from "antd"
import { schema, normalize } from "normalizr"
import { useField } from "formik"
import debounce from "lodash.debounce"
import { SelectProps, SelectValue } from "antd/lib/select"
import { LabeledValue } from "antd/lib/tree-select"

export type SelectAsyncProps<T> = {
  name: string
  fetcher: (search: string) => Promise<LabeledValue[]>
} & SelectProps<T>

export function SelectAsync<T extends SelectValue>({
  disabled,
  name,
  fetcher,
  onChange,
  ...restProps
}: SelectAsyncProps<T>) {
  const [options, setOptions] = React.useState<LabeledValue[]>([])
  const debouncedSearch = React.useCallback(
    debounce((v: any) => {
      ;(async () => {
        const data = await fetcher(v)
        setOptions(data)
      })()
    }, 150),
    [fetcher],
  )
  React.useEffect(() => {
    debouncedSearch("")
    // TODO
    // on firstRender without full value
    // get single element
  }, [])
  const [field, _, form] = useField<any>(name)
  const value = field.value
  return (
    <Select<T>
      dropdownMatchSelectWidth={false}
      value={field.value}
      onChange={(value, o) => {
        if (value === undefined) {
          form.setValue(null)
        }
        onChange && onChange(value, o)
      }}
      onBlur={() => {
        form.setTouched(true)
      }}
      style={{ width: "100%", ...restProps?.style }}
      showSearch={true}
      onDeselect={(v) => {
        if (restProps.mode === "multiple") {
          form.setValue((value as any[]).filter((item) => item != v))
        }
      }}
      onSelect={(v) => {
        if (v === undefined) {
          return
        }
        if (restProps.mode === "multiple") {
          form.setValue([...(value || []), v])
        } else {
          form.setValue(v)
        }
      }}
      onSearch={(search: string) => {
        debouncedSearch(search)
      }}
      filterOption={false}
      {...restProps}
    >
      {options.map((v, i) => (
        <Select.Option key={v.key || v.value} value={v.value}>
          {v.label}
        </Select.Option>
      ))}
    </Select>
  )
}

const entitySchema = new schema.Entity<LabeledValue>(
  "values",
  {},
  {
    idAttribute: "value",
  },
)
