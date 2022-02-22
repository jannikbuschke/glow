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
  allowClear?: boolean
} & Omit<RadioProps, "value" | "name">

export function SelectEntityRadioGroup<T>({
  url,
  name,
  map,
  onSelectEntity,
  allowClear,
  ...restProps
}: SelectEntityPropsRadioGroup<T>) {
  const [{ result, setSearch }, {}] = useGlowQuery<T>(url, {
    count: 0,
    value: [],
  })

  const [{ value }, , { setValue }] = useField(name)

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
    <Radio.Group name={name} {...restProps}>
      {options.map((v) => (
        <Radio.Button
          key={v.value}
          name={name}
          value={v.value}
          onClick={() => {
            if (allowClear && value === v.value) {
              setValue(null)
            }
          }}
        >
          {v.label}
        </Radio.Button>
      ))}
    </Radio.Group>
  )
}
