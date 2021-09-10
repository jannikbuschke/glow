import { QueryOptions } from "react-query";
import { useApi, ApiResult } from "glow-react";
import { useAction, useSubmit, UseSubmit } from "glow-react/es/Forms/use-submit";
import * as Glow_Sample_Files from "./Glow.Sample.Files"
import * as Glow_Configurations from "./Glow.Configurations"
import * as Glow_Core_Profiles from "./Glow.Core.Profiles"
import * as Glow_Sample_Configurations from "./Glow.Sample.Configurations"
import * as Glow_Sample_MdxBundle from "./Glow.Sample.MdxBundle"
import * as Glow_Sample_Actions from "./Glow.Sample.Actions"
import * as MediatR from "./MediatR"
import * as Glow_Sample_Views from "./Glow.Sample.Views"
import * as Glow_Sample_Users from "./Glow.Sample.Users"
import * as Glow_Sample_Forms from "./Glow.Sample.Forms"

type QueryInputs = {
  "/api/mdx/get-list": Glow_Sample_MdxBundle.GetMdxList,
  "/api/mdx/get-single": Glow_Sample_MdxBundle.GetEntityViewmodel,
}
type QueryOutputs = {
  "/api/mdx/get-list": Glow_Sample_MdxBundle.Mdx[],
  "/api/mdx/get-single": Glow_Sample_MdxBundle.MdxViewmodel,
}
type Outputs = {
  "/api/mdx/create": Glow_Sample_MdxBundle.Mdx,
  "/api/mdx/update": Glow_Sample_MdxBundle.Mdx,
  "/api/actions/sample": MediatR.Unit,
  "/api/actions/sample-2": Glow_Sample_Actions.Response,
  "/api/mdx/transpile": Glow_Sample_MdxBundle.TranspileResult,
}
type Actions = {
  "/api/mdx/create": Glow_Sample_MdxBundle.CreateMdx,
  "/api/mdx/update": Glow_Sample_MdxBundle.UpdateMdx,
  "/api/actions/sample": Glow_Sample_Actions.SampleAction,
  "/api/actions/sample-2": Glow_Sample_Actions.SampleAction2,
  "/api/mdx/transpile": Glow_Sample_MdxBundle.Transpile,
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




