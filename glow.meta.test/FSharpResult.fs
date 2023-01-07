module Test.FSharpResult

open Expecto
open Xunit

let typedef = typedefof<Result<string, string>>

[<Fact>]
let ``Render FSharpResult`` () =
  let rendered = renderTypeAndValue typedef

  Expect.similar
    rendered
    """
export type FSharpResult_Case_Ok<T> = { Case: "Ok", Fields: T }

export type FSharpResult_Case_Error<TError> = { Case: "Error", Fields: TError }

export type FSharpResult<T,TError> = FSharpResult_Case_Ok<T> | FSharpResult_Case_Error<TError>

export type FSharpResult_Case = "Ok" | "Error"
export const FSharpResult_AllCases = [ "Ok", "Error" ] as const
export const defaultFSharpResult_Case_Ok = <T>(defaultT:T) => ({ Case: "Ok", Fields: defaultT })
export const defaultFSharpResult_Case_Error = <TError>(defaultTError:TError) => ({ Case: "Error", Fields: defaultTError })
export const defaultFSharpResult = <T,TError>(t:T,tError:TError) => null as any as FSharpResult<T,TError>
"""

type MyRecord = { Name: string }

type ApiError =
  | Forbidden of string
  | BadRequest of string
  | InvalidState of string

type RecordWithResult =
  { Result: Result<MyRecord list, ApiError> }

let typedef2 = typedefof<RecordWithResult>

[<Fact>]
let ``Render FSharpResult #2`` () =
  let rendered = renderTypeAndValue typedef2

  Expect.similar
    rendered
    """
export type RecordWithResult = {
 result: Microsoft_FSharp_Core.FSharpResult<Microsoft_FSharp_Collections.FSharpList<MyRecord>,ApiError>
}
export var defaultRecordWithResult: RecordWithResult = {
 result: Microsoft_FSharp_Core.defaultFSharpResult(Microsoft_FSharp_Collections.defaultFSharpList(defaultMyRecord),defaultApiError), // wellknown type None
}
"""