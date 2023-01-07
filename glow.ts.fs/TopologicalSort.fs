module Glow.TopologicalSort

open System
open System.Collections.Generic
open Glow.TsGen.Domain

type CyclicDependencyException(args) =
  inherit Exception(args)

type VisitResult =
  | CyclicDependencyDetected
  | New
  | AlreadyVisited

let topologicalSort (getDependencies: TsType -> TsType list) (source: TsType list) =
  let sorted = ResizeArray<TsType>()
  let visited = Dictionary<TsSignature, bool>()
  let cyclics = ResizeArray<TsType>()

  let rec visit
    (item: TsType)
    (getDependencies: TsType -> TsType list)
    (sorted: ResizeArray<TsType>)
    (visited: Dictionary<TsSignature, bool>)
    (cyclics: ResizeArray<TsType>)
    =
    let name = item.Id.OriginalName
    let alreadyVisited, inProcess = visited.TryGetValue(item.Id.TsSignature)

    if alreadyVisited then
      if inProcess then
        VisitResult.CyclicDependencyDetected
      else
        VisitResult.AlreadyVisited
    else if sorted.Contains(item) then
      VisitResult.AlreadyVisited
    else
      visited[item.Id.TsSignature] <- true
      let dependencies = getDependencies item

      dependencies
      |> List.iter (fun dependency ->
        let result = visit dependency getDependencies sorted visited cyclics

        match result with
        | CyclicDependencyDetected -> cyclics.Add(item)
        | _ -> ()

        ())

      visited[item.Id.TsSignature] <- false
      sorted.Add(item)

      if item.HasCyclicDependency then
        cyclics.Add(item)

      VisitResult.New

  source
  |> List.iter (fun item ->
    let result = visit item getDependencies sorted visited cyclics

    match result with
    | CyclicDependencyDetected -> cyclics.Add(item)
    | _ -> ())

  sorted |> Seq.toList, cyclics |> Seq.toList