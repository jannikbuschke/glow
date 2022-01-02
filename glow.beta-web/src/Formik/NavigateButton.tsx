import { Button } from "antd"
import * as React from "react"
import { Link } from "react-router-dom"
import { ButtonProps } from "antd/lib/button"

type Props = { to: string } & ButtonProps

export const NavigateButton = (props: Props) => (
  <Button type="primary" {...props}>
    <Link to={props.to}>new</Link>
  </Button>
)
