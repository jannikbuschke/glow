namespace Test

open System.Text.RegularExpressions
open Expecto
open Glow.TsGen.Domain
open Glow.TsGen.Gen

module Regex =
  let replace (pattern: string) (replacement: string) (input: string) =
    Regex.Replace(input, pattern, replacement)

module Expect =
  let eq actual expected =
    "Should be equal" |> Expect.equal actual expected

  let private normalizeLineFeeds = Regex.replace @"(\r\n|\r|\n)" "\n"

  let private removeSuccessiveLineFeeds = Regex.replace @"[\n]{2,}" "\n"

  let private removeSuccessiveWhiteSpace = Regex.replace @"[ ]{2,}" " "

  let private trim (v: string) = v.Trim()

  let private clean =
    normalizeLineFeeds
    >> removeSuccessiveLineFeeds
    >> removeSuccessiveWhiteSpace
    >> trim

  let similar actual expected =
    "Should be equal" |> Expect.equal (actual |> clean) (expected |> clean)

[<AutoOpen>]
module Helpers =

  let renderTypeAndValue t =
    let types = [ t ]
    let modules = generateModules types
    let item = findeTsTypeInModules modules t

    let rendered = renderTypeAndDefaultValue item

    match rendered with
    | Some rendered -> rendered
    | None -> ""

  let renderTypeAndValue2 t =
    let rendered = Glow.SecondApproach.renderType t
    rendered
