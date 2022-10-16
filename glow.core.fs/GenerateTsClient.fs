module Glow.Core.TsGen.Generate

open System.Reflection
open Glow.TsGen.Domain

let renderTsTypes (assemblies: Assembly list) =
  printfn "Generate ts types"

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
  |> List.iter (fun v ->
    let fs = Glow.TsGen.Gen.renderModule v
    System.IO.File.WriteAllText($".\\web\\src\\client\\{NamespaceName.filename v.Name}", fs)
    ()
  )

  GenerateApi.render assemblies $".\\web\\src\\client\\"
  GenerateSubscriptions.render assemblies $".\\web\\src\\client\\"
  stopWatch.Stop()
  printfn "Generated time in %f ms" stopWatch.Elapsed.TotalMilliseconds
  ()

// RenderApi.Render(typeCollection, option);
// RenderSubscrptions.Render(typeCollection, option);
