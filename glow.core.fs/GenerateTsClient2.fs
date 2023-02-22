module Glow.Core.TsGen.Generate2

open System
open System.Diagnostics
open System.Reflection

let renderTsTypesInternal (path: string) (assemblies: Assembly list) =
  printfn "Generate ts types"

  if not (System.IO.Directory.Exists(path)) then
    System.IO.Directory.CreateDirectory path |> ignore
    ()

  // TODO remember old files
  System.IO.Directory.EnumerateFiles path
  |> Seq.iter (fun file -> System.IO.File.Delete(file))

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

  let found =
    allTypes
    |> List.find (fun v -> v.FullName.Contains("DistributionUserViewModel"))

  let allAsText =
    allTypes
    |> List.map (fun v -> v.FullName)
    |> String.concat Environment.NewLine

  let modules = Glow.TsGen.Gen.generateModules2 allTypes

  let distinctModules = modules |> List.distinctBy (fun v -> v.Name)

  if distinctModules.Length <> modules.Length then
    failwith "modules not distinct"

  // let getDeps (m: Namespace) =
  //   let deps = Glow.TsGen.Gen.getModuleDependencies2 m
  //   deps |> List.map (fun v -> modules |> List.find (fun x -> x.Name = v))

  // let sorted, cyclics =
  //   Glow.GenericTopologicalSort.topologicalSort getDeps modules
  //
  // let sortedModules =
  //   sorted |> List.filter (fun v -> v.Name |> NamespaceName.value <> "")

  distinctModules
  |> List.iter (fun v ->
    let fs = Glow.SecondApproach.renderModule v
    let sanitizedName = v.Name

    let fileName = v.Name
    let filePath = $"{path}{fileName}.ts"

    // if System.IO.File.Exists filePath then
    //   failwith (sprintf "error module already rendered %s" filePath)

    System.IO.File.WriteAllText(filePath, fs)

    System.IO.File.AppendAllText(
      $"{path}index.ts",
      sprintf
        "import * as %s from './%s'%s"
        sanitizedName
        sanitizedName
        Environment.NewLine
    )
    // System.IO.File.AppendAllText($"{path}index.ts", sprintf "export { %s }\n" sanitizedName)
    ())

  distinctModules
  |> List.iter (fun v ->
    let sanitizedName = v.Name

    System.IO.File.AppendAllText(
      $"{path}index.ts",
      sprintf "export { %s }%s" sanitizedName Environment.NewLine
    )

    ())

  GenerateApi2.render assemblies path
  GenerateSubscriptions2.render assemblies path
  stopWatch.Stop()
  printfn "Generated time in %f ms" stopWatch.Elapsed.TotalMilliseconds
  ()

// RenderApi.Render(typeCollection, option);
// RenderSubscrptions.Render(typeCollection, option);
let renderTsTypes (assemblies: Assembly list) =
  let stopwatch = Stopwatch()
  stopwatch.Start()
  renderTsTypesInternal ".\\web\\src\\client\\" assemblies
  stopwatch.Stop()

  Console.WriteLine(
    sprintf
      "#### Generated ts client in %d ms ####"
      stopwatch.ElapsedMilliseconds
  )

let renderTsTypesFromAssemblies (assemblies: Assembly seq) (path: string) =
  Console.WriteLine("#### start generating ts client ####")
  let stopwatch = Stopwatch()
  stopwatch.Start()

  assemblies
  |> Seq.toList
  |> renderTsTypesInternal path

  stopwatch.Stop()

  Console.WriteLine(
    sprintf
      "#### Generated ts client in %d ms ####"
      stopwatch.ElapsedMilliseconds
  )
