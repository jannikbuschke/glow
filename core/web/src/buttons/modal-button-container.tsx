import { Button, ButtonProps, Menu, MenuItemProps } from "antd"
import * as React from "react"

type Props = {
  modal: React.ReactElement<AutoControlledModalProps>
  unmountModalOnAfterClose?: boolean
}

type PropsButton = Omit<ButtonProps, "onClick"> & Props

type PropsMenuItem = Omit<MenuItemProps, "onClick"> & Props

export type AutoControlledModalProps = {
  visible: boolean
  onCancel: () => void
  afterClose: () => void
}

export function ModalMenuItemContainer({
  modal,
  unmountModalOnAfterClose,
  ...props
}: PropsMenuItem) {
  const [visible, setVisible] = React.useState(false)
  const [mountModal, setMountModal] = React.useState(false)

  return (
    <>
      <Menu.Item
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

export function ModalButtonContainer({
  modal,
  unmountModalOnAfterClose,
  ...props
}: PropsButton) {
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
