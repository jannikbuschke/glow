import * as React from "react"

import { useFormikContext } from "formik"
import debounce from "lodash.debounce"

interface IAutoSaveContext {
  pause: (pause: boolean) => void
}

export const AutoSaveContext = React.createContext<IAutoSaveContext>({
  pause: () => {},
})

export function AutoSave({
  delayMs,
  pause,
}: {
  delayMs: number
  pause: boolean
}) {
  const ctx = useFormikContext()
  const debouncedSubmit = React.useCallback(
    debounce(() => {
      // this might not work
      if (ctx.dirty && !ctx.isSubmitting) {
        ctx.submitForm().then(() => {})
      }
    }, delayMs),
    [delayMs, ctx.dirty, ctx.submitForm, pause],
  )

  React.useEffect(() => {
    if (!pause) {
      debouncedSubmit()
    }
  }, [ctx.values, pause])

  return null
}

export function AutoSaveProvider({
  children,
}: {
  children: React.ReactElement | React.ReactElement[]
}) {
  const [pause, setPause] = React.useState(false)
  const value = React.useMemo(
    () => ({
      pause: (value: boolean) => setPause(value),
    }),
    [pause],
  )
  return (
    <AutoSaveContext.Provider value={value}>
      {children}
      <AutoSave delayMs={1000} pause={pause} />
    </AutoSaveContext.Provider>
  )
}

export function SimpleAutoSave({ delayMs }: { delayMs: number }) {
  const ctx = useFormikContext()
  const debouncedSubmit = React.useCallback(
    debounce(() => {
      // this might not work
      if (ctx.dirty && !ctx.isSubmitting) {
        ctx.submitForm().then(() => {})
      }
    }, delayMs),
    [delayMs, ctx.dirty, ctx.submitForm],
  )

  React.useEffect(() => {
    debouncedSubmit()
  }, [ctx.values])

  return null
}
