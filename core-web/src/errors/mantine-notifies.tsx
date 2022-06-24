import * as React from "react"
import { ProblemDetails } from "../actions/use-submit"
import { showNotification } from "@mantine/notifications"

export function notifyError(r: ProblemDetails | string) {
  if (typeof r === "string") {
    showNotification({ color: "red", message: r })
  } else {
    if (r.title && r.detail) {
      showNotification({
        color: "red",
        message: r.detail,
        title: r.title,
      })
    } else {
      showNotification({
        color: "red",
        message:
          r.title && r.detail
            ? r.title + ": " + r.detail
            : r.title || r.detail || r.status,
      })
    }
  }
}

export function notifySuccess(message?: string) {
  showNotification({
    color: "green",
    message: message || "success",
  })
}

export function messageSuccess(msg: React.ReactNode | string) {
  showNotification({ color: "green", message: msg })
}

export function notifyInfo(msg: React.ReactNode | string) {
  showNotification({ color: "blue", message: msg })
}
