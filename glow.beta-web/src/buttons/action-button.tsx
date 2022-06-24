import * as React from "react"
import { Button, notification } from "antd"
import { ButtonProps } from "antd/es/button"
import { Result, useAction } from "glow-core"
import { useNotify } from "glow-core"

export * from "./promise-button"
export * from "./promise-popconfirm"

type ActionProps<Input, Output> = {
  url: string
  input: Input
  onResult?: (result: Result<Output>) => void
  onSuccess?: (output: Output) => void
  onErrorResult?: (error: any) => void
}

type ActionButtonProps<Input, Output> = Omit<
  ButtonProps,
  "onClick" | "loading"
> &
  ActionProps<Input, Output>

export function ActionButton<Input, Output>({
  onResult,
  onSuccess,
  onErrorResult,
  ...props
}: ActionButtonProps<Input, Output>) {
  const input = props.input
  const [submit, validate, { submitting }] = useAction<Input, Output>(props.url)
  const { notifyError } = useNotify()
  return (
    <Button
      {...props}
      loading={submitting}
      onClick={async () => {
        try {
          const response = await submit(input)
          onResult && onResult(response)
          response.ok && onSuccess && onSuccess(response.payload)
          !response.ok && onErrorResult && onErrorResult(response.error)
          !response.ok && !onErrorResult && notifyError(response.error)
        } catch (E: any) {
          onErrorResult
            ? onErrorResult(E)
            : notification.error({
                message: "An error occured:" + E.toString(),
              })
        }
      }}
    />
  )
}
