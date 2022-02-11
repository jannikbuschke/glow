import { Button, ButtonProps, Menu, MenuItemProps } from "antd"
import * as React from "react"

type Props = {
  modal: React.ReactElement<AutoControlledModalProps>
  unmountModalOnAfterClose?: boolean
}

type PropsButton = Omit<ButtonProps, "onClick"> & Props

type PropsMenuItem = Omit<MenuItemProps, "onClick"> & Props

type PropsButtonV2 = { triggerType: "Button" } & ButtonProps

type PropsMenuItemV2 = { triggerType: "MenuItem" } & MenuItemProps

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
  onClick,
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
          onClick={(e) => {
            setVisible(true)
            setMountModal(true)
            onClick && onClick(e as any)
          }}
        />
      ) : (
        <Button
          {...btnProps}
          onClick={(e) => {
            setVisible(true)
            setMountModal(true)
            onClick && onClick(e as any)
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
