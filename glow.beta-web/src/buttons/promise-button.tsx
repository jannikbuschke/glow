import * as React from "react"
import { Button, notification } from "antd"
import { ButtonProps } from "antd/es/button"

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
        } catch (E: any) {
          notification.error({ message: "An error occured:" + E.toString() })
        } finally {
          setLoading(false)
        }
      }}
      {...props}
    />
  )
}
