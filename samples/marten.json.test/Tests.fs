module Tests

open System
open System.Text.Json.Serialization
open Marten
open Marten.Services
open Xunit

type JsonSystem =
  | Default = 1
  | Stj = 2
  | FsStj = 3
  | FsStjCustom = 4

type SimpleRecord = { Id: Guid; Name: string; Number: int }

type RecordWithPrimitiveOption = { Id: Guid; NumberOption: int option }

type RecordWithPrimitiveList = { Id: Guid; StringList: string list }

type RecordWithRecordList =
  { Id: Guid
    RecordList: SimpleRecord list }

type RecordWithOptionalRecordList =
  { Id: Guid
    OptionalRecordList: SimpleRecord option list }

type SingleCaseDu =
  | FirstCase
  | SecondCase
  | ThirdCase

type ComplexDu =
  | Record of SimpleRecord
  | Record2 of RecordWithPrimitiveOption

type MixedDu =
  | FirstCase
  | Record of SimpleRecord
  | Record2 of RecordWithPrimitiveOption

type RecordWithSingleCaseDu = { Id: Guid; Du: SingleCaseDu }

type RecordWithComplexDu = { Id: Guid; Du: ComplexDu }

type RecordWithMixedDu = { Id: Guid; Du: MixedDu }

let getStore jsonSystem =
  let store =
    match jsonSystem with
    | JsonSystem.Default ->
      DocumentStore.For
        (fun v ->
          v.Connection($"User ID=postgres;Password=postgreS123asd123;Host=localhost;Port=5432;Database=marten.json.default;Pooling=true;Connection Lifetime=0;"))
    | JsonSystem.Stj ->
      DocumentStore.For
        (fun v ->
          v.Connection($"User ID=postgres;Password=postgreS123asd123;Host=localhost;Port=5432;Database=marten.json.stj;Pooling=true;Connection Lifetime=0;")

          let serializer = SystemTextJsonSerializer()
          v.Serializer(serializer))
    | JsonSystem.FsStj ->
      DocumentStore.For
        (fun v ->
          v.Connection($"User ID=postgres;Password=postgreS123asd123;Host=localhost;Port=5432;Database=marten.json.fsstj;Pooling=true;Connection Lifetime=0;")

          let serializer = SystemTextJsonSerializer()
          serializer.Customize(fun v -> v.Converters.Add(JsonFSharpConverter()))
          v.Serializer(serializer))
    | JsonSystem.FsStjCustom ->
      DocumentStore.For
        (fun v ->
          v.Connection($"User ID=postgres;Password=postgreS123asd123;Host=localhost;Port=5432;Database=marten.json.fsstj-custom;Pooling=true;Connection Lifetime=0;")

          let serializer = SystemTextJsonSerializer()
          //                    JsonUnionEncoding =
          serializer.Customize
            (fun v ->
              v.Converters.Add(
                JsonFSharpConverter(
                  JsonUnionEncoding.AdjacentTag
                  ||| JsonUnionEncoding.UnwrapRecordCases
                  ||| JsonUnionEncoding.UnwrapOption
                  ||| JsonUnionEncoding.UnwrapSingleCaseUnions
                  ||| JsonUnionEncoding.AllowUnorderedTag
                )
              ))

          v.Serializer(serializer))
    | _ -> failwith "not covered"



  //  store.Advanced.Clean.CompletelyRemoveAll()
  store

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Simple records are equal`` (jsonSystem: JsonSystem) =
  let store = getStore jsonSystem

  task {
    let session = store.LightweightSession()

    let e: SimpleRecord =
      { Id = Guid.NewGuid()
        Name = "Hello World"
        Number = 5 }

    session.Store(e)
    do! session.SaveChangesAsync()

    let session2 = store.LightweightSession()

    let! e2 = session2.LoadAsync<SimpleRecord>(e.Id)
    Assert.Equal(e, e2)
  }


[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Simple records are not equal`` (jsonSystem: JsonSystem) =
  let store = getStore jsonSystem

  task {
    let session = store.LightweightSession()

    let e: SimpleRecord =
      { Id = Guid.NewGuid()
        Name = "Hello World"
        Number = 5 }

    session.Store(e)
    do! session.SaveChangesAsync()

    let session2 = store.LightweightSession()

    let! e2 = session2.LoadAsync<SimpleRecord>(e.Id)
    Assert.NotEqual({ e with Name = "x" }, e2)
  }


[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with options (Some) are equal`` (jsonSystem: JsonSystem) =
  let store = getStore jsonSystem

  task {
    let session = store.LightweightSession()

    let e: RecordWithPrimitiveOption =
      { Id = Guid.NewGuid()
        NumberOption = Some(5) }

    session.Store(e)
    do! session.SaveChangesAsync()

    let session2 = store.LightweightSession()

    let! e2 = session2.LoadAsync<RecordWithPrimitiveOption>(e.Id)
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with options (None) are equal`` (jsonSystem: JsonSystem) =
  let store = getStore jsonSystem

  task {
    let session = store.LightweightSession()

    let e: RecordWithPrimitiveOption =
      { Id = Guid.NewGuid()
        NumberOption = None }

    session.Store(e)
    do! session.SaveChangesAsync()

    let session2 = store.LightweightSession()

    let! e2 = session2.LoadAsync<RecordWithPrimitiveOption>(e.Id)
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with empty primitive list`` (jsonSystem: JsonSystem) =
  let store = getStore jsonSystem

  task {
    let session = store.LightweightSession()

    let e: RecordWithPrimitiveList = { Id = Guid.NewGuid(); StringList = [] }

    session.Store(e)
    do! session.SaveChangesAsync()

    let session2 = store.LightweightSession()

    let! e2 = session2.LoadAsync<RecordWithPrimitiveList>(e.Id)
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with one-element primitive list`` (jsonSystem: JsonSystem) =
  let store = getStore jsonSystem

  task {
    let session = store.LightweightSession()

    let e: RecordWithPrimitiveList =
      { Id = Guid.NewGuid()
        StringList = [ "single element" ] }

    session.Store(e)
    do! session.SaveChangesAsync()

    let session2 = store.LightweightSession()

    let! e2 = session2.LoadAsync<RecordWithPrimitiveList>(e.Id)
    Assert.Equal(e, e2)
  }

let storeAndReload<'T> (store: IDocumentStore) (id: Guid) (obj: 'T) =
  task {
    let session = store.LightweightSession()

    session.Store(obj)
    do! session.SaveChangesAsync()

    let session2 = store.LightweightSession()

    let! e2 = session2.LoadAsync<'T>(id)
    return e2
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with several-element primitive list`` (jsonSystem: JsonSystem) =
  task {
    //    let session = store.LightweightSession()
    let store = getStore jsonSystem

    let e: RecordWithPrimitiveList =
      { Id = Guid.NewGuid()
        StringList =
          [ "single element"
            "second element"
            "third element" ] }

    let! e2 = storeAndReload store e.Id e
    //    session.Store(e)
//    do! session.SaveChangesAsync()
//
//    let session2 = store.LightweightSession()
//
//    let! e2 = session2.LoadAsync<RecordWithPrimitiveList>(e.Id)
    Assert.Equal(e, e2)
  }


[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with empty Record list`` (jsonSystem: JsonSystem) =
  task {
    let store = getStore jsonSystem
    let e: RecordWithRecordList = { Id = Guid.NewGuid(); RecordList = [] }
    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with single-item Record list`` (jsonSystem: JsonSystem) =
  task {
    let store = getStore jsonSystem

    let e: RecordWithRecordList =
      { Id = Guid.NewGuid()
        RecordList =
          [ { Id = Guid.NewGuid()
              Name = "hello"
              Number = 2 } ] }

    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with several-items Record list`` (jsonSystem: JsonSystem) =
  task {
    let store = getStore jsonSystem

    let e: RecordWithRecordList =
      { Id = Guid.NewGuid()
        RecordList =
          [ { Id = Guid.NewGuid()
              Name = "asd"
              Number = 0 }
            { Id = Guid.NewGuid()
              Name = "hello world"
              Number = 2 }
            { Id = Guid.NewGuid()
              Name = "hello xx"
              Number = 2 }
            { Id = Guid.NewGuid()
              Name = "bam"
              Number = 245844 } ] }

    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with empty optional Record list`` (jsonSystem: JsonSystem) =
  task {
    let store = getStore jsonSystem

    let e: RecordWithOptionalRecordList =
      { Id = Guid.NewGuid()
        OptionalRecordList = [] }

    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with some Nones optional Record list`` (jsonSystem: JsonSystem) =
  task {
    let store = getStore jsonSystem

    let e: RecordWithOptionalRecordList =
      { Id = Guid.NewGuid()
        OptionalRecordList = [ None; None; None ] }

    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>]
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with mixed Nones and Somes optional Record list`` (jsonSystem: JsonSystem) =
  task {
    let store = getStore jsonSystem

    let e: RecordWithOptionalRecordList =
      { Id = Guid.NewGuid()
        OptionalRecordList =
          [ None
            Some(
              { Id = Guid.NewGuid()
                Name = "x"
                Number = 10 }
            )
            None
            None
            Some(
              { Id = Guid.NewGuid()
                Name = "x"
                Number = 10 }
            )
            Some(
              { Id = Guid.NewGuid()
                Name = "x"
                Number = 10 }
            ) ] }

    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>] // System.NotSupportedException: F# discriminated union serialization is not supported. Consider authoring a custom converter for the type.
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with Single case DU`` (jsonSystem: JsonSystem) =
  task {
    let store = getStore jsonSystem

    let e: RecordWithSingleCaseDu =
      { Id = Guid.NewGuid()
        Du = SingleCaseDu.FirstCase }

    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)

    let e: RecordWithSingleCaseDu =
      { Id = Guid.NewGuid()
        Du = SingleCaseDu.SecondCase }

    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)
  }

[<Theory>]
[<InlineData(JsonSystem.Default)>]
[<InlineData(JsonSystem.Stj)>] // System.NotSupportedException: F# discriminated union serialization is not supported. Consider authoring a custom converter for the type.
[<InlineData(JsonSystem.FsStj)>]
[<InlineData(JsonSystem.FsStjCustom)>]
let ``Record with Complex case DU`` (jsonSystem: JsonSystem) =
  task {
    let store = getStore jsonSystem

    let e: RecordWithComplexDu =
      { Id = Guid.NewGuid()
        Du =
          ComplexDu.Record2(
            { Id = Guid.NewGuid()
              NumberOption = None }
          ) }

    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)

    let e: RecordWithComplexDu =
      { Id = Guid.NewGuid()
        Du =
          ComplexDu.Record(
            { Id = Guid.NewGuid()
              Name = ""
              Number = 5 }
          ) }

    let! e2 = storeAndReload store e.Id e
    Assert.Equal(e, e2)
  }
