import * as React from "react"
import { Alert } from "antd"
import { RenderObject } from "../debugging"
import { Alert as MantineAlert } from "@mantine/core"
import { useGlowContext } from "../glow-provider"
import {
  messageSuccess,
  notifyError,
  notifyInfo,
  notifySuccess,
} from "./antd-notifies"

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
  return <Render type="info" message={message} />
}
export function SuccessBanner({ message }: { message: any }) {
  return <Render type="success" message={message} />
}

function Render({
  type,
  message: msg,
}: {
  type: "error" | "info" | "warning" | "success"
  message: string | null | undefined | any
}) {
  const { componentLibrary } = useGlowContext()
  if (!msg) {
    return null
  }
  const message = React.isValidElement(msg) ? (
    msg
  ) : typeof msg === "object" ? (
    msg instanceof Error ? (
      msg.message
    ) : (
      <RenderObject msg={msg} />
    )
  ) : (
    msg.toString()
  )

  return componentLibrary === "antd" ? (
    <Alert
      type={type}
      message={
        React.isValidElement(msg) ? (
          msg
        ) : typeof msg === "object" ? (
          msg instanceof Error ? (
            msg.message
          ) : (
            <RenderObject msg={msg} />
          )
        ) : (
          msg.toString()
        )
      }
      showIcon={false}
      style={{
        borderRight: "none",
        borderBottom: "none",
        borderTop: "none",
        borderLeftWidth: 5,
        marginTop: 5,
        marginBottom: 5,
      }}
    />
  ) : (
    <MantineAlert
      variant="outline"
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
