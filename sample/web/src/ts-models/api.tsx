/* eslint-disable prettier/prettier */
import * as React from "react"
import { QueryOptions, UseQueryOptions } from "react-query"
import { useApi, ApiResult, notifySuccess, notifyError } from "glow-react"
import { useAction, useSubmit, UseSubmit, ProblemDetails } from "glow-react/es/Forms/use-submit"
import { Formik, FormikFormProps } from "formik"
import { Form } from "formik-antd"
import * as Glow_Sample_Files from "./Glow.Sample.Files"
import * as Glow_Configurations from "./Glow.Configurations"
import * as Glow_Core_Profiles from "./Glow.Core.Profiles"
import * as Glow_Sample_Configurations from "./Glow.Sample.Configurations"
import * as Glow_Sample_MdxBundle from "./Glow.Sample.MdxBundle"
import * as Glow_Sample_Azdo from "./Glow.Sample.Azdo"
import * as Glow_Sample_Actions from "./Glow.Sample.Actions"
import * as Microsoft_TeamFoundation_SourceControl_WebApi from "./Microsoft.TeamFoundation.SourceControl.WebApi"
import * as Microsoft_VisualStudio_Services_WebApi from "./Microsoft.VisualStudio.Services.WebApi"
import * as Microsoft_VisualStudio_Services_Common from "./Microsoft.VisualStudio.Services.Common"
import * as Microsoft_TeamFoundation_Core_WebApi from "./Microsoft.TeamFoundation.Core.WebApi"
import * as Microsoft_TeamFoundation_DistributedTask_WebApi from "./Microsoft.TeamFoundation.DistributedTask.WebApi"
import * as MediatR from "./MediatR"
import * as Glow_Sample_Views from "./Glow.Sample.Views"
import * as Glow_Sample_Users from "./Glow.Sample.Users"
import * as Glow_Sample_Forms from "./Glow.Sample.Forms"

type QueryInputs = {
  "/api/mdx/get-list": Glow_Sample_MdxBundle.GetMdxList,
  "/api/mdx/get-single": Glow_Sample_MdxBundle.GetEntityViewmodel,
  "/azdo/get-commits": Glow_Sample_Azdo.GetCommits,
}
type QueryOutputs = {
  "/api/mdx/get-list": Glow_Sample_MdxBundle.Mdx[],
  "/api/mdx/get-single": Glow_Sample_MdxBundle.MdxViewmodel,
  "/azdo/get-commits": Glow_Sample_Azdo.Commit[],
}
export type Outputs = {
  "/api/mdx/create": Glow_Sample_MdxBundle.Mdx,
  "/api/mdx/update": Glow_Sample_MdxBundle.Mdx,
  "/azdo/create-commit": Microsoft_TeamFoundation_SourceControl_WebApi.GitPush,
  "/azdo/create-library": Microsoft_TeamFoundation_DistributedTask_WebApi.VariableGroup,
  "/api/actions/sample": MediatR.Unit,
  "/api/actions/sample-2": Glow_Sample_Actions.Response,
  "/api/mdx/transpile": Glow_Sample_MdxBundle.TranspileResult,
}
export type Actions = {
  "/api/mdx/create": Glow_Sample_MdxBundle.CreateMdx,
  "/api/mdx/update": Glow_Sample_MdxBundle.UpdateMdx,
  "/azdo/create-commit": Glow_Sample_Azdo.CreateCommit,
  "/azdo/create-library": Glow_Sample_Azdo.CreateLibrary,
  "/api/actions/sample": Glow_Sample_Actions.SampleAction,
  "/api/actions/sample-2": Glow_Sample_Actions.SampleAction2,
  "/api/mdx/transpile": Glow_Sample_MdxBundle.Transpile,
}

type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
};

//export function useTypedAction<ActionName extends keyof ActionTable>(key: ActionName): UseSubmit<Actions[ActionName], Outputs[ActionName]>{
//  const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
//  return s
//}

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

// export function useTypedAction<ActionName extends keyof ActionTable>(
//   key: ActionName,
// ): UseSubmit<Actions[ActionName], Outputs[ActionName]> {
//   const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
//   return s
// }

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
}: React.PropsWithChildren<{
  actionName: ActionName
  initialValues: Actions[ActionName]
  formProps?: FormikFormProps
  onSuccess?: (payload: Outputs[ActionName]) => void
  onError?: (error: ProblemDetails) => void
}>) {
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

