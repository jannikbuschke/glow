import * as React from "react"
import { notification, Popconfirm } from "antd"
import { useAction } from "glow-core"
import { notifyError } from "glow-core"
import { PopconfirmProps } from "antd/lib/popconfirm"
import { ActionProps } from "./shared"

export * from "./promise-button"
export * from "./promise-popconfirm"

export type ActionPopconfirmProps<Input, Output> = Omit<
  PopconfirmProps,
  "onConfirm"
> &
  ActionProps<Input, Output>

export function ActionPopconfirm<Input, Output>({
  input,
  onSuccess,
  onErrorResult,
  onResult,
  children,
  ...props
}: ActionPopconfirmProps<Input, Output>) {
  const [submit, , { submitting }] = useAction<Input, Output>(props.url)

  return (
    <Popconfirm
      {...props}
      onConfirm={async () => {
        try {
          const response = await submit(input)
          onResult && onResult(response)
          if (response.ok) {
            onSuccess && onSuccess(response.payload)
          } else {
            onErrorResult && onErrorResult(response.error)
            !onErrorResult && notifyError(response.error)
          }
        } catch (E) {
          onErrorResult && onErrorResult(E)
          !onErrorResult &&
            notification.error({ message: "An error occured:" + E.toString() })
        }
      }}
    >
      {React.Children.map(children, (v) =>
        React.cloneElement(v as any, { loading: submitting }),
      )}
    </Popconfirm>
  )
}
