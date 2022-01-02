import * as React from "react"
import { useLocalStorage } from "../local-storage"

interface LocalFeatures {
  [key: string]: boolean
}

export function useLocalFeatures<T extends LocalFeatures>(initial: T) {
  const features = useLocalStorage<T>("local-features", initial)
  return features
}
