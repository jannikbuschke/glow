module Test.RecordWithOption

open System
open Expecto
open Glow.TsGen
open Xunit
open Glow.TsGen.Gen
open Glow.TsGen.Domain

type Record = { Id: string }

type RecordWithOption =
    { Id: Guid
      NumberOption: Record option }

[<Fact>]
let ``Render record with option`` () =

    let types = [ typedefof<RecordWithOption> ]

    let modules = generateModules types

    expectAllElementsExist modules
        [ typedefof<System.Guid>
          typedefof<System.String>
          typedefof<System.Int32>
          typeof<Option<Record>> ]

    let rendered = renderTypeAsString typedefof<RecordWithOption>

    "Rendered glow type as expected"
    |> Expect.equal
        rendered
        """
export type RecordWithOption = {
  id: Guid
  numberOption: FSharpOption<Record>
}"""
