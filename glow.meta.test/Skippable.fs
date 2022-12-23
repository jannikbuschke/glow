module Test.Skippable

open System.Text.Json.Serialization
open Expecto
open Glow.Ts
open Xunit

type MyRecord = {
  SkippableProp: Skippable<string>
  SkippableOfOption: Skippable<string option>
}

// serialize: obj -> string
// deserizalize: string -> obj

[<Fact>]
let ``Serialize skippable`` () =
  // string key is serialized as dictionary with string indexer
  let skippable =
    DefaultSerialize.serialize (Skippable.Include (Some "string"))
  let skippablNonee =
    DefaultSerialize.serialize (Skippable.Include None)
    
  let skippableSkip = DefaultSerialize.serialize Skippable.Skip

  let result = DefaultSerialize.deserialize<MyRecord>("{}")
  Expect.equal result.SkippableProp Skippable.Skip "Skip"

  let result1 = DefaultSerialize.deserialize<MyRecord>("""{"SkippableOfOption":null}""")
  Expect.equal result1.SkippableOfOption (Skippable.Include(None)) "Skip"
  ()
