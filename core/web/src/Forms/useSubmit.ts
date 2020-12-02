import * as React from "react"
import { set } from "lodash"
import { useFetch } from "../http/fetch-context"

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
  const fetch = useFetch()
  return [
    async (values: any): Promise<T | undefined> => {
      const response = await fetch(url, {
        method: "POST",
        body: JSON.stringify(values),
        headers: {
          "x-submit-intent": "execute",
          "content-type": "application/json",
        },
      })
      if (!response.ok) {
        if (response.headers.has("content-type")) {
          const contentType = response.headers.get("content-type")
          if (contentType == "application/problem+json") {
            const description = (await response.json()) as ProblemDetails
            setError(
              `${description.title}: ${description.detail} (${description.status})`,
            )
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
      const response = await fetch(url, {
        method: "POST",
        body: JSON.stringify(values),
        headers: {
          "x-submit-intent": "validate",
          "content-type": "application/json",
        },
      })
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
