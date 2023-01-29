//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import {TsType} from "./"
import {System} from "./"
import {Microsoft_FSharp_Collections} from "./"
import {Microsoft_FSharp_Core} from "./"

export type RecordId = System.Guid
export var defaultRecordId: RecordId = System.defaultGuid
export type ComplexRecord = {
  id: System.Guid
  name: System.String
  number: System.Int32
  items: Microsoft_FSharp_Collections.FSharpList<System.Int32>
  obj: System.Object
  option: Microsoft_FSharp_Core.FSharpOption<RecordId>
  result: Microsoft_FSharp_Core.FSharpResult<System.Int32,System.String>
}
export var defaultComplexRecord: ComplexRecord = {
  id: "00000000-0000-0000-0000-000000000000",
  name: "",
  number: 0,
  items: [],
  obj: {},
  option: null,
  result: Microsoft_FSharp_Core.defaultFSharpResult(System.defaultInt32,System.defaultString),
}
