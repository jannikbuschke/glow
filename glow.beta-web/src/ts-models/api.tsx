/* eslint-disable prettier/prettier */
import * as React from "react"
import { QueryOptions } from "react-query"
import { useApi, ApiResult, notifySuccess, notifyError } from "glow-core"
import {
  useAction,
  useSubmit,
  UseSubmit,
  ProblemDetails,
} from "glow-core/es/actions/use-submit"
import { Formik, FormikFormProps } from "formik"
import { Form } from "formik-antd"
import * as Glow_TestAutomation from "./Glow.TestAutomation"
import * as Glow_Core_EfCore from "./Glow.Core.EfCore"
import * as Glow_Core_OpenIdConnect from "./Glow.Core.OpenIdConnect"
import * as Glow_Core_Application from "./Glow.Core.Application"
import * as MediatR from "./MediatR"
import * as Glow_Core_Queries from "./Glow.Core.Queries"
import * as Glow_Core_Profiles from "./Glow.Core.Profiles"
import * as Serilog_Events from "./Serilog.Events"

type QueryInputs = {
  "/api/glow/test-automation/get-available-fake-users": Glow_TestAutomation.GetAvailableFakeUsers
}
type QueryOutputs = {
  "/api/glow/test-automation/get-available-fake-users": Glow_TestAutomation.FakeUsers
}
export type Outputs = {
  "/api/glow/db/reset-database": MediatR.Unit
  "/api/glow/set-openid-connect-options": MediatR.Unit
  "/api/glow/restart-application": MediatR.Unit
}
export type Actions = {
  "/api/glow/db/reset-database": Glow_Core_EfCore.ResetDatabase
  "/api/glow/set-openid-connect-options": Glow_Core_OpenIdConnect.SetOpenIdConnectOptions
  "/api/glow/restart-application": Glow_Core_Application.RestartApplication
}

type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
}

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
    </Formik>
  )
}
