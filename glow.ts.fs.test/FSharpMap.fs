module Test.FSharpMap

open Expecto
open Glow.Ts
open Xunit

let typedef = typedefof<Map<string, string>>
let typeof = typeof<Map<string, string>>

// https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/docs/Format.md#maps
[<Fact>]
let ``Serialize FSharpMap`` () =
  // string key is serialized as dictionary with string indexer
  let mapStringKey =
    DefaultSerialize.serialize ([ ("k1", "v1"); ("k2", "v2") ] |> Map.ofSeq)

  Expect.similar mapStringKey """{"k1":"v1","k2":"v2"}"""

  // other key types are serialized as array of arrays
  let mapIntKey = DefaultSerialize.serialize ([ (0, "v1"); (1, "v2") ] |> Map.ofSeq)

  Expect.similar mapIntKey """[[0,"v1"],[1,"v2"]]"""

  let mapComplexKey =
    DefaultSerialize.serialize ([ ((0, 0), {| Id = "FOo" |}); ((0, 1), {| Id = "FOo111" |}) ] |> Map.ofSeq)

  Expect.similar mapComplexKey """[[[0,0],{"Id":"FOo"}],[[0,1],{"Id":"FOo111"}]]"""

[<Fact>]
let ``Render FSharp String Map`` () =
  let rendered = renderTypeAndValue2 typeof

  Expect.similar
    rendered
    """export type FSharpStringMap<TValue> = { [key: string ]: TValue }
export var defaultFSharpStringMap: <TValue>(t:string,tValue:TValue) => FSharpStringMap<TValue> = <TValue>(t:string,tValue:TValue) => ({})
"""


[<Fact>]
let ``Render FSharp Map`` () =
  let rendered = renderTypeAndValue2 typedef

  Expect.similar
    rendered
    """
export type FSharpMap<TKey, TValue> = [TKey,TValue][]
export var defaultFSharpMap: <TKey, TValue>(tKey:TKey,tValue:TValue) => FSharpMap<TKey, TValue> = <TKey, TValue>(tKey:TKey,tValue:TValue) => []
"""

type Language =
  | De
  | En

type LocalizableValue<'T> =
  { Default: 'T
    Localized: Map<Language, 'T> }

[<Fact>]
let ``Render LocalizableValue`` () =
  let rendered = renderTypeAndValue2 typedefof<LocalizableValue<string>>

  Expect.similar
    rendered
    """export type LocalizableValue<T> = {
  default: T
  localized: Microsoft_FSharp_Collections.FSharpMap<Language,T>
}
export var defaultLocalizableValue: <T>(defaultT:T) => LocalizableValue<T> = <T>(defaultT:T) => ({
  default: defaultT,
  localized: [],
})
"""

type LocalizableString = LocalizableValue<string>
type Container = { Title: LocalizableString }

[<Fact>]
let ``Render LocalizableString`` () =
  let rendered = renderTypeAndValue2 typedefof<Container>

  Expect.similar
    rendered
    """export type Container = {
 title: LocalizableValue<System.String>
}
export var defaultContainer: Container = {
 title: defaultLocalizableValue(System.defaultString),
}"""
