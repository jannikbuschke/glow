namespace GlowTest

open System
open Expecto
open Glow.TsGen
open Xunit

module Tests =

  type SimpleRecord = { Id: Guid; Name: string; Number: int }

  [<Fact>]
  let ``Render simple record`` () =
    let types = [ typedefof<SimpleRecord> ]

    let modules =
      Glow.TsGen.generateModules types

    Expect.hasLength modules 1 "Module count"

    Expect.hasLength modules.Head.Items 1 "Item count"

    Expect.isEmpty modules.Head.Items.Head.Dependencies "No deps"

    let rendered = Glow.TsGen.renderType modules.Head.Items.Head

    "Rendered ts type as expected" |> Expect.equal rendered """
export type SimpleRecord = {
  id: Guid
  name: String
  number: Int32
}"""

  type RecordWithPrimitiveOption = { Id: Guid; NumberOption: int option }

  [<Fact>]
  let ``Render record with primitive option`` () =
    let types =
      [ typedefof<RecordWithPrimitiveOption> ]

    let modules =
      Glow.TsGen.generateModules types

    Expect.hasLength modules 3 "Module count"

    let fsharpCore = modules |> List.find (fun v -> v.Name = TsNamespace("Microsoft.FSharp.Core"))
    let system = modules |> List.find (fun v -> v.Name = TsNamespace("System"))
    let glowTest = modules |> List.find (fun v -> v.Name = TsNamespace("GlowTest"))

    "Item count fsharp" |> Expect.hasLength fsharpCore.Items 1
    "Item count system" |> Expect.hasLength system.Items 3
    "Item count glow" |> Expect.hasLength glowTest.Items 1

    let fsharpCoreRendered = Glow.TsGen.renderType fsharpCore.Items.Head

    Expect.hasLength modules.Head.Items 1 "Item count"

    "Rendered FSharpOption as expected" |> Expect.equal fsharpCoreRendered """
export type FSharpOption<T> = T | null
"""

    let systemRendered = system.Items |> List.map Glow.TsGen.renderType |> String.concat "\n"

    "Rendered System types as expected" |> Expect.equal systemRendered """export type Guid = string
export type Int32 = number
export type Boolean = bool"""

    Expect.isEmpty modules.Head.Items.Head.Dependencies "No deps"

    let rendered = Glow.TsGen.renderType glowTest.Items.Head

    "Rendered glow type as expected" |> Expect.equal rendered """
export type RecordWithPrimitiveOption = {
  id: Guid
  numberOption: FSharpOption<Int32>
}"""

  type RecordWithOption =
    { Id: Guid
      Option: SimpleRecord option }

  type RecordWithResult =
    { Id: Guid
      Result: Result<int, string> }

  type RecordWithResult2 =
    { Id: Guid
      Result: Result<SimpleRecord, string> }

  type RecordWithNodatimeInstant = { Id: Guid; Instant: NodaTime.Instant }

  type RecordWithPrimitiveList = { Id: Guid; StringList: string list }

  type RecordWithRecordList =
    { Id: Guid
      RecordList: SimpleRecord list }

  type RecordWithOptionalRecordList =
    { Id: Guid
      OptionalRecordList: SimpleRecord option list }

  type SingleCaseUnion = Email of string
  type RecordWithSingleCaseDu = { SingleCaseUnion: SingleCaseUnion }

  type DuWithoutTypes =
    | FirstCase
    | SecondCase
    | ThirdCase

  type ComplexDu =
    | Record of SimpleRecord
    | Record2 of RecordWithPrimitiveOption

  type MixedDu =
    | FirstCase
    | SecondCase of string
    | Record of SimpleRecord
    | Record2 of RecordWithPrimitiveOption

  type RecordWithDuWithoutFields = { Id: Guid; Du: DuWithoutTypes }

  type RecordWithComplexDu = { Id: Guid; Du: ComplexDu }

  type RecordWithMixedDu = { Id: Guid; Du: MixedDu }
