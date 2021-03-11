import * as React from "react"
import { useGlobalSettings } from "./index"

export function useFeatureFlags() {
  const ctx = useGlobalSettings()

  function isEnabled(feature: string, ifUndefined?: "show" | "hide") {
    if (ctx[feature] == "true") {
      return true
    }
    if (ctx[feature] == "false") {
      return false
    }
    if (ctx[feature] === undefined || ctx[feature] === null) {
      return ifUndefined === "show" ? true : false
    }
    return false
  }

  return isEnabled
}

interface Props {
  feature: string
  ifUndefined?: "show" | "hide"
  overrideEnable?: boolean
}

export const IfFeature: React.FC<Props> = ({
  ifUndefined,
  feature,
  children,
  overrideEnable
}) => {
  const isEnabled = useFeatureFlags()
  if(overrideEnable === true) {
    return children as React.ReactElement<any>
  }
  if (isEnabled(feature, ifUndefined)) {
    return children as React.ReactElement<any>
  }
  return null
}
