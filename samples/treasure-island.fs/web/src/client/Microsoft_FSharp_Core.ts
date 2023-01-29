//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////



export type Unit = any
export var defaultUnit: Unit = ({})



export type FSharpResult_Case_Ok<T> = { Case: "Ok", Fields: T }
export type FSharpResult_Case_Error<TError> = { Case: "Error", Fields: TError }
export type FSharpResult<T,TError> = FSharpResult_Case_Ok<T> | FSharpResult_Case_Error<TError>
export type FSharpResult_Case = "Ok" | "Error"
export var FSharpResult_AllCases = [ "Ok", "Error" ] as const
export var defaultFSharpResult_Case_Ok = <T,TError>(defaultT:T,defaultTError:TError) => ({ Case: "Ok", Fields: defaultT })
export var defaultFSharpResult_Case_Error = <T,TError>(defaultT:T,defaultTError:TError) => ({ Case: "Error", Fields: defaultTError })
export var defaultFSharpResult = <T,TError>(defaultT:T,defaultTError:TError) => defaultFSharpResult_Case_Ok(defaultT,defaultTError) as FSharpResult<T,TError>


export type FSharpOption<T> = T | null
export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null

