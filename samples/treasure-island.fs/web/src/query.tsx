import { ErrorBanner } from "glow-core"
import React from "react"
import { UseQueryOptions } from "react-query"
import {
  useTypedQuery,
  QueryTable,
  QueryOutputs,
  QueryInputs,
} from "./client/api"

export function Query<ActionName extends keyof QueryTable>(
  props: {
    name: ActionName
    placeholder: QueryOutputs[ActionName]
    input: QueryInputs[ActionName]
    queryOptions?: UseQueryOptions<QueryOutputs[ActionName]>
  } & (
    | {
        children: (data: QueryOutputs[ActionName]) => React.ReactNode
      }
    | { children?: undefined }
  ),
) {
  const queryOptions: UseQueryOptions<QueryOutputs[ActionName]> = {
    useErrorBoundary: true,
    suspense: true,
    ...props.queryOptions,
  }
  const x = useTypedQuery(props.name, props)
  if (typeof props["children"] === "undefined") {
    return <div>{JSON.stringify(x)}</div>
  } else {
    if (x.isLoading || x.isFetching) {
      return <div>Loading...</div>
    }
    return (
      <>
        <ErrorBanner error={x.error} />
        {props.children(x.data)}
      </>
    )
  }
}

function MyCoponent1() {
  return <Query name="/api/es/get-events2" placeholder={[]} input={{}} />
}

function MyCoponent() {
  return (
    <Query name="/api/es/get-events2" placeholder={[]} input={{}}>
      {(data) => <div>{JSON.stringify(data)}</div>}
    </Query>
  )
}
