module Test.Dus

open Expecto
open Glow.Ts
open Xunit

type SimpleRecord0 = { Name: string }

type DuWithRecordFields =
  | Case1 of SimpleRecord0
  | Case2 of SimpleRecord0

[<Fact>]
let ``Du with single record fields`` () =

  Expect.similar
    (renderTypeAndValue2 typedefof<DuWithRecordFields>)
    """export type DuWithRecordFields_Case_Case1 = { Case: "Case1", Fields: SimpleRecord0 }
export type DuWithRecordFields_Case_Case2 = { Case: "Case2", Fields: SimpleRecord0 }
export type DuWithRecordFields = DuWithRecordFields_Case_Case1 | DuWithRecordFields_Case_Case2
export type DuWithRecordFields_Case = "Case1" | "Case2"
export var DuWithRecordFields_AllCases = [ "Case1", "Case2" ] as const
export var defaultDuWithRecordFields_Case_Case1 = { Case: "Case1", Fields: defaultSimpleRecord0 }
export var defaultDuWithRecordFields_Case_Case2 = { Case: "Case2", Fields: defaultSimpleRecord0 }
export var defaultDuWithRecordFields = defaultDuWithRecordFields_Case_Case1 as DuWithRecordFields

"""

type SimpleRecord = { Name: string }

type DuWithMultipleFields =
  | Case1 of System.Guid * string
  | Case2 of Foo: string * SimpleRecord * X: int32
// TODO: add generic version
[<Fact>]
let ``Du with multiple fields`` () =
  let serialized0 =
    DefaultSerialize.serialize (DuWithMultipleFields.Case1(System.Guid.NewGuid(), "Hello world"))

  let serialized1 =
    DefaultSerialize.serialize (DuWithMultipleFields.Case2("FIII", { Name = "string" }, 5))

  Expect.similar
    (renderTypeAndValue2 typedefof<DuWithMultipleFields>)
    """export type DuWithMultipleFields_Case_Case1 = { Case: "Case1", Fields: { Item1: System.Guid, Item2: System.String } }
export type DuWithMultipleFields_Case_Case2 = { Case: "Case2", Fields: { Foo: System.String, Item2: SimpleRecord, X: System.Int32 } }
export type DuWithMultipleFields = DuWithMultipleFields_Case_Case1 | DuWithMultipleFields_Case_Case2
export type DuWithMultipleFields_Case = "Case1" | "Case2"
export var DuWithMultipleFields_AllCases = [ "Case1", "Case2" ] as const
export var defaultDuWithMultipleFields_Case_Case1 = { Case: "Case1", Fields: { Item1: '00000000-0000-0000-0000-000000000000', Item2: '' } }
export var defaultDuWithMultipleFields_Case_Case2 = { Case: "Case2", Fields: { Foo: '', Item2: defaultSimpleRecord, X: 0 } }
export var defaultDuWithMultipleFields = defaultDuWithMultipleFields_Case_Case1 as DuWithMultipleFields
"""
