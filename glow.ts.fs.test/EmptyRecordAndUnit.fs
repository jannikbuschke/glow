module Test.EmptyRecordAndUnit

open System.Text.Json.Serialization
open Expecto
open Glow.TsGen.Domain
open Xunit

type EmptyRecord = { Skip: Skippable<unit> } static member Instance = { Skip = Skippable.Skip }

[<Fact>]
let ``Empty record`` () =

  let rendered = renderTypeAndValue2 typeof<EmptyRecord>

  Expect.similar rendered """
export type EmptyRecord = {
   skip: System_Text_Json_Serialization.Skippable<Microsoft_FSharp_Core.Unit>
}
export var defaultEmptyRecord: EmptyRecord = {
  skip: undefined
}
"""

type EmptyClass()=
  let x = "foo"

[<Fact>]
let ``Empty class`` () =

  let rendered = renderTypeAndValue2 typeof<EmptyClass>

  Expect.similar rendered """
export type EmptyClass = {
}
export var defaultEmptyClass: EmptyClass = {
}
"""

[<Fact>]
let ``Unit`` () =

  let rendered = renderTypeAndValue2 typeof<unit>

  Expect.similar rendered """
export type Unit = any
export var defaultUnit: Unit = ({})
"""
