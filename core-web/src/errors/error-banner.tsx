import * as React from "react"
import { Alert } from "antd"
import { RenderObject } from "../debugging"
import { Alert as MantineAlert } from "@mantine/core"
import { ProblemDetails } from "../types"

export function WarningBanner({ message }: { message: any }) {
  return <Render type="warning" message={message} />
}
export function ErrorBanner({
  error,
  message,
}: {
  error?: any
  message?: any
}) {
  return <Render type="error" message={message || error} />
}

export function InfoBanner({ message }: { message: any }) {
  return <Render type="info" message={message} variant="light" />
}

type Variant = "light" | "outline" | "filled"

export function SuccessBanner({
  message,
  variant,
}: {
  message: any
  variant?: Variant
}) {
  return <Render type="success" message={message} variant={variant} />
}

function isProblemDetails(data: any): data is ProblemDetails {
  return (
    Boolean((data as ProblemDetails).title) &&
    Boolean((data as ProblemDetails).detail)
  )
}

function Render({
  type,
  message: msg,
  variant,
}: {
  type: "error" | "info" | "warning" | "success"
  variant?: Variant
  message: string | null | undefined | any
}) {
  if (!msg) {
    return null
  }
  const message = React.isValidElement(msg) ? (
    msg
  ) : typeof msg === "object" ? (
    isProblemDetails(msg) ? (
      <div>
        <b>{msg.title}</b>
        <div>{msg.detail}</div>
      </div>
    ) : msg instanceof Error ? (
      msg.message
    ) : (
      <RenderObject msg={msg} />
    )
  ) : (
    msg.toString()
  )

  return (
    <MantineAlert
      variant={variant ? variant : "outline"}
      color={
        type === "error"
          ? "red"
          : type === "info"
          ? "blue"
          : type === "success"
          ? "green"
          : type === "warning"
          ? "yellow"
          : undefined
      }
      style={{
        borderLeftWidth: 4,
        marginTop: 5,
        marginBottom: 5,
      }}
    >
      {message}
    </MantineAlert>
  )
}

// export { notifyError, notifySuccess, messageSuccess, notifyInfo }
