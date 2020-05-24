import { Link } from "react-router-dom"
import * as React from "react"
import { Menu } from "antd"
import { MenuItemProps } from "antd/lib/menu/MenuItem"

export const MenuItemLink = ({
  to,
  children,
  ...rest
}: React.PropsWithChildren<MenuItemProps & { to: string }>) => {
  return (
    <Link to={to}>
      <Menu.Item {...rest}>{children}</Menu.Item>
    </Link>
  )
}
