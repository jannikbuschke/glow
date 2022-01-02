import * as React from "react"
import { Alert, message, notification } from "antd"
import { ProblemDetails } from "../actions/use-submit"
import { RenderObject } from "../debugging"

export function WarningBanner({ message }: { message: any }) {
  return render("warning", message)
}
export function ErrorBanner({
  error,
  message,
}: {
  error?: any
  message?: any
}) {
  return render("error", message || error)
}

export function InfoBanner({ message }: { message: any }) {
  return render("info", message)
}
export function SuccessBanner({ message }: { message: any }) {
  return render("success", message)
}

function render(
  type: "error" | "info" | "warning" | "success",
  msg: string | null | undefined | any,
) {
  return msg ? (
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
  ) : null
}

export function notifyError(r: ProblemDetails) {
  notification.error({
    message:
      r.title && r.detail
        ? r.title + ": " + r.detail
        : r.title || r.detail || r.status,
  })
}

export function notifySuccess(message?: string) {
  notification.success({
    message: message || "success",
  })
}

export function messageSuccess(msg: React.ReactNode | string) {
  message.success(msg)
}

export function notifyInfo(msg: React.ReactNode | string) {
  notification.info({ message: msg })
}
