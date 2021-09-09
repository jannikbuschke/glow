import { Button, ButtonProps } from "antd"
import * as React from "react"
import { RenderObject } from ".."

type Props = Omit<ButtonProps, "onClick"> & {
  modal: React.ReactElement<AutoControlledModalProps>
  unmountModalOnAfterClose?: boolean
}

export type AutoControlledModalProps = {
  visible: boolean
  onCancel: () => void
  afterClose: () => void
}

export function ModalButtonContainer({
  modal,
  unmountModalOnAfterClose,
  ...props
}: Props) {
  const [visible, setVisible] = React.useState(false)
  const [mountModal, setMountModal] = React.useState(false)
  return (
    <>
      <Button
        {...props}
        onClick={() => {
          setVisible(true)
          setMountModal(true)
        }}
      />
      {unmountModalOnAfterClose && !mountModal
        ? null
        : React.cloneElement(modal, {
            visible,
            onCancel: () => {
              setVisible(false)
            },
            afterClose: () => {
              setMountModal(false)
            },
          } as AutoControlledModalProps)}
    </>
  )
}
