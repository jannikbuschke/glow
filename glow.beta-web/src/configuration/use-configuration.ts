import { useData } from "../query/use-data"

export function useConfiguration<T>(path: string, placeholder: T) {
  const url = `/api/configurations/${path}`
  const data = useData<T>(url, placeholder)
  return data
}
