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
    """export type Object = {}
export const defaultObject: Object = {}
export type String = string
export const defaultString: String = ""
export type Guid = string
export const defaultGuid: Guid = "00000000-0000-0000-000000000000"
export type Boolean = boolean
export const defaultBoolean: Boolean = false
export type Char = string
export const defaultChar: Char = ''
export type Int32 = number
export const defaultInt32: Int32 = 0
"""

[<Fact>]
let ``Render FS module`` () =

  let fs = getModule (NamespaceName "Microsoft.FSharp.Core")

  let fsRendered = renderModule fs
  System.IO.File.WriteAllText($"..\\..\\..\\{NamespaceName.filename fs.Name}", fsRendered)

  let expected =
    """import * as System from "./System"

export type FSharpOption<T> = T | null
export const defaultFSharpOption: <T>(t:T) => FSharpOption<T> = <T>(t:T) => null

export type FSharpResult_Case_Ok<T> = { Case: "Ok", Fields: T }
export type FSharpResult_Case_Error<TError> = { Case: "Error", Fields: TError }
export type FSharpResult<T,TError> = FSharpResult_Case_Ok<T> | FSharpResult_Case_Error<TError>
export type FSharpResult_Case = "Ok" | "Error"
export const FSharpResult_AllCases = [ "Ok", "Error" ] as const
export const defaultFSharpResult_Case_Ok = <T>(defaultT:T) => ({ Case: "Ok", Fields: defaultT })
export const defaultFSharpResult_Case_Error = <TError>(defaultTError:TError) => ({ Case: "Error", Fields: defaultTError })
export const defaultFSharpResult = <T,TError>(t:T,tError:TError) => null as any as FSharpResult<T,TError>
"""

  Expect.similar fsRendered expected

[<Fact>]
let ``Render FS collections module`` () =

  let fs = getModule (NamespaceName "Microsoft.FSharp.Collections")

  let fsRendered = renderModule fs
  System.IO.File.WriteAllText($"..\\..\\..\\{NamespaceName.filename fs.Name}", fsRendered)

  Expect.similar
    fsRendered
    """import * as System from "./System"

export type FSharpList<T> = Array<T>

export const defaultFSharpList: <T>(t:T) => FSharpList<T> = <T>(t:T) => []
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
    """import * as System from "./System"
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
  items: Microsoft_FSharp_Collections.defaultFSharpList<System.Int32>(System.defaultInt32),
  obj: System.defaultObject,
  option: Microsoft_FSharp_Core.defaultFSharpOption<RecordId>(defaultRecordId),
  result: Microsoft_FSharp_Core.defaultFSharpResult<System.Int32,System.String>(System.defaultInt32,System.defaultString),
}
"""