module Test.DateTimesTest

open System
open Expecto
open Xunit

type SimpleRecord =
  { Id: Guid
    Name: string
    Number: int
    Obj: obj }

[<Fact>]
let ``DateTimes`` () =

  Expect.similar
    (renderTypeAndValue typedefof<DateTime>)
    """export type DateTime = `${number}-${number}-${number}T${number}:${number}:${number}`
export var defaultDateTime: DateTime = "0001-01-01T00:00:00" """
