import { QueryOptions } from "react-query";
import { useApi, ApiResult } from "glow-react";
import { useAction, useSubmit, UseSubmit } from "glow-react/es/Forms/use-submit";
import * as Glow_Sample_Files from "./Glow.Sample.Files"
import * as Glow_Configurations from "./Glow.Configurations"
import * as Glow_Core_Profiles from "./Glow.Core.Profiles"
import * as System_Collections_Generic from "./System.Collections.Generic"
import * as Glow_Sample_Configurations from "./Glow.Sample.Configurations"
import * as Glow_Sample_MdxSourceFiles from "./Glow.Sample.MdxSourceFiles"
import * as Glow_Sample_Actions from "./Glow.Sample.Actions"
import * as MediatR from "./MediatR"
import * as Glow_Sample_Views from "./Glow.Sample.Views"
import * as Glow_Sample_Users from "./Glow.Sample.Users"
import * as Glow_Sample_Forms from "./Glow.Sample.Forms"

type QueryInputs = {
  "/api/source-file/get-list": Glow_Sample_MdxSourceFiles.GetList,
  "/api/source-file/get-single": Glow_Sample_MdxSourceFiles.GetEntityViewmodel,
}
type QueryOutputs = {
  "/api/source-file/get-list": Glow_Sample_MdxSourceFiles.Entity[],
  "/api/source-file/get-single": Glow_Sample_MdxSourceFiles.EntityViewmodel,
}
type Outputs = {
  "/api/source-file/create": Glow_Sample_MdxSourceFiles.Entity,
  "/api/source-file/update": Glow_Sample_MdxSourceFiles.Entity,
  "/api/actions/sample": MediatR.Unit,
  "/api/actions/sample-2": Glow_Sample_Actions.Response,
}
type Actions = {
  "/api/source-file/create": Glow_Sample_MdxSourceFiles.Create,
  "/api/source-file/update": Glow_Sample_MdxSourceFiles.Update,
  "/api/actions/sample": Glow_Sample_Actions.SampleAction,
  "/api/actions/sample-2": Glow_Sample_Actions.SampleAction2,
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




