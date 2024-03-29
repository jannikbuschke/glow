import * as React from "react"
import { Button, notification } from "antd"
import { ButtonProps } from "antd/lib/button"

export type PromiseButtonProps = Omit<ButtonProps, "onClick" | "loading"> & {
  onClick: () => Promise<void>
}

export function PromiseButton({ onClick, ...props }: PromiseButtonProps) {
  const [loading, setLoading] = React.useState(false)
  return (
    <Button
      loading={loading}
      onClick={async () => {
        setLoading(true)
        try {
          await onClick()
        } catch (E) {
          notification.error({ message: "An error occured:" + E.toString() })
        } finally {
          setLoading(false)
        }
      }}
      {...props}
    />
  )
}
