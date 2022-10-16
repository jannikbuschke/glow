namespace GlowTest

open Glow.TsGen.Gen
open Glow.TsGen.Domain

module RecordWithResultTest =

  open System
  open Expecto
  open Glow.TsGen
  open Xunit

  type RecordWithResult =
    { Id: Guid
      Result: Result<int, string> }

  //TODO
  [<Fact>]
  let ``Render record with result`` () =
    let types = [ typedefof<RecordWithResult> ]

    let modules = generateModules types

    //        Expect.hasLength modules 3 "Module count"
    //
    //        let fsharpCore =
    //            modules
    //            |> List.find (fun v -> v.Name = TsNamespace("Microsoft.FSharp.Core"))
    //
    //        let system =
    //            modules
    //            |> List.find (fun v -> v.Name = TsNamespace("System"))
    //
    //        let glowTest =
    //            modules
    //            |> List.find (fun v -> v.Name = TsNamespace("GlowTest"))
    //
    //        "Item count fsharp"
    //        |> Expect.hasLength fsharpCore.Items 1
    //
    //        "Item count system"
    //        |> Expect.hasLength system.Items 3
    //
    //        "Item count glow"
    //        |> Expect.hasLength glowTest.Items 1
    //
    //        let fsharpCoreRendered =
    //            Glow.TsGen.renderType fsharpCore.Items.Head
    //
    //        Expect.hasLength modules.Head.Items 1 "Item count"
    //
    //        "Rendered FSharpOption as expected"
    //        |> Expect.equal
    //            fsharpCoreRendered
    //            """
    //    export type FSharpResult_OkCase<Ok> = { Case: "Ok", Fields: Ok }
    //    export type FSharpResult_ErrorCase<Error> = { Case: "Error", Fields: Error }
    //    export type FSharpResult<Ok, Error> = FSharpResult_OkCase<Ok> | FSharpResult_ErrorCase<Error>
    //    """
    //
    //        let systemRendered =
    //            system.Items
    //            |> List.map Glow.TsGen.renderType
    //            |> String.concat "\n"
    //
    //        "Rendered System types as expected"
    //        |> Expect.equal
    //            systemRendered
    //            """export type Guid = string
    //    export type Int32 = number
    //    export type Boolean = bool"""
    //
    //        Expect.isEmpty modules.Head.Items.Head.Dependencies "No deps"



    let item = findeTsTypeInModules modules typedefof<RecordWithResult>
    let rendered = renderType item

    "Rendered glow type as expected"
    |> Expect.equal
         rendered
         """
export type RecordWithResult = {
  id: Guid
  result: FSharpResult<Int32,String>
}"""