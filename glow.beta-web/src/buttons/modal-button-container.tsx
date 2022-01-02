import { Button, ButtonProps, Menu, MenuItemProps } from "antd"
import * as React from "react"

type Props = {
  modal: React.ReactElement<AutoControlledModalProps>
  unmountModalOnAfterClose?: boolean
}

type PropsButton = Omit<ButtonProps, "onClick"> & Props

type PropsMenuItem = Omit<MenuItemProps, "onClick"> & Props

type PropsButtonV2 = { triggerType: "Button" } & Omit<ButtonProps, "onClick">

type PropsMenuItemV2 = { triggerType: "MenuItem" } & Omit<
  MenuItemProps,
  "onClick"
>

export type ModalTriggerType = "Button" | "MenuItem"

export type AutoControlledModalProps = {
  visible: boolean
  onCancel: () => void
  afterClose: () => void
}

export function ModalContainer({
  modal,
  unmountModalOnAfterClose,
  triggerType,
  ...props
}: (PropsMenuItemV2 | PropsButtonV2) & Props) {
  const btnProps = props as PropsButtonV2
  const menuItemProps = props as PropsMenuItemV2
  const [visible, setVisible] = React.useState(false)
  const [mountModal, setMountModal] = React.useState(false)
  return (
    <>
      {triggerType === "MenuItem" ? (
        <Menu.Item
          {...menuItemProps}
          onClick={() => {
            setVisible(true)
            setMountModal(true)
          }}
        />
      ) : (
        <Button
          {...btnProps}
          onClick={() => {
            setVisible(true)
            setMountModal(true)
          }}
        />
      )}
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

/// deprecated, use ModalContainer
export function ModalMenuItemContainer({
  modal,
  unmountModalOnAfterClose,
  ...props
}: PropsMenuItem) {
  return (
    <ModalContainer
      triggerType="MenuItem"
      modal={modal}
      unmountModalOnAfterClose={unmountModalOnAfterClose}
      {...props}
    />
  )
}

/// deprecated, use ModalContainer
export function ModalButtonContainer({
  modal,
  unmountModalOnAfterClose,
  ...props
}: PropsButton) {
  return (
    <ModalContainer
      triggerType="Button"
      modal={modal}
      unmountModalOnAfterClose={unmountModalOnAfterClose}
      {...props}
    />
  )
}
