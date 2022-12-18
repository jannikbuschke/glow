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
      
  // let filtered = allTypes |> List.filter (fun t -> t.Namespace = "Gertrud.Configuration")
  // let modules0 =
  //   Glow.TsGen.Gen.generateModules filtered
  let modules =
    Glow.TsGen.Gen.generateModules allTypes
    
  let distinctModules = modules |> List.distinctBy(fun v-> v.Name)
  if distinctModules.Length <> modules.Length then
    failwith "modules not distinct"

  modules
  |> List.filter(fun v->v.Name |> NamespaceName.value <> "")
  |> List.iter (fun v ->
    let fs = Glow.TsGen.Gen.renderModule v
    let filePath = $"{path}{NamespaceName.filename v.Name}"
    if System.IO.File.Exists filePath then
      failwith (sprintf "error module already rendered %s" filePath)
    System.IO.File.WriteAllText(filePath, fs)
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
