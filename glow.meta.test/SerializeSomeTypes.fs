module Test.SerializeSomeTypes

open System
open System.Collections.Generic
open Glow.Ts
open Xunit
open System.Linq

type SimpleRecord={Name:string}

[<Fact>]
let ``Serialize some types`` () =
  // ["val1", 5]
  let tuple =
    DefaultSerialize.serialize ("val1", 5)

  let tuple2 =
    DefaultSerialize.serialize ("val1", 5, System.Guid.NewGuid(), {| Hello = "World" |})

  // [[3,"hello"],[2,"hello]]
  let map =
    DefaultSerialize.serialize (Map([ (3, "hello"); (2, "hello") ]))

  // "2022-12-03T01:06:02.311306+01:00"
  let datetimeoffset =
    DefaultSerialize.serialize System.DateTimeOffset.Now

  // {} ???
  let instant =
    DefaultSerialize.serialize (NodaTime.Instant.FromDateTimeUtc DateTime.UtcNow)

  // "5.00:00:00"
  let timespan = DefaultSerialize.serialize (TimeSpan.FromDays(5))

  // does not work
  // let complexDict = DefaultSerialize.serialize (System.Collections.Generic.Dictionary([KeyValuePair({Name="my record"},"string") ]))


  let enumerableKeyValues = DefaultSerialize.serialize (ResizeArray([KeyValuePair({Name="my record"},"string")]).AsEnumerable())

// Friday
  let dayOfWeek = DefaultSerialize.serialize DayOfWeek.Friday

  ()
