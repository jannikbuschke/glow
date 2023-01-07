module Glow.Core.TsGen.GetTypes

open System.Reflection
open System.Linq
open Glow.Core.Actions
open Glow.NotificationsCore
open MediatR

let getEvents (assemblies: Assembly seq) =
  assemblies
  |> Seq.collect (fun v ->
    v
      .GetExportedTypes()
      .Where(fun x ->
        x.GetInterfaces().Contains(typedefof<IClientNotification>))
    |> Seq.toList)

let getRequests (assemblies: Assembly seq) =
  assemblies
  |> Seq.collect (fun v ->
    v
      .GetExportedTypes()
      .Where(fun x ->
        x.GetCustomAttributes(typedefof<ActionAttribute>, true).Any())
    |> Seq.toList)
  |> Seq.map (fun candidate ->
    let attribute = candidate.GetCustomAttribute<ActionAttribute>()

    let interfaces = candidate.GetInterfaces()

    if interfaces.Contains(typedefof<IRequest>) then
      {| Input = candidate
         Output = typedefof<MediatR.Unit>
         ActionAttribute = attribute |}
    else
      let returnType =
        interfaces
          .FirstOrDefault(fun v ->
            v.IsGenericType
            && v.GetGenericTypeDefinition() = typedefof<IRequest<_>>)
          .GenericTypeArguments.FirstOrDefault()

      {| Input = candidate
         Output = returnType
         ActionAttribute = attribute |})
