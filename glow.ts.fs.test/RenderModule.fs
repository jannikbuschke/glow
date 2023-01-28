module Test.RenderModule

open System.Collections.Generic
open Glow.TsGen.Domain
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

let types = [ typedefof<ComplexRecord> ]

let modules = generateModules types

let getModule name =
  modules |> List.find (fun v -> v.Name = name)

[<Fact>]
let ``Render system module`` () =

  let fs = getModule (NamespaceName "System")

  let sysRendered = renderModule fs
  System.IO.File.WriteAllText($"..\\..\\..\\{NamespaceName.filename fs.Name}", sysRendered)

  Expect.similar
    sysRendered
    """
//////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////
import {TsType} from "./"
export type Object = any
export var defaultObject: Object = {}
export type Boolean = boolean
export var defaultBoolean: Boolean = false
export type Int32 = number
export var defaultInt32: Int32 = 0
export type String = string
export var defaultString: String = ""
export type Guid = `${number}-${number}-${number}-${number}-${number}`
export var defaultGuid: Guid = "00000000-0000-0000-0000-000000000000"

"""

[<Fact>]
let ``Render FS module`` () =

  let fs = getModule (NamespaceName "Microsoft.FSharp.Core")

  let fsRendered = renderModule fs
  System.IO.File.WriteAllText($"..\\..\\..\\{NamespaceName.filename fs.Name}", fsRendered)

  let expected =
    """
//////////////////////////////////////
// This file is auto generated //
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

"""

  Expect.similar fsRendered expected

[<Fact>]
let ``Render FS collections module`` () =

  let fs = getModule (NamespaceName "Microsoft.FSharp.Collections")

  let fsRendered = renderModule fs
  System.IO.File.WriteAllText($"..\\..\\..\\{NamespaceName.filename fs.Name}", fsRendered)

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

  let fs = getModule (NamespaceName "Test")

  let testRendered = renderModule fs

  System.IO.File.WriteAllText($"..\\..\\..\\{NamespaceName.filename fs.Name}", testRendered)

  Expect.similar
    testRendered
    """
//////////////////////////////////////
// This file is auto generated //
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

"""
