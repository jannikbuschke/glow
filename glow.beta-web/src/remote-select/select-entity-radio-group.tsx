import * as React from "react"
import { useGlowQuery } from "../query/use-data"
import { Radio } from "formik-antd"
import { RadioProps } from "antd"
import { useField } from "formik"

interface Option {
  value: string | number
  label: React.ReactNode
}

export type SelectEntityPropsRadioGroup<T> = {
  url: string
  name: string
  map: (v: T) => Option
  onSelectEntity?: (v: T | null) => void
} & Omit<RadioProps, "value" | "name">

export function SelectEntityRadioGroup<T>({
  url,
  name,
  map,
  onSelectEntity,
  // onChange,
  ...restProps
}: SelectEntityPropsRadioGroup<T>) {
  const [{ result, setSearch }, {}] = useGlowQuery<T>(url, {
    count: 0,
    value: [],
  })

  const [{ value }] = useField(name)

  const options = React.useMemo(() => result.value.map(map), [result])

  React.useEffect(() => {
    if (value && typeof value === "string") {
      const item = result?.value?.find((v: any) => v.id == value) || null
      onSelectEntity && onSelectEntity(item)
    } else {
      onSelectEntity && onSelectEntity(null)
    }
  }, [options, value])

  return (
    <Radio.Group
      name={name}
      // onChange={(v) => {
      //   const id = v?.target?.value
      //   const item = result?.value?.find((v: any) => v.id == id) || null
      //   onSelectEntity && onSelectEntity(item)
      //   onChange && onChange(v)
      // }}
      {...restProps}
    >
      {options.map((v) => (
        <Radio.Button key={v.value} name={name} value={v.value}>
          {v.label}
        </Radio.Button>
      ))}
    </Radio.Group>
  )
}
