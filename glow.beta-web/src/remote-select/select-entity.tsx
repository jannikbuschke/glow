import * as React from "react"
import { Select } from "antd"
import { useField } from "formik"
import { useDebouncedCallback } from "use-lodash-debounce"
import { SelectProps } from "antd/lib/select"
import { LabeledValue } from "antd/lib/tree-select"
import { QueryParameter, useGlowQuery } from "../query/use-data"

export type SelectEntityProps<T> = {
  url: string
  name: string
  map: (v: T) => LabeledValue
  customItems?: LabeledValue[]
  initialParameters?: Partial<QueryParameter>
  onSelectEntity?: (v: T | null) => void
} & Omit<SelectProps<LabeledValue>, "fetcher">

export function SelectEntity<T>({
  url,
  name,
  map,
  onSearch,
  customItems,
  initialParameters,
  onSelect,
  onSelectEntity,
  ...restProps
}: SelectEntityProps<T>) {
  const [{ result, setSearch, setWhere, sendQuery }, {}] = useGlowQuery<T>(
    url,
    {
      count: 0,
      value: [],
    },
    undefined,
    initialParameters,
  )

  const entities = React.useMemo(() => {
    const newEntities = result.value.reduce(
      (prev, curr: any) => ({ ...prev, [curr.id]: curr }),
      {},
    )
    return newEntities
  }, [result])

  const [fieldValue, setFieldValue] = React.useState<LabeledValue | null>(null)

  const options = React.useMemo(() => {
    const searchOptions = result.value.map(map)
    const optionsResult = [
      ...searchOptions,
      ...(fieldValue && !searchOptions.some((v) => v.key == fieldValue.key)
        ? [fieldValue]
        : []),
      ...(customItems !== null && customItems !== undefined ? customItems : []),
    ]
    return optionsResult.filter(
      (v, i, a) => a.findIndex((v2) => v2.key === v.key) === i,
    )
  }, [result, fieldValue, customItems])

  const debouncedSearch = useDebouncedCallback(setSearch, 400)

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
            const x = v.value.map(map)[0]
            if (x) {
              setFieldValue(x)
            }
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
      onClear={() => {
        onSelectEntity && onSelectEntity(null)
      }}
      onSelect={(v, o) => {
        onSelect && onSelect(v, o)
        if (onSelectEntity) {
          if (v === undefined || v === null) {
            onSelectEntity(null)
          } else {
            const id = v as any
            const entity = entities[id]
            onSelectEntity(entity || null)
          }
        }
        if (v === undefined) {
          //
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
        <Select.Option key={v.key} value={v.value}>
          {v.label}
        </Select.Option>
      ))}
    </Select>
  )
}
