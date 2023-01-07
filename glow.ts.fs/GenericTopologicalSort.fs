module Glow.GenericTopologicalSort

open System
open System.Collections.Generic
open Glow.TsGen.Domain

type CyclicDependencyException(args) =
  inherit Exception(args)

type VisitResult =
  | CyclicDependencyDetected
  | New
  | AlreadyVisited

let topologicalSort<'a when 'a: equality> (getDependencies: 'a -> 'a list) (source: 'a list) =
  let sorted = ResizeArray<'a>()
  let visited = Dictionary<'a, bool>()
  let cyclics = ResizeArray<'a>()

  let rec visit
    (item: 'a)
    (getDependencies: 'a -> 'a list)
    (sorted: ResizeArray<'a>)
    (visited: Dictionary<'a, bool>)
    (cyclics: ResizeArray<'a>)
    =
    // let name = item.Id.OriginalName
    let alreadyVisited, inProcess = visited.TryGetValue(item)

    if alreadyVisited then
      if inProcess then
        VisitResult.CyclicDependencyDetected
      else
        VisitResult.AlreadyVisited
    else if sorted.Contains(item) then
      VisitResult.AlreadyVisited
    else
      visited[item] <- true
      let dependencies = getDependencies item

      dependencies
      |> List.iter (fun dependency ->
        let result = visit dependency getDependencies sorted visited cyclics

        match result with
        | CyclicDependencyDetected -> cyclics.Add(item)
        | _ -> ()

        ())

      visited[item] <- false
      sorted.Add(item)
      // if cyclics.Contains(item) then
      // if item.HasCyclicDependency then
      // cyclics.Add(item)
      VisitResult.New

  source
  |> List.iter (fun item ->
    let result = visit item getDependencies sorted visited cyclics

    match result with
    | CyclicDependencyDetected -> cyclics.Add(item)
    | _ -> ())

  sorted |> Seq.toList, cyclics |> Seq.toList