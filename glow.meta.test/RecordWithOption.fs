module Test.RecordWithOption

open System
open Expecto
open Xunit
open Glow.TsGen.Gen

type Record = { Id: string }

type RecordWithOption =
    { Id: Guid
      NumberOption: Record option }

[<Fact>]
let ``Render record with option`` () =

    let rendered =
        renderTypeAndValue typedefof<RecordWithOption>

    Expect.similar
        rendered
        """
export type RecordWithOption = {
  id: System.Guid
  numberOption: Microsoft_FSharp_Core.FSharpOption<Record>
}
export const defaultRecordWithOption: RecordWithOption = {
 id: System.defaultGuid,
 numberOption: Microsoft_FSharp_Core.defaultFSharpOption(defaultRecord),
}
"""
