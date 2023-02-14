module Glow.SampleData

open System
open System.Reflection
open Glow.Ts

type SampleData =
  abstract Create: unit -> (string * obj) list

module SampleData =
  let findTypes (assembly: Assembly) =
    let t = typeof<SampleData>

    assembly.GetTypes()
    |> Seq.filter t.IsAssignableFrom

  let instantiateSampleObjects (t: Type) =
    let sampleData = Activator.CreateInstance(t)
    let sampleData = sampleData :?> SampleData
    sampleData.Create()

  let render (serialize: Serialize) (data: (string * obj) list) =
    let result =
      data
      |> List.map (fun (name, obj) ->
        let typeName = Glow.SecondApproach.getName (obj.GetType())
        let moduleName = Glow.SecondApproach.getModuleName (obj.GetType())
        let propSignature = Glow.SecondApproach.getPropertySignature "" (obj.GetType())
        let importName = $"import * as {moduleName} from \"./{moduleName}\""
        importName, $""""{name}": {serialize obj} as {propSignature}""")

    result

let generateSampleData (path: string) (assemblies: Assembly seq) (serialize: Serialize) =
  let imports, definitions =
    assemblies
    |> Seq.collect SampleData.findTypes
    |> Seq.map SampleData.instantiateSampleObjects
    |> Seq.map (SampleData.render serialize)
    |> (fun x ->
      let imports =
        x
        |> Seq.collect (fun v -> v |> Seq.map fst)
        |> Seq.distinct
        |> String.concat "\n"

      let definitions =
        x
        |> Seq.collect (fun v -> v |> Seq.map snd)
        |> String.concat ",\n"

      imports, definitions)

  System.IO.File.WriteAllText(
    path,
    $"""{(imports)}

export const sample_data = {{ {definitions} }}"""
  )