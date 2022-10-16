module Glow.TopologicalSort

open System
open System.Collections.Generic

type CyclicDependencyException(args) =
  inherit Exception(args)

type VisitResult =
  | CyclicDependencyDetected
  | New
  | AlreadyVisited

let topologicalSort<'T when 'T: equality> (getDependencies: 'T -> 'T list) (source: 'T list) =
  let sorted = ResizeArray<'T>()
  let visited = Dictionary<'T, bool>()
  let cyclics = ResizeArray<'T>()

  let rec visit (item: 'T) (getDependencies: 'T -> 'T list) (sorted: ResizeArray<'T>) (visited: Dictionary<'T, bool>) (cyclics: ResizeArray<'T>) =
    let alreadyVisited, inProcess =
      visited.TryGetValue(item)

    if sorted.Contains(item) then
      VisitResult.AlreadyVisited
    else if alreadyVisited then
      if inProcess then
        VisitResult.CyclicDependencyDetected
      else
        VisitResult.AlreadyVisited
    else
      visited[item] <- true
      let dependencies = getDependencies item

      dependencies
      |> List.iter (fun dependency ->
        let result =
          visit dependency getDependencies sorted visited cyclics

        match result with
        | CyclicDependencyDetected -> cyclics.Add(item)
        | _ -> ()

        ())
      visited[item] <- false
      sorted.Add(item)
      VisitResult.New

  source
  |> List.iter (fun item ->
    let result =
      visit item getDependencies sorted visited cyclics

    match result with
    | CyclicDependencyDetected -> cyclics.Add(item)
    | _ -> ())

  sorted |> Seq.toList, cyclics |> Seq.toList
