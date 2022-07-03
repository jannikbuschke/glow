/* eslint-disable prettier/prettier */
// Assembly: Glow.Sample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
import * as React from "react"
import { QueryOptions, UseQueryOptions } from "react-query"
import { useApi, ApiResult, useNotify } from "glow-core"
import { useAction, useSubmit, UseSubmit, ProblemDetails } from "glow-core"
import { Formik, FormikConfig, FormikFormProps } from "formik"
import { Form } from "formik-antd"
import * as Glow_Sample from "./Glow.Sample"
import * as Glow_Sample_TreasureIsland_Api from "./Glow.Sample.TreasureIsland.Api"
import * as Glow_Sample_Azdo from "./Glow.Sample.Azdo"
import * as MediatR from "./MediatR"
import * as Microsoft_TeamFoundation_DistributedTask_WebApi from "./Microsoft.TeamFoundation.DistributedTask.WebApi"
import * as Microsoft_VisualStudio_Services_WebApi from "./Microsoft.VisualStudio.Services.WebApi"
import * as Microsoft_VisualStudio_Services_Common from "./Microsoft.VisualStudio.Services.Common"
import * as Microsoft_TeamFoundation_SourceControl_WebApi from "./Microsoft.TeamFoundation.SourceControl.WebApi"
import * as Microsoft_TeamFoundation_Core_WebApi from "./Microsoft.TeamFoundation.Core.WebApi"

type QueryInputs = {
  "/api/ti/get-players": Glow_Sample_TreasureIsland_Api.GetPlayers,
  "/api/ti/get-games": Glow_Sample_TreasureIsland_Api.GetGames,
  "/azdo/get-commits": Glow_Sample_Azdo.GetCommits,
  "/azdo/get-items": Glow_Sample_Azdo.GetItems,
  "/azdo/get-item": Glow_Sample_Azdo.GetItem,
  "/azdo/get-projects": Glow_Sample_Azdo.GetProjects,
}
type QueryOutputs = {
  "/api/ti/get-players": Array<Glow_Sample.Unit>,
  "/api/ti/get-games": Array<Glow_Sample.Game>,
  "/azdo/get-commits": Array<Glow_Sample_Azdo.Commit>,
  "/azdo/get-items": Array<Microsoft_TeamFoundation_SourceControl_WebApi.GitItem>,
  "/azdo/get-item": Glow_Sample_Azdo.StringWrapper,
  "/azdo/get-projects": Array<Microsoft_TeamFoundation_Core_WebApi.TeamProjectReference>,
}
export type Outputs = {
  "/api/ti/move-player": MediatR.Unit,
  "/api/ti/restart-game": MediatR.Unit,
  "/api/ti/create-player": Glow_Sample_TreasureIsland_Api.CreatePlayerResult,
  "/api/ti/join": MediatR.Unit,
  "/azdo/create-library": Microsoft_TeamFoundation_DistributedTask_WebApi.VariableGroup,
  "/azdo/create-commit": Microsoft_TeamFoundation_SourceControl_WebApi.GitPush,
}
export type Actions = {
  "/api/ti/move-player": Glow_Sample_TreasureIsland_Api.MoveOrAttack,
  "/api/ti/restart-game": Glow_Sample_TreasureIsland_Api.RestartGame,
  "/api/ti/create-player": Glow_Sample_TreasureIsland_Api.CreatePlayer,
  "/api/ti/join": Glow_Sample_TreasureIsland_Api.Join,
  "/azdo/create-library": Glow_Sample_Azdo.CreateLibrary,
  "/azdo/create-commit": Glow_Sample_Azdo.CreatePullRequest,
}

type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
};

export type ActionTable = TagWithKey<"url", Actions>

export type TypedActionHookResult<
  ActionName extends keyof ActionTable
> = UseSubmit<Actions[ActionName], Outputs[ActionName]>

export type TypedActionHook = <ActionName extends keyof ActionTable>(
  key: ActionName,
) => TypedActionHookResult<ActionName>

export const useTypedAction: TypedActionHook = <
  ActionName extends keyof ActionTable
>(
  key: ActionName,
) => {
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
    queryOptions?: UseQueryOptions<QueryOutputs[ActionName]>
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

export function TypedForm<ActionName extends keyof ActionTable>({
  initialValues,
  actionName,
  formProps,
  children,
  onSuccess,
  onError,
}: Omit<FormikConfig<Actions[ActionName]>, "onSubmit"> & {
  actionName: ActionName
  formProps?: FormikFormProps
  onSuccess?: (payload: Outputs[ActionName]) => void
  onError?: (error: ProblemDetails) => void
}) {
  const { messageSuccess, notifyError } = useNotify()
  const [submit, validate] = useTypedAction<ActionName>(actionName)
  return (
    <Formik
      validate={validate}
      validateOnBlur={true}
      enableReinitialize={true}
      validateOnChange={false}
      initialValues={initialValues}
      onSubmit={async (values) => {
        const response = await submit(values)
        if (response.ok) {
          onSuccess && onSuccess(response.payload)
        } else {
          onError && onError(response.error)
          !onError && notifyError(response.error)
        }
      }}
    >
      {(f) => (
        <Form {...formProps}>
          {typeof children === "function" ? children(f) : children}
        </Form>
      )}
    </Formik>)
}

