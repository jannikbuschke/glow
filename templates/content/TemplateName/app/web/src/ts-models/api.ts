import { QueryOptions } from "react-query"
import { useApi, ApiResult } from "glow-core"
import { useAction, useSubmit, UseSubmit } from "glow-core/es/Forms/use-submit"
import * as TemplateName_Example from "./TemplateName.Example"

type QueryInputs = {
  "/api/person/get-list": TemplateName_Example.GetPersonList
  "/api/person/get-single": TemplateName_Example.GetPerson
}
type QueryOutputs = {
  "/api/person/get-list": TemplateName_Example.Person[]
  "/api/person/get-single": TemplateName_Example.Person
}
type Outputs = {
  "/api/person/create": TemplateName_Example.Person
  "/api/person/update": TemplateName_Example.Person
  "/api/person/delete": TemplateName_Example.Person
}
type Actions = {
  "/api/person/create": TemplateName_Example.CreatePerson
  "/api/person/update": TemplateName_Example.UpdatePerson
  "/api/person/delete": TemplateName_Example.DeletePerson
}

type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
}

type ActionTable = TagWithKey<"url", Actions>

type Unionize<T> = T[keyof T]

type MyActions = Unionize<ActionTable>

export function useTypedAction<ActionName extends keyof ActionTable>(
  key: ActionName,
): UseSubmit<Actions[ActionName], Outputs[ActionName]> {
  const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
  return s
}

type QueryTable = TagWithKey<"url", QueryInputs>

export function useTypedQuery<ActionName extends keyof QueryTable>(
  key: ActionName,
  {
    placeholder,
    input,
    queryOptions,
  }: {
    placeholder: QueryOutputs[ActionName]
    input: QueryInputs[ActionName]
    queryOptions?: QueryOptions<QueryOutputs[ActionName]>
  },
): ApiResult<QueryOutputs[ActionName]> {
  const { data, ...rest } = useApi({
    url: key,
    method: "POST",
    payload: input,
    // todo: find defaultPlaceholder
    placeholder: placeholder,
    queryOptions: queryOptions,
  })

  const result = data as QueryOutputs[ActionName]

  return { data: result, ...rest } as any
}
