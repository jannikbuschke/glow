import * as React from "react"
import { Button, notification, Popconfirm } from "antd"
import { ButtonProps } from "antd/lib/button"
import { PopconfirmProps } from "antd/lib/popconfirm"

type Props = Omit<ButtonProps, "onClick"> & { onClick: () => Promise<void> }

export function PromiseButton({ onClick, ...props }: Props) {
  const [loading, setLoading] = React.useState(false)
  return (
    <>
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
    </>
  )
}

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
        } catch (E) {
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
