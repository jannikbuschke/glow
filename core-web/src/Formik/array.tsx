import * as React from "react"
import { FieldArray, FieldArrayRenderProps, useField } from "formik"

export function MyFieldArray<RecordType = any>({
  name,
  renderItem,
  renderNewPlaceholder,
}: {
  name: string
  renderNewPlaceholder: boolean
  renderItem: (
    i: number,
    isLast: boolean,
    array: FieldArrayRenderProps,
    path: string,
    item?: RecordType,
  ) => React.ReactNode
}) {
  const [field] = useField<RecordType[]>(name)

  return (
    <FieldArray name={name}>
      {(array) => {
        const value = field.value || []
        if (renderNewPlaceholder) {
          return value
            .map((_: any, i: number) =>
              renderItem(i, false, array, name + "." + i + "."),
            )
            .concat([
              renderItem(
                value.length,
                true,
                array,
                name + "." + value.length + ".",
              ),
            ])
        } else {
          return value.map((value: any, i: number) =>
            renderItem(i, false, array, name + "." + i + ".", value),
          )
        }
      }}
    </FieldArray>
  )
}
