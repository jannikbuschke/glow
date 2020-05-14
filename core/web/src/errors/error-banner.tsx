import * as React from "react"
import { Alert } from "antd"

export function ErrorBanner({ error }: { error: any }) {
  return error ? (
    <Alert
      type="error"
      banner={true}
      message={error.toString()}
      showIcon={false}
    />
  ) : null
}
