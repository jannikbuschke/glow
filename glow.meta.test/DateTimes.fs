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
    """export type DateTime = string
export const defaultDateTime: DateTime = "0001-01-01T00:00:00" """