//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as TsType from "./TsType"
import * as System from "./System"

export type FSharpOption<T> = T | null
export const defaultFSharpOption: <T>(t:T) => FSharpOption<T> = <T>(t:T) => null
export type Unit = {
  
}
export const defaultUnit: Unit = {
  
}
// skipped T
// skipped TError
export type FSharpResult_Case_Ok<T> = { Case: "Ok", Fields: T }
export type FSharpResult_Case_Error<TError> = { Case: "Error", Fields: TError }
export type FSharpResult<T,TError> = FSharpResult_Case_Ok<T> | FSharpResult_Case_Error<TError>
export type FSharpResult_Case = "Ok" | "Error"
export const FSharpResult_AllCases = [ "Ok", "Error" ] as const
export const defaultFSharpResult_Case_Ok = <T>(defaultT:T) => ({ Case: "Ok", Fields: defaultT })
export const defaultFSharpResult_Case_Error = <TError>(defaultTError:TError) => ({ Case: "Error", Fields: defaultTError })
export const defaultFSharpResult = <T,TError>(t:T,tError:TError) => null as any as FSharpResult<T,TError>
