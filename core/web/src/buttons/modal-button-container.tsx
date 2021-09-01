import { Button, ButtonProps } from "antd"
import * as React from "react"

type Props = Omit<ButtonProps, "onClick"> & { modal: React.ReactElement }

export function ModalButtonContainer({ modal, ...props }: Props) {
  const [visible, setVisible] = React.useState(false)
  return (
    <>
      <Button {...props} onClick={() => setVisible(true)} />
      {visible
        ? React.cloneElement(modal, {
            visible,
            onClose: () => setVisible(false),
          })
        : false}
    </>
  )
}
