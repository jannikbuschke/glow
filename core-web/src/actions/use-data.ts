import React from "react"
import {
  QueryOptions,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query"
import { useFetchJson } from "./fetch"
import { OrderBy, Query, QueryResult, Where } from "../ts-models"

type GlowOptions<T> = GlowGetOptions | GlowPostOptions<T>

interface GlowGetOptions {
  type: "GET"
}

interface GlowPostOptions<T> {
  type: "POST"
  payload: T
}

export function useData2<T, U = any>(
  url: string,
  placeholder: T,
  config?: UseQueryOptions<T>,
  glowOptions?: GlowOptions<T>,
) {
  const fetchJson = useFetchJson<T>()
  const { data, ...rest } = useQuery<T, any>(
    url,
    () =>
      glowOptions?.type === "POST"
        ? fetchJson(url, {
            method: "POST",
            headers: { "content-type": "application/json" },
            body: JSON.stringify(glowOptions.payload),
          })
        : fetchJson(url),
    {
      retry: 1,
      refetchOnWindowFocus: false,
      ...config,
    },
  )
  return {
    data: Boolean(data) ? data! : placeholder,
    loading: rest.status === "loading",
    reload: rest.refetch,
    ...rest,
  }
}

interface UseApiGetProps<Result> {
  url: string
  placeholder: Result
  queryOptions?: QueryOptions<Result>
  method: "GET"
}

interface UseApiPostProps<Result, Request> {
  url: string
  placeholder: Result
  queryOptions?: QueryOptions<Result>
  method: "POST" | undefined
  // rename to request or params/parameters
  payload: Request
}

export type ApiResult<Result> = UseQueryResult<Result, any> & {
  data: Result
  loading: boolean
  reload: () => void
}

export function useApi<Result = any, Request = any>({
  url,
  placeholder,
  queryOptions,
  ...rest
}:
  | UseApiGetProps<Result>
  | UseApiPostProps<Result, Request>): ApiResult<Result> {
  const fetchJson = useFetchJson<Result>()
  const inputKey = rest.method === "POST" ? rest.payload : {}

  const { data, ...queryRest } = useQuery<Result, any>(
    [url, inputKey],
    () =>
      rest.method === "POST" || rest.method === undefined
        ? fetchJson(url, {
            method: "POST",
            headers: {
              "content-type": "application/json",
              "x-submit-intent": "execute",
            },
            body: JSON.stringify(rest.payload),
          })
        : fetchJson(url),
    {
      retry: 1,
      refetchOnWindowFocus: false,
      ...queryOptions,
    },
  )

  return {
    data: Boolean(data) ? data! : placeholder,
    loading: queryRest.status === "loading",
    reload: queryRest.refetch,
    ...queryRest,
  } as ApiResult<Result>
}

export type UseGlowQueryResult<T> = [
  UseGlowQuery<T>,
  UseQueryResult<QueryResult<T>, any>,
]

export interface UseGlowQuery<T> {
  result: QueryResult<T>
  skip: number
  take: number | null
  search: string | null
  setSearch: React.Dispatch<React.SetStateAction<string | null>>
  setSkip: React.Dispatch<React.SetStateAction<number>>
  setTake: React.Dispatch<React.SetStateAction<number | null>>
  setOrderBy: React.Dispatch<React.SetStateAction<null | OrderBy>>
  setWhere: React.Dispatch<React.SetStateAction<null | Where>>
  sendQuery: (query: Query, url: string) => Promise<QueryResult<T>>
}

function createSendQuery<T>(
  fetch: (
    key: RequestInfo,
    init?: RequestInit | undefined,
  ) => Promise<QueryResult<T>>,
) {
  async function sendQuery(query: Query, url: string) {
    const result = await fetch(url, {
      method: "POST",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(query),
    })
    return result
  }
  return sendQuery
}

export type QueryParameter = {
  take: number | null
  skip: number
  orderBy: OrderBy
  where: Where
  search: string | null
}

type QueryKey = [string, QueryParameter]

export function useGlowQuery<T>(
  url: string,
  placeholder: QueryResult<T>,
  config?: QueryOptions<QueryResult<T>>,
  initialQuery?: Partial<QueryParameter>,
): UseGlowQueryResult<T> {
  const fetch = useFetchJson<QueryResult<T>>()
  const sendQuery = React.useMemo(() => createSendQuery<T>(fetch), [fetch])
  const [take, setTake] = React.useState<number | null>(
    initialQuery?.take || 10,
  )
  const [skip, setSkip] = React.useState(initialQuery?.skip || 0)
  const [orderBy, setOrderBy] = React.useState<null | OrderBy>(
    initialQuery?.orderBy || null,
  )
  const [search, setSearch] = React.useState<null | string>(
    initialQuery?.search || null,
  )
  const [where, setWhere] = React.useState<null | Where>(
    initialQuery?.where || null,
  )

  const useQueryResult = useQuery<QueryResult<T>>(
    [url, { search, take, skip, orderBy, where }] as QueryKey,
    async ({ queryKey }: { queryKey: QueryKey }) => {
      const query: Query = {
        ...queryKey[1],
        count: true,
      }
      const result = await sendQuery(query, url)
      return result
    },
    config,
  )
  React.useEffect(() => {
    useQueryResult.refetch()
  }, [take, skip, orderBy, search])

  const data = useQueryResult.data
  return [
    {
      result: Boolean(data) ? data! : placeholder,
      skip,
      take,
      setSkip,
      setTake,
      search,
      setSearch,
      setOrderBy,
      setWhere,
      sendQuery,
    },
    useQueryResult,
  ]
}
