//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import {TsType} from "./"
import {System} from "./"

// skipped TError
// skipped T
export type FSharpResult_Case_Ok<T> = { Case: "Ok", Fields: T }
export type FSharpResult_Case_Error<TError> = { Case: "Error", Fields: TError }
export type FSharpResult<T,TError> = FSharpResult_Case_Ok<T> | FSharpResult_Case_Error<TError>
export type FSharpResult_Case = "Ok" | "Error"
export var FSharpResult_AllCases = [ "Ok", "Error" ] as const
export var defaultFSharpResult_Case_Ok = <T>(defaultT:T) => ({ Case: "Ok", Fields: defaultT })
export var defaultFSharpResult_Case_Error = <TError>(defaultTError:TError) => ({ Case: "Error", Fields: defaultTError })
export var defaultFSharpResult = <T,TError>(t:T,tError:TError) => null as any as FSharpResult<T,TError>
export type FSharpOption<T> = T | null
export var defaultFSharpOption: <T>(t:T) => FSharpOption<T> = <T>(t:T) => null
