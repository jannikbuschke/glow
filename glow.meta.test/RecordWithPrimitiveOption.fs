module Test.RecordWithPrimitiveOption

open System
open Expecto
open Glow.TsGen
open Xunit

type RecordWithPrimitiveOption = { Id: Guid; numberOption: int option }

[<Fact>]
let ``Render record with primitive option`` () =

  let rendered = renderTypeAsString typedefof<RecordWithPrimitiveOption>

  "Rendered glow type as expected"
  |> Expect.equal
       rendered
       """
export type RecordWithPrimitiveOption = {
  id: Guid
  numberOption: FSharpOption<Int32>
}"""