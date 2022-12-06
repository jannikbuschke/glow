module Glow.Core.TsGen.Generate

open System.Reflection
open Glow.TsGen.Domain

let renderTsTypesInternal (path: string) (assemblies: Assembly list) =
  printfn "Generate ts types"

  if not (System.IO.Directory.Exists(path)) then
    System.IO.Directory.CreateDirectory path |> ignore
    ()
  System.IO.Directory.EnumerateFiles path |> Seq.iter(fun file ->
    System.IO.File.Delete(file)
  )
  let stopWatch = System.Diagnostics.Stopwatch.StartNew()
  let es = GetTypes.getEvents assemblies
  let actions = GetTypes.getRequests assemblies

  let allTypes =
    (es |> Seq.toList)
    @ (actions
       |> Seq.map (fun v -> v.Input)
       |> Seq.toList)
      @ (actions
         |> Seq.map (fun v -> v.Output)
         |> Seq.toList)

  let modules =
    Glow.TsGen.Gen.generateModules allTypes

  modules
  |> List.filter(fun v->v.Name |> NamespaceName.value <> "")
  |> List.iter (fun v ->
    let fs = Glow.TsGen.Gen.renderModule v
    System.IO.File.WriteAllText($"{path}{NamespaceName.filename v.Name}", fs)
    ()
  )

  GenerateApi.render assemblies path
  GenerateSubscriptions.render assemblies path
  stopWatch.Stop()
  printfn "Generated time in %f ms" stopWatch.Elapsed.TotalMilliseconds
  ()

// RenderApi.Render(typeCollection, option);
// RenderSubscrptions.Render(typeCollection, option);
let renderTsTypes (assemblies: Assembly list) =
  renderTsTypesInternal ".\\web\\src\\client\\" assemblies

let renderTsTypesFromAssemblies (assemblies: Assembly seq) (path:string) =
  assemblies |> Seq.toList |> renderTsTypesInternal path
