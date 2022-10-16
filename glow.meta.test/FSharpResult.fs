module Test.FSharpResult

open Expecto
open Xunit

[<Fact>]
let ``Render FSharpResult`` () =
    let rendered = renderTypeAsString typedefof<Result<string, string>>

    "Rendered FSharpResult as expected"
    |> Expect.equal
        rendered
        """
export type FSharpResult_Case_Ok<T> = { Case: "T", Fields: T }
export type FSharpResult_Case_Error<TError> = { Case: "TError", Fields: TError }
export type FSharpResult<T,TError> = FSharpResult_Case_Ok<T> | FSharpResult_Case_Error<TError>
"""
