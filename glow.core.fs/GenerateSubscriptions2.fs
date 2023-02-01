module Glow.Core.TsGen.GenerateSubscriptions2

open System.Reflection
open System.Text
open Glow.Core.TsGen
open Glow.TsGen.Domain

let render (assemblies: Assembly list) (path: string) =
  let es = GetTypes.getEvents assemblies

  let glowPath, useSubmitPath =
    if assemblies.Head.FullName.StartsWith("Glow.Core") then
      "..", ".."
    else
      "glow-core", "glow-core"

  let imports = StringBuilder()

  // imports.AppendLine($"// Assembly: {options.Assemblies.FirstOrDefault()?.FullName}")

  imports
    .AppendLine(@"import * as React from ""react""")
    .AppendLine(@"import { Form } from ""formik-antd""")
    .AppendLine(@"import mitt, { Handler, WildcardHandler } from ""mitt""")
    .AppendLine(
      @$"import {{ useNotification, useWildcardNotification }} from ""{glowPath}/lib/notifications/type-notifications"""
    )
  |> ignore
  // imports.AppendLine(@"import * as emitt from "mitt");

  let modules = Glow.TsGen.Gen.generateModules2 (es |> Seq.toList)

  modules
  |> List.iter (fun m ->
    let name = m.Name

    let sanitized = name 

    let filename = name 

    imports.AppendLine($"import * as {sanitized} from \"./{filename}\"") |> ignore)

  // let tryFind (t: System.Type) =
  //   modules
  //   |> List.collect (fun v -> v.Items)
  //   |> List.tryFind (fun v -> v.Type.FullName = t.FullName)

  imports.AppendLine("").AppendLine("export type Events = {") |> ignore

  es
  // |> Seq.choose tryFind
  |> Seq.iter (fun v ->
    let fullTypeName = v.FullName
    let name = Glow.SecondApproach.getPropertySignature "" v
    // let name = v.Id.TsSignature.FullName()

    // let fullSanitizedName = v.Id.TsSignature.FullSanitizedName()

    imports.AppendLine($"  '{fullTypeName}': {name},") |> ignore)

  imports.AppendLine("}") |> ignore

  let x =
    @"
// export const emitter = mitt<Events>();

type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
}

export type EventTable = TagWithKey<""url"", Events>

export function useSubscription<EventName extends keyof EventTable>(
    name: EventName,
    callback: Handler<Events[EventName]>,
    deps?: any[],
    ) {
    const ctx = useNotification<EventName, Events>(name, callback, deps)
    return ctx
}

export function useSubscriptions(
  callback: WildcardHandler<Events>,
  deps?: any[],
) {
  const ctx = useWildcardNotification<Events>(callback, deps)
  return ctx
}
"

  imports.AppendLine(x) |> ignore

  System.IO.File.WriteAllText(path + "subscriptions.tsx", imports.ToString())
