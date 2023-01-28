module Test.SimpleRecordTest

open System
open Expecto
open Xunit

type SimpleRecord =
  { Id: Guid
    Name: string
    Number: int
    Obj: obj }

[<Fact>]
let ``Render simple record`` () =
  let rendered = renderTypeAndValue typedefof<SimpleRecord>

  Expect.similar
    rendered
    """
export type SimpleRecord = {
  id: System.Guid
  name: System.String
  number: System.Int32
  obj: System.Object
}
export var defaultSimpleRecord: SimpleRecord = {
 id: "00000000-0000-0000-0000-000000000000",
 name: "",
 number: 0,
 obj: {},
}
"""
