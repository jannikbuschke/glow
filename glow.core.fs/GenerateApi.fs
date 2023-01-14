module Glow.Core.TsGen.GenerateApi

open System.Reflection
open System.Text
open Glow.TsGen.Domain

let render (assemblies: Assembly list) (path: string) =
  let es = GetTypes.getEvents assemblies

  let actions = GetTypes.getRequests assemblies

  let allTypes =
    (es |> Seq.toList)
    @ (actions |> Seq.map (fun v -> v.Input) |> Seq.toList)
      @ (actions |> Seq.map (fun v -> v.Output) |> Seq.toList)

  let modules = Glow.TsGen.Gen.generateModules allTypes

  let tryFind (t: System.Type) =
    modules
    |> List.collect (fun v -> v.Items)
    |> List.tryFind (fun v -> v.Type.FullName = t.FullName)

  let glowPath, useSubmitPath =
    if assemblies.Head.FullName.StartsWith("Glow.Core") then
      "..", ".."
    else
      "glow-core", "glow-core"

  let imports = StringBuilder()

  // imports.AppendLine($"// Assembly: {options.Assemblies.FirstOrDefault()?.FullName}")

  imports
    .AppendLine(@"import * as React from ""react""")
    .AppendLine(
      @"import { QueryOptions, UseQueryOptions } from ""react-query"""
    )
    // allow adjusting?
    .AppendLine(
      $@"import {{ useApi, ApiResult, useNotify }} from ""{glowPath}"""
    )
    .AppendLine(
      $@"import {{ useAction, useSubmit, UseSubmit, ProblemDetails }} from ""{useSubmitPath}"""
    )
    .AppendLine(
      @"import { Formik, FormikConfig, FormikFormProps, FormikProps } from ""formik"""
    )
    .AppendLine(@"import { Form } from ""formik-antd""")
  |> ignore

  modules
  |> List.iter (fun m ->
    if m.Name |> NamespaceName.value = "" then
      ()
    else
      let name = m.Name
      let sanitized = name |> NamespaceName.sanitize
      let filename = name |> NamespaceName.filenameWithoutExtensions

      imports.AppendLine($"import * as {sanitized} from \"{filename}\"")
      |> ignore)

  let queryInputs = StringBuilder()
  let queryOutputs = StringBuilder()
  let actionInputs = StringBuilder()
  let actionOutputs = StringBuilder()

  queryInputs.AppendLine("\nexport type QueryInputs = {") |> ignore

  queryOutputs.AppendLine("export type QueryOutputs = {") |> ignore

  actionInputs.AppendLine("export type Actions = {") |> ignore

  actionOutputs.AppendLine("export type Outputs = {") |> ignore

  let getTypeName (t: System.Type) =

    match tryFind t with
    | Some t ->
      sprintf
        "%s.%s"
        (t.Id.TsSignature.TsNamespace |> NamespaceName.sanitize)
        (t.Id.TsSignature.NameWithFullLengthGenericArguments())
    | None -> "any"

  actions
  |> Seq.iter (fun v ->
    // Console.WriteLine(v.ActionAttribute.Route);
    let outputTypeName = getTypeName v.Output
    let inputTypeName = getTypeName v.Input

    if v.Input.Name.StartsWith("Get") then
      queryOutputs.AppendLine(
        $@"  ""/{v.ActionAttribute.Route}"": {outputTypeName},"
      )
      |> ignore

      queryInputs.AppendLine(
        $@"  ""/{v.ActionAttribute.Route}"": {inputTypeName},"
      )
      |> ignore
    else
      actionOutputs.AppendLine(
        $@"  ""/{v.ActionAttribute.Route}"": {outputTypeName},"
      )
      |> ignore

      actionInputs.AppendLine(
        $@"  ""/{v.ActionAttribute.Route}"": {inputTypeName},"
      )
      |> ignore)

  queryInputs.AppendLine("}") |> ignore
  queryOutputs.AppendLine("}") |> ignore
  actionOutputs.AppendLine("}") |> ignore
  actionInputs.AppendLine("}") |> ignore

  let options = {| WithPathProxy = true |}

  let omitChildren = (if options.WithPathProxy then @"|""children""" else "")

  let proxyPathAsChildrenArgument =
    (if options.WithPathProxy then
       "  children: ((props: FormikProps<Actions[ActionName]>, pathProxy: PathProxy<Actions[ActionName], Actions[ActionName]>) => React.ReactNode) | React.ReactNode"
     else
       "")

  let children =
    if options.WithPathProxy then
      @$"       <form {{...formProps}}>
        {{typeof children === ""function"" ? children(f,_) : children}}
        </form>"
    else
      @$"        <Form {{...formProps}}>
          {{typeof children === ""function"" ? children(f) : children}}
        </Form>"

  let x =
    $@"
type TagWithKey<TagName extends string, T> = {{
  [K in keyof T]: {{ [_ in TagName]: K }} & T[K]
}};

export type ActionTable = TagWithKey<""url"", Actions>

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
) => {{
  const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
  return s
}}

type QueryTable = TagWithKey<""url"", QueryInputs>;

export function useTypedQuery<ActionName extends keyof QueryTable>(key: ActionName, {{
    placeholder,
    input,
    queryOptions
  }}: {{
    placeholder: QueryOutputs[ActionName],
    input:  QueryInputs[ActionName]
    queryOptions?: UseQueryOptions<QueryOutputs[ActionName]>
  }}): ApiResult<QueryOutputs[ActionName]> {{

  const {{ data, ...rest}} = useApi({{
    url: key,
    method:""POST"",
    payload: input,
    // todo: find defaultPlaceholder
    placeholder: placeholder,
    queryOptions: queryOptions
  }})

  const result = data as QueryOutputs[ActionName]

  return {{ data: result, ...rest}} as any
}}

export type FieldPath<P, S> = {{
  _PATH_: string & {{ _BRAND_: P & S }}
}}

export type PathProxy<P, S> = FieldPath<P, S> &
  {{ [K in keyof S]: PathProxy<P, S[K]> }}

const IdPath = {{ _PATH_: """" }} as FieldPath<any, any>

export function pathProxy<S, P = S>(
  parent: FieldPath<P, S> = IdPath as any,
): PathProxy<P, S> {{
  return new Proxy(parent as any, {{
    get(target: any, key: any) {{
      if (key in target) return target[key]
      return pathProxy<any, any>({{
        _PATH_: `${{parent._PATH_ && parent._PATH_ + "".""}}${{key}}`,
        }} as any)
    }},
}})
}}

export function TypedForm<ActionName extends keyof ActionTable>({{
  initialValues,
  actionName,
  formProps,
  children,
  onSuccess,
  beforeSubmit,
  onSubmit,
  onError,
}}: Omit<FormikConfig<Actions[ActionName]>, ""onSubmit""{omitChildren}> & {{
{proxyPathAsChildrenArgument}
  actionName: ActionName
  beforeSubmit?: (values: Actions[ActionName]) => Actions[ActionName]
  onSubmit?: (values: Actions[ActionName]) => void
  formProps?: FormikFormProps
  onSuccess?: (payload: Outputs[ActionName]) => void
  onError?: (error: ProblemDetails) => void
}}) {{
  const _ = React.useMemo(()=> pathProxy<Actions[ActionName]>(),[])
  const {{ messageSuccess, notifyError}} = useNotify()
  const [submit, validate] = useTypedAction<ActionName>(actionName)
  return (
    <Formik
      validate={{(values) => validate(beforeSubmit ? beforeSubmit(values) : values) }}
      validateOnBlur={{true}}
      enableReinitialize={{true}}
      validateOnChange={{false}}
      initialValues={{initialValues}}
      onSubmit={{async (values) => {{
        onSubmit && onSubmit(values)
        const response = await submit(beforeSubmit ? beforeSubmit(values) : values)
        if (response.ok) {{
          onSuccess && onSuccess(response.payload)
        }} else {{
          onError && onError(response.error)
          !onError && notifyError(response.error)
        }}
      }}
    }}
    >
      {{(f) => (
{children}

      )}}
    </Formik>)
}}
"

  actionInputs.AppendLine(x) |> ignore

  System.IO.File.WriteAllText(path + "api.tsx", imports.ToString())
  System.IO.File.AppendAllText(path + "api.tsx", queryInputs.ToString())
  System.IO.File.AppendAllText(path + "api.tsx", queryOutputs.ToString())
  System.IO.File.AppendAllText(path + "api.tsx", actionOutputs.ToString())
  System.IO.File.AppendAllText(path + "api.tsx", actionInputs.ToString())
  printf "Rendered api %s" path
  ()
