module Tests

open System
open Xunit
open Glow.Core.StringExtensions
open FluentAssertions

[<Fact>]
let ``simple test`` () =
  let input = @"{MeetingDate:yyyy-MM-dd dddd hh:mm}"

  let date = DateTime.Parse("2021/10/15")

  let result =
    input.ReplaceDatetimes(date, "MeetingDate")

  result.Should().Be(@"2021-10-15 Friday 12:00", null)

[<Fact>]
let ``simple test with new format`` () =
  let input = @"{MeetingDate::yyyy-MM-dd dddd hh:mm}"

  let date = DateTime.Parse("2021/10/15")

  let result =
    input.ReplaceDatetimes(date, "MeetingDate")

  result.Should().Be(@"2021-10-15 Friday 12:00", null)

[<Fact>]
let ``simple test with custom culture`` () =
  let input = @"{MeetingDate::de-DE::yyyy-MM-dd dddd hh:mm}"

  let date = DateTime.Parse("2021/10/15")

  let result =
    input.ReplaceDatetimes(date, "MeetingDate")

  result.Should().Be(@"2021-10-15 Freitag 12:00", null)


[<Fact>]
let ``test longer text`` () =
  let input =
    @"this is some {MeetingDate:yyyy hh} random text with {MeetingDate:yyyy-dd} dates sprinkeled {MeetingDate:yy-MM-dd} in {MeetingDate:yyyy/MM/dd}"

  let date = DateTime.Parse("2021/10/15")

  let result =
    input.ReplaceDatetimes(date, "MeetingDate")

  result
    .Should()
    .Be(
      @"this is some 2021 12 random text with 2021-15 dates sprinkeled 21-10-15 in 2021/10/15",
      null
    )
