import * as React from "react"
import { set } from "lodash"
import { useFetch } from "../http/fetch-context"
import { Modal } from "antd"

export type Result<T> = Success<T> | Error

export interface Success<T> {
  ok: true
  result: "success"
  payload: T
}

export interface Error {
  ok: false
  result: "error"
  error: ProblemDetails
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

export interface ProblemDetails {
  detail: string
  status: number
  title: string
  type: string
  extensions: any
}

export type UseSubmit<RequestPayload = any, ResultPayload = any> = [
  (values: RequestPayload) => Promise<Result<ResultPayload>>,
  (values: RequestPayload) => Promise<SerializableError | undefined>,
  { error: string | null; submitting: boolean },
]

export function useAction<RequestPayload = any, ResultPayload = any>(
  url: string,
): UseSubmit<RequestPayload, ResultPayload> {
  return useSubmit<ResultPayload, RequestPayload>(url)
}

export function useSubmit<ResultPayload = any, RequestPayload = any>(
  url: string,
): UseSubmit<RequestPayload, ResultPayload> {
  const [error, setError] = React.useState<null | string>(null)
  const fetch = useFetch()
  const [submitting, setSubmitting] = React.useState(false)

  return [
    async (values: any): Promise<Result<ResultPayload>> => {
      setSubmitting(true)
      const response = await fetch(url, execute(values))
      setSubmitting(false)

      if (!response.ok) {
        if (response.headers.has("content-type")) {
          const contentType = response.headers.get("content-type")
          if (
            contentType == "application/problem+json" ||
            contentType == "application/json" ||
            contentType?.startsWith("application/json")
          ) {
            const description = (await response.json()) as ProblemDetails
            setError(
              `${description.title}: ${description.detail} (${description.status})`,
            )

            if (
              description.type === "missing_consent" &&
              description.status === 403 &&
              description.extensions["scope"]
            ) {
              const scope = description.extensions["scope"]
              Modal.confirm({
                title: "Consent required",
                content: `Consent for ${scope} is required`,
                onOk: () => {
                  window.location.replace(
                    `/Account/SignIn?redirectUrl=${window.location.pathname}&scopes=${scope}`,
                  )
                },
              })
            }
            return { error: description, ok: false, result: "error" } as Error
          } else {
            const content = await response.text()
            console.error(content)
          }
        }
        return {
          error: {
            status: response.status,
            title: response.statusText,
            detail: "",
            type: "",
          } as ProblemDetails,
          ok: false,
          result: "error",
        } as Error
      } else {
        const data = (await response.json()) as ResultPayload
        return {
          payload: data,
          ok: true,
          result: "success",
        } as Success<ResultPayload>
      }
    },
    async (values: any): Promise<any | undefined> => {
      const response = await fetch(url, validate(values))
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
    { error, submitting },
  ]
}

function camelize(str: string) {
  return str
    .split(".")
    .map((v) => {
      if (v.endsWith("]") && v.includes("[")) {
        const split = v.split("[")
        return `${_camelize(split[0])}[${split[1]}`
      } else {
        return _camelize(v)
      }
    })
    .join(".")
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

function execute(values: any): RequestInit {
  return {
    method: "POST",
    body: JSON.stringify(values),
    credentials: "same-origin",
    headers: {
      "x-submit-intent": "execute",
      "content-type": "application/json",
    },
  }
}

function validate(values: any): RequestInit {
  return {
    method: "POST",
    body: JSON.stringify(values),
    credentials: "same-origin",
    headers: {
      "x-submit-intent": "validate",
      "content-type": "application/json",
    },
  }
}
