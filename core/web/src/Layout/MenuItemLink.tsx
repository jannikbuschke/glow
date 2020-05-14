import { useNavigate } from "react-router-dom"
import * as React from "react"
import { Menu } from "antd"
import { MenuItemProps } from "antd/lib/menu/MenuItem"

export const MenuItemLink = ({
  to,
  children,
  ...rest
}: React.PropsWithChildren<MenuItemProps & { to: string }>) => {
  const navigate = useNavigate()
  return (
    <Menu.Item onClick={() => navigate(to)} {...rest}>
      {children}
    </Menu.Item>
  )
}
