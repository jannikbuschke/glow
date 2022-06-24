import * as React from "react"
import { notification, Popconfirm } from "antd"
import { PopconfirmProps } from "antd/es/popconfirm"

export type PromisePopconfirmProps = Omit<PopconfirmProps, "onConfirm"> & {
  onConfirm: () => Promise<void>
}

export function PromisePopconfirm({
  onConfirm,
  children,
  ...props
}: PromisePopconfirmProps) {
  const [loading, setLoading] = React.useState(false)

  return (
    <Popconfirm
      {...props}
      onConfirm={async () => {
        setLoading(true)
        try {
          await onConfirm()
        } catch (E: any) {
          notification.error({ message: "An error occured:" + E.toString() })
        } finally {
          setLoading(false)
        }
      }}
    >
      {React.Children.map(children, (v) =>
        React.cloneElement(v as any, { loading }),
      )}
    </Popconfirm>
  )
}
