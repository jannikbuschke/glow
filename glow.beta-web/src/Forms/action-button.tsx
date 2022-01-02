import Button, { ButtonProps } from "antd/lib/button"
import * as React from "react"
import { notifyError } from "glow-core"
import { Result, useSubmit } from "glow-core/es/actions/use-submit"

export type ActionButtonProps<T> = {
  url: string
  payload: T
  onResponse?: (result: Result<T>) => void
} & Omit<ButtonProps, "onClick" | "loading">

export function ActionButton<T = any>(props: ActionButtonProps<T>) {
  const { url, payload, onResponse, ...buttonProps } = props
  const [submit, validate, { error, submitting }] = useSubmit(url)

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
