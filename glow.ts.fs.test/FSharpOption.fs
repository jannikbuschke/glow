module Test.FSharpTypes

open Expecto
open Xunit

[<Fact>]
let ``Render FSharp Option`` () =
  let rendered = renderTypeAndValue2 typedefof<Option<string>>

  Expect.similar
    rendered
    """
export type FSharpOption<T> = T | null
export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null
"""
