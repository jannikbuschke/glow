module Test.RenderModule2

open System.Collections.Generic
open Glow.TsGen.Gen
open Expecto
open Xunit

type RecordId = RecordId of System.Guid

type OtherRecord =
  { Id: RecordId
    keyValue: KeyValuePair<string, int>
    keyValues: IEnumerable<KeyValuePair<string, int>> }

type ComplexRecord =
  { Id: System.Guid
    Name: string
    Number: int
    Items: int list
    Obj: obj
    Option: RecordId option
    Result: Result<int, string> }

let types = [ typedefof<ComplexRecord>; typeof<bool> ]

let modules = generateModules2 types

let getModule name =
  modules |> List.find (fun v -> v.Name = name)

[<Fact>]
let ``Render system module`` () =

  let fs = getModule "System"

  let sysRendered = renderModule2 fs
  System.IO.File.WriteAllText($"..\\..\\..\\{fs.Name}", sysRendered)

  Expect.similar
    sysRendered
    """
//////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////
import {TsType} from "./"
export type Boolean = boolean
export var defaultBoolean: Boolean = false
export type Guid = `${number}-${number}-${number}-${number}-${number}`
export var defaultGuid: Guid = '00000000-0000-0000-0000-000000000000'
export type String = string
export var defaultString: String = ''
export type Int32 = number
export var defaultInt32: Int32 = 0
export type Object = any
export var defaultObject: Object = {}

"""

[<Fact>]
let ``Render FS module`` () =

  let fs = getModule "Microsoft_FSharp_Core"

  let fsRendered = renderModule2 fs
  System.IO.File.WriteAllText($"..\\..\\..\\{fs.Name}", fsRendered)

  let expected =
    """
//////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////
import {TsType} from "./"
export type FSharpOption<T> = T | null
export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null

export type FSharpResult_Case_Ok<T> = { Case: "Ok", Fields: T }
export type FSharpResult_Case_Error<TError> = { Case: "Error", Fields: TError }
export type FSharpResult<T,TError> = FSharpResult_Case_Ok<T> | FSharpResult_Case_Error<TError>
export type FSharpResult_Case = "Ok" | "Error"
export var FSharpResult_AllCases = [ "Ok", "Error" ] as const
export var defaultFSharpResult_Case_Ok = <T,TError>(defaultT:T,defaultTError:TError) => ({ Case: "Ok", Fields: defaultT })
export var defaultFSharpResult_Case_Error = <T,TError>(defaultT:T,defaultTError:TError) => ({ Case: "Error", Fields: defaultTError })
export var defaultFSharpResult = <T,TError>(defaultT:T,defaultTError:TError) => defaultFSharpResult_Case_Ok(defaultT,defaultTError) as FSharpResult<T,TError>

"""

  Expect.similar fsRendered expected

[<Fact>]
let ``Render FS collections module`` () =

  let fs = getModule "Microsoft_FSharp_Collections"

  let fsRendered = renderModule2 fs
  System.IO.File.WriteAllText($"..\\..\\..\\{fs.Name}", fsRendered)

  Expect.similar
    fsRendered
    """
//////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////
import {TsType} from "./"
export type FSharpList<T> = Array<T>
export var defaultFSharpList: <T>(t:T) => FSharpList<T> = <T>(t:T) => []
"""


[<Fact>]
let ``remove whitespace`` () =
  let x = "_     _"
  Expect.similar x "_ _"

[<Fact>]
let ``normalize new line feed`` () =
  let x = "_   \n     \r\n _"
  Expect.similar "_ \n \n _" x

[<Fact>]
let ``Render Test module`` () =

  let fs = getModule "Test"

  let testRendered = renderModule2 fs

  System.IO.File.WriteAllText($"..\\..\\..\\{fs.Name}", testRendered)

  Expect.similar
    testRendered
    """
//////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////
import {TsType} from "./"
export type RecordId_Case_RecordId = System.Guid
export type RecordId = RecordId_Case_RecordId
export type RecordId_Case = "RecordId"
export var RecordId_AllCases = [ "RecordId" ] as const
export var defaultRecordId_Case_RecordId = '00000000-0000-0000-0000-000000000000'
export var defaultRecordId = defaultRecordId_Case_RecordId as RecordId
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
 id: '00000000-0000-0000-0000-000000000000',
 name: '',
 number: 0,
 items: [],
 obj: {},
 option: null,
 result: Microsoft_FSharp_Core.defaultFSharpResult(System.defaultInt32,System.defaultString),
}

"""
