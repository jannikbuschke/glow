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
export const defaultRecordWithResult: RecordWithResult = {
 id: System.defaultGuid,
 result: Microsoft_FSharp_Core.defaultFSharpResult(System.defaultInt32,System.defaultString),
}
"""
