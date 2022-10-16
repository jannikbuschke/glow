//******************************************
//****This file is auto generated***********
//******************************************
import * as System from "./System"
import * as Microsoft_FSharp_Collections from "./Microsoft_FSharp_Collections"
import * as Microsoft_FSharp_Core from "./Microsoft_FSharp_Core"
export type RecordId = System.Guid
export const defaultRecordId: RecordId = System.defaultGuid
export type ComplexRecord = {
  id: System.Guid
  name: System.String
  number: System.Int32
  items: Microsoft_FSharp_Collections.FSharpList<System.Int32>
  obj: System.Object
  option: Microsoft_FSharp_Core.FSharpOption<RecordId>
  result: Microsoft_FSharp_Core.FSharpResult<System.Int32,System.String>
}
export const defaultComplexRecord: ComplexRecord = {
  id: System.defaultGuid,
  name: System.defaultString,
  number: System.defaultInt32,
  items: Microsoft_FSharp_Collections.defaultFSharpList(System.defaultInt32),
  obj: System.defaultObject,
  option: Microsoft_FSharp_Core.defaultFSharpOption(defaultRecordId),
  result: Microsoft_FSharp_Core.defaultFSharpResult(System.defaultInt32,System.defaultString),
}
