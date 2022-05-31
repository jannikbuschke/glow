import { Button } from "antd"
import * as React from "react"
import { Link } from "react-router-dom"
import { ButtonProps } from "antd/es/button"

export type NavigationButtonProps = { to: string } & ButtonProps

export const NavigationButton = (props: NavigationButtonProps) => (
  <Button {...props}>
    <Link to={props.to}>{props.children}</Link>
  </Button>
)
