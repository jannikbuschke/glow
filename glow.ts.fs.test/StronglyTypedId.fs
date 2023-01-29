module Test.StronglyTypedId

open Expecto
open Xunit

type MyRecordId = MyRecordId of System.Guid
type MyRecord = { Id: MyRecordId }

[<Fact>]
let ``Render StronglyTypedId`` () =
  let rendered = renderTypeAndValue2 typedefof<MyRecordId>

  Expect.similar
    rendered
    """
export type MyRecordId_Case_MyRecordId = System.Guid
export type MyRecordId = MyRecordId_Case_MyRecordId
export type MyRecordId_Case = "MyRecordId"
export var MyRecordId_AllCases = [ "MyRecordId" ] as const
export var defaultMyRecordId_Case_MyRecordId = '00000000-0000-0000-0000-000000000000'
export var defaultMyRecordId = defaultMyRecordId_Case_MyRecordId as MyRecordId

"""
// export type MyRecordId = System.Guid
// export var defaultMyRecordId: MyRecordId = System.defaultGuid

  let renderedValue = renderTypeAndValue2 typedefof<MyRecord>

  Expect.similar
    renderedValue
    """
export type MyRecord = {
  id: MyRecordId
}
export var defaultMyRecord: MyRecord = {
  id: defaultMyRecordId,
}
"""
