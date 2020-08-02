import * as React from "react"
import { Button } from "antd"
import { ButtonProps } from "antd/lib/button"

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
          } catch {
          } finally {
            setLoading(false)
          }
        }}
        {...props}
      />
    </>
  )
}
