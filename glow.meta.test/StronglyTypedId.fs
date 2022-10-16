module Test.StronglyTypedId

open Expecto
open Xunit

type MyRecordId = MyRecordId of System.Guid
type MyRecord = { Id: MyRecordId }

[<Fact>]
let ``Render StronglyTypedId`` () =
    let rendered = renderTypeAndValue typedefof<MyRecordId>

    Expect.similar rendered """
export type MyRecordId = System.Guid
export const defaultMyRecordId: MyRecordId = System.defaultGuid
"""

    let renderedValue = renderTypeAndValue typedefof<MyRecord>

    Expect.similar
        renderedValue
        """
export type MyRecord = {
  id: MyRecordId
}
export const defaultMyRecord: MyRecord = {
  id: defaultMyRecordId,
}
"""
