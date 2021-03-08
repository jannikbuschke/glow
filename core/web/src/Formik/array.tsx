import * as React from "react"
import { FieldArray, FieldArrayRenderProps, useField } from "formik"

export function MyFieldArray({
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
  ) => React.ReactNode
}) {
  const [field] = useField(name)

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
          return value.map((_: any, i: number) =>
            renderItem(i, false, array, name + "." + i + "."),
          )
        }
      }}
    </FieldArray>
  )
}
