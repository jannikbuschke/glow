module Test.NodaTime

open System
open System.Collections.Generic
open Glow.Ts
open Xunit
open System.Linq

[<Fact>]
let ``Serialize some nodatime types`` () =
  let instantMin =
    DefaultSerialize.serialize NodaTime.Instant.MaxValue
    
  Expect.eq instantMin "\"9999-12-31T23:59:59.999999999Z\""

  let instantNow =
    DefaultSerialize.serialize (NodaTime.Instant.FromDateTimeUtc(DateTime.UtcNow))

  let localTimeNoon =
    DefaultSerialize.serialize NodaTime.LocalTime.Noon
  Expect.eq localTimeNoon "\"12:00:00\""
  let nowAsLocalDate =
    DefaultSerialize.serialize (NodaTime.LocalDate.FromDateTime(DateTime.UtcNow))

  let localMinDate = DefaultSerialize.serialize NodaTime.LocalDate.MinIsoValue

  ()
