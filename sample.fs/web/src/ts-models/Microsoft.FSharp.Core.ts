import { Person } from "./Sample.Fs.Agenda"
import { defaultPerson } from "./Sample.Fs.Agenda"

export type FSharpOption_Case_None<T> = null

export type FSharpOption_Case_Some<T> = {
  case: "Some",
  fields: [T]
}


export type FSharpOption<T> = FSharpOption_Case_None<T> | FSharpOption_Case_Some<T>


export const defaultFSharpOption = null

