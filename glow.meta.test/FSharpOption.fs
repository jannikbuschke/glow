module Test.FSharpOption

open Expecto
open Glow.TsGen
open Xunit
open Glow.TsGen.Gen
open Glow.TsGen.Domain

[<Fact>]
let ``Render FSharp Option`` () =
  let types = [ typedefof<Option<string>> ]

  let modules = generateModules types

  let item = findeTsTypeInModules modules typedefof<Option<string>>

  let fsharpCoreRendered = renderType item

  "Rendered FSharpOption as expected"
  |> Expect.equal
       fsharpCoreRendered
       """
export type FSharpOption<T> = T | null
"""