namespace Test

open Expecto
open Glow.TsGen.Gen
open Glow.TsGen.Domain

[<AutoOpen>]
module Helpers =

  let renderTypeAsString t =
    let types = [ t ]
    let modules = generateModules types
    let item = findeTsTypeInModules modules t
    let rendered = renderType item
    rendered

  let allItems (modules: Namespace list) =
    modules |> List.collect (fun v -> v.Items)

  let expectAllElementsExist modules elements =
    let allItems = allItems modules

    let expectTypeIdExists (items: TsType list) (typeId: FullTsTypeId) =
      $"Item {typeId.OriginalName} should exists"
      |> Expect.exists items (fun v -> v.Id = typeId)

    let expectExists = expectTypeIdExists allItems

    elements |> List.map getModuleNameAndId |> List.iter expectExists
//
open System
//
//module NewTests =
//
//  type RecordWithOption =
//    { Id: Guid
//      Option: SimpleRecord option }
//
//  type RecordWithResult =
//    { Id: Guid
//      Result: Result<int, string> }
//
//  type RecordWithResult2 =
//    { Id: Guid
//      Result: Result<SimpleRecord, string> }
//
//  type RecordWithNodatimeInstant = { Id: Guid; Instant: NodaTime.Instant }
//
//  type RecordWithPrimitiveList = { Id: Guid; StringList: string list }
//
//  type RecordWithRecordList =
//    { Id: Guid
//      RecordList: SimpleRecord list }
//
//  type RecordWithOptionalRecordList =
//    { Id: Guid
//      OptionalRecordList: SimpleRecord option list }
//
//  type SingleCaseUnion = Email of string
//  type RecordWithSingleCaseDu = { SingleCaseUnion: SingleCaseUnion }
//
//  type DuWithoutTypes =
//    | FirstCase
//    | SecondCase
//    | ThirdCase
//
//  type ComplexDu =
//    | Record of SimpleRecord
//    | Record2 of RecordWithPrimitiveOption
//
//  type MixedDu =
//    | FirstCase
//    | SecondCase of string
//    | Record of SimpleRecord
//    | Record2 of RecordWithPrimitiveOption
//
//  type RecordWithDuWithoutFields = { Id: Guid; Du: DuWithoutTypes }
//
//  type RecordWithComplexDu = { Id: Guid; Du: ComplexDu }
//
//  type RecordWithMixedDu = { Id: Guid; Du: MixedDu }