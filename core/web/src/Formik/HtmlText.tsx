import * as React from "react"
import { useField } from "formik"

export function HtmlText({ name }: { name: string }) {
  const [field] = useField(name)
  return field.value !== undefined && field.value !== null ? (
    <>{field.value}</>
  ) : null
}
