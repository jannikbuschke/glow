import * as React from "react"
import { ButtonProps } from "antd/lib/button"
import { Button, message } from "antd"
import { postJson } from "glow-core/es/actions/fetch"

export function ActionButton<Request = any, Response = {}>({
  path,
  payload,
  onSuccess,
  onError,
  ...props
}: ButtonProps & {
  payload: Request
  path: string
  onSuccess?: (value?: any) => void
  onError?: (e: any) => void
}) {
  const [loading, setLoading] = React.useState(false)
  return (
    <Button
      loading={loading && { delay: 150 }}
      onClick={async () => {
        setLoading(true)
        try {
          const response = await postJson<Response>(path, payload, {
            "x-submit-intent": "execute",
          })
          onSuccess && onSuccess(response)
        } catch (E) {
          onError ? onError(E.toString()) : message.error(E.toString())
        } finally {
          setLoading(false)
        }
      }}
      {...props}
    />
  )
}
