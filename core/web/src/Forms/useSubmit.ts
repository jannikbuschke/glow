import * as React from "react"
import { set } from "lodash"

function camelize(str: string) {
  return str.split(".").map(_camelize).join(".")
}

function _camelize(str: string) {
  return str.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function (match, index) {
    if (+match === 0) return "" // or if (/\s+/.test(match)) for white spaces
    return index == 0 ? match.toLowerCase() : match.toUpperCase()
  })
}

const toFormikErrors = (error: SerializableError) => {
  const errors = {}
  Object.keys(error).forEach((key) => {
    const path = camelize(key)
    set(errors, path, error[key])
  })
  return errors
}

function send(url: string, values: any, intent: "execute" | "validate") {
  return fetch(url, {
    method: "POST",
    body: JSON.stringify(values),
    headers: {
      "x-submit-intent": intent,
      "content-type": "application/json",
    },
  })
}

export interface ValidationResult {
  isValid: boolean
  errors: SerializableError
}

export interface SerializableError {
  [key: string]: string[]
}

export interface ModelStateDictionary {
  root: any
  maxAllowedErrors: number
  hasReachedMaxErrors: boolean
  errorCount: number
  count: number
  keys: string | null[]
  values: any[]
  isValid: boolean
  validationState: number
  item: any
}

interface ProblemDetails {
  detail: string
  status: number
  title: string
  type: string
}

export function useSubmit<T = any>(
  url: string,
): [
  (values: any) => Promise<T | undefined>,
  (values: any) => Promise<SerializableError | undefined>,
  string,
] {
  const [error, setError] = React.useState("")
  return [
    async (values: any): Promise<T | undefined> => {
      const response = await send(url, values, "execute")
      if (!response.ok) {
        if (response.headers.has("content-type")) {
          const contentType = response.headers.get("content-type")
          if (contentType == "application/problem+json") {
            setError("problem json")
            const description = (await response.json()) as ProblemDetails
            if (description.type == "bad_request") {
              setError(description.detail)
            }
          }
        }
        console.error(response)
        return
      } else {
        const data = (await response.json()) as T
        return data
      }
    },
    async (values: any): Promise<any | undefined> => {
      const response = await send(url, values, "validate")
      if (response.ok) {
        const data = (await response.json()) as ValidationResult
        const errors = toFormikErrors(data.errors)
        return errors
      } else {
        console.error(response)
        setError("error while validating: " + response.statusText)
        return
      }
    },
    error,
  ]
}
