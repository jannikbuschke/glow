import { useFetch } from "./fetch-context"
import { ProblemDetails } from "./use-submit"

async function handleResponse<T>(response: Response) {
  if (response.ok) {
    if (response.status === 204) {
      return (null as any) as T
    }
    const data = (await response.json()) as T
    return data
  } else {
    const contentType = response.headers.get("content-type")
    if (contentType === "application/problem+json") {
      const data = (await response.json()) as {
        detail: string
        title: string
        type: string
        status: number
      }
      throw new Error(
        data.detail || data.title || data.type || "" + data.status,
      )
    } else if (response.status === 403) {
      throw {
        detail: "You are not authorized",
        title: "Forbidden",
      } as ProblemDetails
    } else {
      throw new Error(
        response.statusText + " (" + (await response.text()) + ")",
      )
    }
  }
}

export function useFetchJson<T>(): (
  key: RequestInfo,
  init?: RequestInit,
) => Promise<T> {
  const fetch = useFetch()
  return async (key: RequestInfo, init?: RequestInit) => {
    const response = await fetch(key, init)
    const result = handleResponse<T>(response)
    return result
  }
}

export async function fetchJson<T>(key: RequestInfo, init?: RequestInit) {
  const response = await fetch(key, init)
  const result = handleResponse<T>(response)
  return result
}

export async function postJson<Response>(
  url: string,
  payload: any,
  headers?: any,
) {
  const response = await fetch(url, {
    method: "POST",
    headers: {
      "content-type": "application/json",
      ...(headers ? headers : {}),
    },
    body: JSON.stringify(payload),
  })
  if (response.ok) {
    const data = (await response.json()) as Response
    return data
  } else {
    throw new Error(response.statusText + " (" + (await response.text()) + ")")
  }
}
