import Button, { ButtonProps } from "antd/es/button"
import * as React from "react"
import { useNotify } from "glow-core"
import { Result, useSubmit } from "glow-core"

export type ActionButtonProps<T> = {
  url: string
  payload: T
  onResponse?: (result: Result<T>) => void
} & Omit<ButtonProps, "onClick" | "loading">

export function ActionButton<T = any>(props: ActionButtonProps<T>) {
  const { url, payload, onResponse, ...buttonProps } = props
  const [submit, validate, { error, submitting }] = useSubmit(url)
  const { notifyError } = useNotify()
  return (
    <Button
      {...buttonProps}
      loading={submitting}
      onClick={async () => {
        const response = await submit(payload)
        if (onResponse) {
          onResponse(response)
        } else if (!response.ok) {
          notifyError(response.error)
        }
      }}
    />
  )
}
