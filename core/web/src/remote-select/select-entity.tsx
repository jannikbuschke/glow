import * as React from "react"
import { Select } from "antd"
import { useField } from "formik"
import debounce from "lodash.debounce"
import { SelectProps } from "antd/lib/select"
import { LabeledValue } from "antd/lib/tree-select"
import { useGlowQuery } from "../query/use-data"

export type SelectEntityProps<T> = {
  url: string
  name: string
  map: (v: T) => LabeledValue
  customItems?: LabeledValue[]
} & Omit<SelectProps<LabeledValue>, "fetcher">

export function SelectEntity<T>({
  url,
  name,
  map,
  onSearch,
  customItems,
  ...restProps
}: SelectEntityProps<T>) {
  const [{ result, setSearch, setWhere, sendQuery }, {}] = useGlowQuery<T>(
    url,
    {
      count: 0,
      value: [],
    },
  )

  const searchOptions = React.useMemo(() => result.value.map(map), [result])
  const [fieldValue, setFieldValue] = React.useState<LabeledValue | null>(null)

  const options = React.useMemo(() => {
    return [
      ...searchOptions,
      ...(fieldValue && !searchOptions.some((v) => v.key == fieldValue.key)
        ? [fieldValue]
        : []),
      ...(customItems !== null && customItems !== undefined ? customItems : []),
    ]
  }, [searchOptions, fieldValue, customItems])

  const debouncedSearch = React.useCallback(
    debounce((v: any) => {
      ;(async () => {
        setSearch(v)
      })()
    }, 150),
    [],
  )

  const [field, , form] = useField<any>(name)
  const value = field.value
  React.useEffect(() => {
    debouncedSearch("")
    if (value && restProps.mode !== "multiple") {
      sendQuery(
        {
          where: { operation: "Equals", property: "Id", value },
          search: null,
          count: null,
          orderBy: null as any,
          skip: null,
          take: null,
        },
        url,
      )
        .then((v) => {
          if (v.value.length === 1) {
            setFieldValue(v.value.map(map)[0])
          }
        })
        .catch((v) => {
          console.error(v)
        })
    }
  }, [value])
  return (
    <Select<LabeledValue>
      dropdownMatchSelectWidth={false}
      value={field.value}
      onChange={(value) => {
        if (value === undefined) {
          form.setValue(null)
        }
      }}
      onBlur={() => {
        form.setTouched(true)
        debouncedSearch("")
      }}
      style={{ width: "100%", ...restProps?.style }}
      showSearch={true}
      onDeselect={(v) => {
        if (restProps.mode === "multiple") {
          if (v.value) {
            form.setValue(
              (value as any[]).filter((item) => item.value != v.value),
            )
          } else {
            form.setValue((value as any[]).filter((item) => item != v))
          }
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
        onSearch && onSearch(search)
      }}
      filterOption={false}
      {...restProps}
      id={name}
    >
      {options.map((v) => (
        <Select.Option key={v.key || v.value} value={v.value}>
          {v.label}
        </Select.Option>
      ))}
    </Select>
  )
}
