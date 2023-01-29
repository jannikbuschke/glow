//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import {TsType} from "./"
import {System} from "./"
import {Microsoft_FSharp_Collections} from "./"
import {Microsoft_FSharp_Core} from "./"

//*** Cyclic dependencies dected ***
//*** this can cause problems when generating types and defualt values ***
//*** Please ensure that your types don't have cyclic dependencies ***
//ComplexRecord
//*** ******************* ***

export type RecordId = System.Guid
export var defaultRecordId: RecordId = System.defaultGuid
// the type ComplexRecord has cyclic dependencies
// in general this should be avoided
// Render an empty object to be filled later at end of file
// to prevent typescript errors (reference used before declaration)
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
  id: undefined as any,
  name: undefined as any,
  number: undefined as any,
  items: undefined as any,
  obj: undefined as any,
  option: undefined as any,
  result: undefined as any, }

// Render cyclic fixes
// the type ComplexRecord has cyclic dependencies
// in general this should be avoided
// fill all props
defaultComplexRecord.id = "00000000-0000-0000-0000-000000000000"
defaultComplexRecord.name = ""
defaultComplexRecord.number = 0
defaultComplexRecord.items = []
defaultComplexRecord.obj = {}
defaultComplexRecord.option = null
defaultComplexRecord.result = Microsoft_FSharp_Core.defaultFSharpResult(System.defaultInt32,System.defaultString)
