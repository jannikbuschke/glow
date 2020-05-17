import * as React from "react"
import { Alert } from "antd"

export function WarningBanner({ message }: { message: any }) {
  return render("warning", message)
}
export function ErrorBanner({ error }: { error: any }) {
  return render("error", error)
}

export function InfoBanner({ message }: { message: any }) {
  return render("info", message)
}
export function SuccessBanner({ message }: { message: any }) {
  return render("success", message)
}

function render(
  type: "error" | "info" | "warning" | "success",
  msg: string | null | undefined,
) {
  return msg ? (
    <Alert
      type={type}
      message={msg.toString()}
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
