module Test.RecordWithResultTest

open System
open Expecto
open Xunit

type RecordWithResult =
  { Id: Guid
    Result: Result<int, string> }

[<Fact>]
let ``Render record with result`` () =
  let rendered = renderTypeAndValue typedefof<RecordWithResult>

  Expect.similar
    rendered
    """
export type RecordWithResult = {
  id: System.Guid
  result: Microsoft_FSharp_Core.FSharpResult<System.Int32,System.String>
}
export var defaultRecordWithResult: RecordWithResult = {
 id: "00000000-0000-0000-0000-000000000000",
 result: Microsoft_FSharp_Core.defaultFSharpResult(System.defaultInt32,System.defaultString),
}
"""
