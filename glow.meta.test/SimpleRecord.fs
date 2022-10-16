
module Test.SimpleRecordTest

open System
open Expecto
open Glow.TsGen
open Xunit

type SimpleRecord = { Id: Guid; Name: string; Number: int }

[<Fact>]
let ``Render simple record`` () =
    let rendered = renderTypeAsString typedefof<SimpleRecord>

    "Rendered ts type as expected"
    |> Expect.equal
        rendered
            """
export type SimpleRecord = {
  id: Guid
  name: String
  number: Int32
}"""
