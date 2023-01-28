module Test.RecordWithPrimitiveOption

open System
open Expecto
open Xunit

type RecordWithPrimitiveOption = { Id: Guid; numberOption: int option }

[<Fact>]
let ``Render record with primitive option`` () =
  let rendered = renderTypeAndValue typedefof<RecordWithPrimitiveOption>

  Expect.similar
    rendered
    """
export type RecordWithPrimitiveOption = {
  id: System.Guid
  numberOption: Microsoft_FSharp_Core.FSharpOption<System.Int32>
}
export var defaultRecordWithPrimitiveOption: RecordWithPrimitiveOption = {
 id: "00000000-0000-0000-0000-000000000000",
 numberOption: null,
}
"""
