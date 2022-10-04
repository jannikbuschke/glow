/* eslint-disable prettier/prettier */
// Assembly: glow.beta, Version=0.23.0.0, Culture=neutral, PublicKeyToken=null
import * as React from "react"
import { QueryOptions, UseQueryOptions } from "react-query"
import { useApi, ApiResult, useNotify } from "glow-core"
import { useAction, useSubmit, UseSubmit, ProblemDetails } from "glow-core"
import { Formik, FormikConfig, FormikFormProps, FormikProps } from "formik"
import { Form } from "formik-antd"
import * as Glow_Core_Application from "./Glow.Core.Application"
import * as MediatR from "./MediatR"
import * as Glow_Core_Queries from "./Glow.Core.Queries"
import * as Serilog_Events from "./Serilog.Events"

export type QueryInputs = {
}
export type QueryOutputs = {
}
export type Outputs = {
  "/api/glow/restart-application": MediatR.Unit,
}
export type Actions = {
  "/api/glow/restart-application": Glow_Core_Application.RestartApplication,
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

export type FieldPath<P, S> = {
  _PATH_: string & { _BRAND_: P & S }
}

export type PathProxy<P, S> = FieldPath<P, S> &
  { [K in keyof S]: PathProxy<P, S[K]> }

const IdPath = { _PATH_: "" } as FieldPath<any, any>

export function pathProxy<S, P = S>(
  parent: FieldPath<P, S> = IdPath as any,
): PathProxy<P, S> {
  return new Proxy(parent as any, {
    get(target: any, key: any) {
      if (key in target) return target[key]
      return pathProxy<any, any>({
        _PATH_: `${parent._PATH_ && parent._PATH_ + "."}${key}`,
        } as any)
    },
})
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
  const _ = React.useMemo(()=> pathProxy<Actions[ActionName]>(),[])
  const { messageSuccess, notifyError} = useNotify()
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
      }
    }
    >
      {(f) => (
        <Form {...formProps}>
          {typeof children === "function" ? children(f) : children}
        </Form>

      )}
    </Formik>)
}

