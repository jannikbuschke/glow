import { QueryOptions, useQuery } from "react-query"
import { fetchJson } from "../http/fetch"

export function useData<T>(
  url: string,
  placeholder: T,
  config?: QueryOptions<T>,
) {
  const { data, ...rest } = useQuery<T, any>(url, () => fetchJson(url), {
    retry: 1,
    refetchOnWindowFocus: false,
    ...config,
  })
  return {
    data: Boolean(data) ? data! : placeholder,
    loading: rest.status === "loading",
    reload: rest.refetch,
    ...rest,
  }
}
