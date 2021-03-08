import * as React from "react"

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

export function VerticalSpace(
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
        flexDirection: "column",
        // alignItems: "center",
        ...style,
      }}
    />
  )
}
