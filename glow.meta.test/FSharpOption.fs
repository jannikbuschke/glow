module Test.FSharpTypes

open Expecto
open Xunit

[<Fact>]
let ``Render FSharp Option`` () =
    let rendered = renderTypeAndValue typedefof<Option<string>>

    Expect.similar
        rendered
        """
export type FSharpOption<T> = T | null
export const defaultFSharpOption: <T>(t:T) => FSharpOption<T> = <T>(t:T) => null
"""
