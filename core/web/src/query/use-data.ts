import * as React from "react"
import { useQuery } from "react-query"
import { fetchJson } from "../http/fetch"

export function useData<T>(url: string, placeholder: T) {
  const { data, ...rest } = useQuery<T, any>(url, () => fetchJson(url), {
    retry: 1,
  })
  if (!Boolean(data)) {
    return { data: placeholder, ...rest }
  }
  return { data: data!, ...rest }
}
