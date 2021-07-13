import { QueryOptions } from "react-query";
import { useApi, ApiResult } from "glow-react";
import { useAction, useSubmit, UseSubmit } from "glow-react/es/Forms/use-submit";
import * as Glow_Core_EfCore from "./Glow.Core.EfCore"
import * as System from "./System"
import * as System_Reflection from "./System.Reflection"
import * as System_Runtime_InteropServices from "./System.Runtime.InteropServices"
import * as MediatR from "./MediatR"
import * as Glow_Core_Queries from "./Glow.Core.Queries"
import * as Serilog_Events from "./Serilog.Events"
import * as System_Security from "./System.Security"

type QueryInputs = {
}
type QueryOutputs = {
}
type Outputs = {
  "/api/glow/db/reset-database": MediatR.Unit,
}
type Actions = {
  "/api/glow/db/reset-database": Glow_Core_EfCore.ResetDatabase,
}

type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
};

type ActionTable = TagWithKey<"url", Actions>;

type Unionize<T> = T[keyof T];

type MyActions = Unionize<ActionTable>;

export function useTypedAction<ActionName extends keyof ActionTable>(key: ActionName): UseSubmit<Actions[ActionName], Outputs[ActionName]>{
  const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
  return s
}

type QueryTable = TagWithKey<"url", QueryInputs>;

export function useTypedQuery<ActionName extends keyof QueryTable>(key: ActionName, {
    placeholder,
    input,
    queryOptions
  }: {
    placeholder: QueryOutputs[ActionName],
    input:  QueryInputs[ActionName]
    queryOptions?: QueryOptions<QueryOutputs[ActionName]>
  }): ApiResult<QueryOutputs[ActionName]> {

  const { data, ...rest} = useApi({
    url: key,
    method:"POST",
    payload: input,
    // todo: find defaultPlaceholder
    placeholder: placeholder,
    queryOptions: queryOptions
  })

  const result = data as QueryOutputs[ActionName]

  return { data: result, ...rest} as any
}




