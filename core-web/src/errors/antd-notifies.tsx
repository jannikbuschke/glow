import * as React from "react"
import { message, notification } from "antd"
import { ProblemDetails } from "../actions/use-submit"

export function notifyError(r: ProblemDetails | string) {
  if (typeof r === "string") {
    notification.error({ message: r })
  } else {
    if (r.title && r.detail) {
      notification.error({
        description: r.detail,
        message: r.title,
      })
    } else {
      notification.error({
        message:
          r.title && r.detail
            ? r.title + ": " + r.detail
            : r.title || r.detail || r.status,
      })
    }
  }
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
