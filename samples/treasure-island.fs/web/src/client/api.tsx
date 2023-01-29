import * as React from "react"
import { QueryOptions, UseQueryOptions } from "react-query"
import { useApi, ApiResult, useNotify } from "glow-core"
import { useAction, useSubmit, UseSubmit, ProblemDetails } from "glow-core"
import { Formik, FormikConfig, FormikFormProps, FormikProps } from "formik"
import { Form } from "formik-antd"
import * as System from "./System"
import * as TreasureIsland from "./TreasureIsland"
import * as Microsoft_FSharp_Core from "./Microsoft_FSharp_Core"
import * as Microsoft_FSharp_Collections from "./Microsoft_FSharp_Collections"
import * as Glow_Api from "./Glow_Api"
import * as Glow_Debug from "./Glow_Debug"
import * as Glow_TestAutomation from "./Glow_TestAutomation"
import * as Glow_Azure_AzureKeyVault from "./Glow_Azure_AzureKeyVault"
import * as Glow_Core_Profiles from "./Glow_Core_Profiles"
import * as System_Collections_Generic from "./System_Collections_Generic"
import * as MediatR from "./MediatR"

export type QueryInputs = {
  "/api/es/get-events2": TreasureIsland.GetEsEvents,
  "/api/ti/get-players": TreasureIsland.GetPlayers,
  "/api/ti/get-games": TreasureIsland.GetGames,
  "/api/ti/get-game-sate": TreasureIsland.GetGameState,
  "/api/debug/get-documents": TreasureIsland.GetDocuments,
  "/api/es/get-events": Glow_Api.GetEsEvents,
  "/api/glow/pgsql/get-activity": Glow_Debug.GetPgsqlActivities,
  "/api/glow/test-automation/get-available-fake-users": Glow_TestAutomation.GetAvailableFakeUsers,
  "/glow/profile/get-profile": Glow_Core_Profiles.GetProfile,
}
export type QueryOutputs = {
  "/api/es/get-events2": System_Collections_Generic.List<TreasureIsland.EventViewmodel>,
  "/api/ti/get-players": System_Collections_Generic.IReadOnlyList<TreasureIsland.PlayerUnit>,
  "/api/ti/get-games": System_Collections_Generic.IReadOnlyList<TreasureIsland.Game>,
  "/api/ti/get-game-sate": Microsoft_FSharp_Core.FSharpResult<TreasureIsland.Game,TreasureIsland.Error>,
  "/api/debug/get-documents": System_Collections_Generic.IEnumerable<System.Object>,
  "/api/es/get-events": System_Collections_Generic.List<Glow_Api.EventViewmodel>,
  "/api/glow/pgsql/get-activity": System_Collections_Generic.List<Glow_Debug.Activity>,
  "/api/glow/test-automation/get-available-fake-users": Glow_TestAutomation.FakeUsers,
  "/glow/profile/get-profile": Glow_Core_Profiles.Profile,
}
export type Outputs = {
  "/api/move-player": Microsoft_FSharp_Core.FSharpResult<MediatR.Unit,TreasureIsland.Error>,
  "/api/start-game": Microsoft_FSharp_Core.FSharpResult<MediatR.Unit,TreasureIsland.Error>,
  "/api/restart-game": MediatR.Unit,
  "/api/ti/create-player": TreasureIsland.CreatePlayerResult,
  "/api/debug/rebuild-projections": MediatR.Unit,
  "/api/glow/set-openid-connect-options": MediatR.Unit,
}
export type Actions = {
  "/api/move-player": TreasureIsland.MoveOrAttack,
  "/api/start-game": TreasureIsland.StartGameRequest,
  "/api/restart-game": TreasureIsland.RestartGame,
  "/api/ti/create-player": TreasureIsland.CreatePlayer,
  "/api/debug/rebuild-projections": TreasureIsland.RebuildProjections,
  "/api/glow/set-openid-connect-options": Glow_Azure_AzureKeyVault.SetOpenIdConnectOptions,
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

export type QueryTable = TagWithKey<"url", QueryInputs>;

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
  beforeSubmit,
  onSubmit,
  onError,
}: Omit<FormikConfig<Actions[ActionName]>, "onSubmit"|"children"> & {
  children: ((props: FormikProps<Actions[ActionName]>, pathProxy: PathProxy<Actions[ActionName], Actions[ActionName]>) => React.ReactNode) | React.ReactNode
  actionName: ActionName
  beforeSubmit?: (values: Actions[ActionName]) => Actions[ActionName]
  onSubmit?: (values: Actions[ActionName]) => void
  formProps?: FormikFormProps
  onSuccess?: (payload: Outputs[ActionName]) => void
  onError?: (error: ProblemDetails) => void
}) {
  const _ = React.useMemo(()=> pathProxy<Actions[ActionName]>(),[])
  const { messageSuccess, notifyError} = useNotify()
  const [submit, validate] = useTypedAction<ActionName>(actionName)
  return (
    <Formik
      validate={(values) => validate(beforeSubmit ? beforeSubmit(values) : values) }
      validateOnBlur={true}
      enableReinitialize={true}
      validateOnChange={false}
      initialValues={initialValues}
      onSubmit={async (values) => {
        onSubmit && onSubmit(values)
        const response = await submit(beforeSubmit ? beforeSubmit(values) : values)
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
       <form {...formProps}>
        {typeof children === "function" ? children(f,_) : children}
        </form>

      )}
    </Formik>)
}

