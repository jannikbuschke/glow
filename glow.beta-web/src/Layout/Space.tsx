import * as React from "react"
import { Space as AntSpace } from "antd"

export function Space(
  props: React.DetailedHTMLProps<
    React.HTMLAttributes<HTMLDivElement>,
    HTMLDivElement
  >,
) {
  const { style, ...restProps } = props
  return (
    <div
      {...restProps}
      style={{
        display: "flex",
        gap: "8px",
        alignItems: "center",
        ...style,
      }}
    />
  )
}

export type VerticalSpaceProps = {
  size?: "small" | "default" | "large"
} & React.DetailedHTMLProps<
  React.HTMLAttributes<HTMLDivElement>,
  HTMLDivElement
>

export function VerticalSpace(props: VerticalSpaceProps) {
  //maybe use ant design still

  // return (
  //   <AntSpace style={{ flexDirection: "column", width: "100%" }} {...props} />
  // )

  const { style, ...restProps } = props
  return (
    <div
      {...restProps}
      style={{
        display: "flex",
        gap:
          props.size === "default"
            ? "8px"
            : props.size === "large"
            ? "12px"
            : "4px",
        flexDirection: "column",
        // alignItems: "center",
        ...style,
      }}
    />
  )
}
