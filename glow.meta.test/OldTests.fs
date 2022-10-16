namespace GlowTest

open System
open System.Globalization
open Expecto
open System.Linq
open Glow.Core.Typescript
open Xunit

module OldTests =

  type SimpleRecord = { Id: Guid; Name: string; Number: int }

  type RecordWithPrimitiveOption = { Id: Guid; NumberOption: int option }

  type RecordWithOption = {Id:Guid;Option: SimpleRecord option}

  type RecordWithResult = { Id:Guid;Result: Result<int,string> }
  type RecordWithResult2 = { Id: Guid;Result: Result<SimpleRecord,string> }

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

  [<Fact>]
  let ``Render primitive option as nullable`` () =
    let builder = TypeCollectionBuilder()
    builder.Add<RecordWithPrimitiveOption>()
    let collection = builder.Generate(null)
    let m = collection.Modules.First()

    let renderedModule =
      RenderTypes.RenderModule(m, null)
    let expected =
      @"export interface RecordWithPrimitiveOption {
  id: string
  numberOption: number | null
}

export const defaultRecordWithPrimitiveOption: RecordWithPrimitiveOption = {
  id: ""00000000-0000-0000-0000-000000000000"",
  numberOption: null,
}


"

    let compareResult = String.Compare(renderedModule, expected, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase ||| CompareOptions.IgnoreSymbols)

    Expect.equal compareResult 0 "compare"

  [<Fact>]
  let ``Render Single case union`` () =
    let builder = TypeCollectionBuilder()
    builder.Add<RecordWithSingleCaseDu>()
    let collection = builder.Generate(null)
    let m = collection.Modules.First()

    let renderedModule =
      RenderTypes.RenderModule(m, null)
    let expected =
      @"export interface RecordWithSingleCaseDu {
  singleCaseUnion: string | null
}

export const defaultRecordWithSingleCaseDu: RecordWithSingleCaseDu = {
  singleCaseUnion: null,
}



"

    let compareResult = String.Compare(renderedModule, expected, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase ||| CompareOptions.IgnoreSymbols)

    Expect.equal compareResult 0 "compare"


  [<Fact>]
  let ``Render ComplexDu`` () =
    let builder = TypeCollectionBuilder()
    builder.Add<RecordWithComplexDu>()
    let collection = builder.Generate(null)
    let m = collection.Modules.First()

    let renderedModule =
      RenderTypes.RenderModule(m, null)
    let expected =
      @"export interface SimpleRecord {
}

export const defaultSimpleRecord: SimpleRecord = {
  id: null,
}

export interface RecordWithPrimitiveOption {
}

export const defaultRecordWithPrimitiveOption: SimpleRecord = {
  id: null,
}

type RecordCase = {
  ""Case"":""Record"",
  Fields: SimpleRecord
}

type Record2Case = {
  ""Case"":""Record2"",
  Fields: RecordWithPrimitiveOption
}

type ComplexDu = RecordCase | Record2Case

export const defaultRecordWithSingleCaseDu: RecordWithSingleCaseDu = {
  id: null,
}

interface RecordWithComplexDu = {
  Id: string
  Du: ComplexDu
}


"

    let compareResult = String.Compare(renderedModule, expected, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase ||| CompareOptions.IgnoreSymbols)

    Expect.equal compareResult 0 "compare"


  [<Fact>]
  let ``Render Du without fields`` () =
    let builder = TypeCollectionBuilder()
    builder.Add<RecordWithDuWithoutFields>()
    let collection = builder.Generate(null)
    let m = collection.Modules.First()

    let renderedModule =
      RenderTypes.RenderModule(m, null)
    let expected =
      @"export type DuWithoutTypes_Case_FirstCase = {
  case: ""FirstCase""
}

export type DuWithoutTypes_Case_SecondCase = {
  case: ""SecondCase""
}

export type DuWithoutTypes_Case_ThirdCase = {
  case: ""ThirdCase""
}


export type DuWithoutTypes = DuWithoutTypes_Case_FirstCase | DuWithoutTypes_Case_SecondCase | DuWithoutTypes_Case_ThirdCase


export const defaultDuWithoutTypes = null as any as DuWithoutTypes


"

    let compareResult = String.Compare(renderedModule, expected, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase ||| CompareOptions.IgnoreSymbols)

    Expect.equal compareResult 0 "compare"


  [<Fact>]
  let ``Render Du with mixed cases`` () =
    let builder = TypeCollectionBuilder()
    builder.Add<RecordWithMixedDu>()
    let collection = builder.Generate(null)
    let m = collection.Modules.First()

    let renderedModule =
      RenderTypes.RenderModule(m, null)
    let expected =
      @"export type DuWithoutTypes_Case_FirstCase = {
  case: ""FirstCase""
}

export type DuWithoutTypes_Case_SecondCase = {
  case: ""SecondCase""
}

export type DuWithoutTypes_Case_ThirdCase = {
  case: ""ThirdCase""
}


export type DuWithoutTypes = DuWithoutTypes_Case_FirstCase | DuWithoutTypes_Case_SecondCase | DuWithoutTypes_Case_ThirdCase


export const defaultDuWithoutTypes = null as any as DuWithoutTypes


"

    let compareResult = String.Compare(renderedModule, expected, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase ||| CompareOptions.IgnoreSymbols)

    Expect.equal compareResult 0 "compare"


  [<Fact>]
  let ``Render Record with Option 1`` () =
    let builder = TypeCollectionBuilder()
    builder.Add<RecordWithPrimitiveOption>()
    let collection = builder.Generate(null)
    Expect.hasLength collection.Modules 2 "should be length 2"

    let m = collection.Modules.First(fun v->v.Namespace="GlowTest")
    let m = collection.Modules.First(fun v->v.Namespace="Fsharp.Core")

    let renderedModule =
      RenderTypes.RenderModule(m, null)
    let expected =
      @"export type DuWithoutTypes_Case_FirstCase = {
  case: ""FirstCase""
}

export type DuWithoutTypes_Case_SecondCase = {
  case: ""SecondCase""
}

export type DuWithoutTypes_Case_ThirdCase = {
  case: ""ThirdCase""
}


export type DuWithoutTypes = DuWithoutTypes_Case_FirstCase | DuWithoutTypes_Case_SecondCase | DuWithoutTypes_Case_ThirdCase


export const defaultDuWithoutTypes = null as any as DuWithoutTypes


"

    let compareResult = String.Compare(renderedModule, expected, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase ||| CompareOptions.IgnoreSymbols)

    Expect.equal compareResult 0 "compare"
