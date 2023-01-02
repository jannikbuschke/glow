module Glow.SampleData

open System
open System.Reflection
open Glow.Ts

type SampleData =
  abstract Create: unit -> obj

module Module =
  let findTypes (assembly: Assembly) =
    let t = typeof<SampleData>

    assembly.GetTypes()
    |> Seq.filter t.IsAssignableFrom

  let instantiateSampleObjects (t: Type) =
    let sampleData = Activator.CreateInstance(t)
    let sampleData = sampleData :?> SampleData
    sampleData.Create()

  let render (serialize: Serialize) (obj: obj) = serialize obj

let generateSampleData (assemblies: Assembly seq) (serialize: Serialize) =
  let text =
    assemblies
    |> Seq.collect Module.findTypes
    |> Seq.map Module.instantiateSampleObjects
    |> Seq.map (Module.render serialize)
    |> String.concat "\n"

  System.IO.File.WriteAllText(".\web\src\client\sample-data.json", text)
