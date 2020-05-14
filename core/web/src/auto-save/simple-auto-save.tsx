import * as React from "react"
import { useFormikContext } from "formik"
import { useDebounce } from "use-lodash-debounce"

export function SimpleAutoSave({
  delayMs = 500,
  getId,
}: {
  delayMs?: number
  getId?: (values: any) => string
}) {
  const ctx = useFormikContext()
  const values = useDebounce(ctx.values, delayMs)

  const firstUpdate = React.useRef(true)

  React.useLayoutEffect(() => {
    if (values === null || values === undefined) {
      console.warn("values undefined")
      return
    }
    const id = getId ? getId(values) : (values as any).id
    if (!id) {
      console.warn("no identifier found")
      return
    }
    if (firstUpdate.current) {
      firstUpdate.current = false
      return
    } else {
      ctx.submitForm()
    }
  }, [values])

  return null
}
