module Test.Collections

open Expecto
open Glow.TsGen.Domain
open Xunit
open Glow.TsGen.Gen

type OtherRecord = { Id: string }

type MyRecord =
  { SimpleList: int list
    ComplexList: OtherRecord list }

[<Fact>]
let ``Render record with collection properties`` () =
  let rendered = renderTypeAndValue typedefof<MyRecord>

  Expect.similar
    rendered
    """
export type MyRecord = {
  simpleList: Microsoft_FSharp_Collections.FSharpList<System.Int32>
  complexList: Microsoft_FSharp_Collections.FSharpList<OtherRecord>
}

export const defaultMyRecord: MyRecord = {
  simpleList: Microsoft_FSharp_Collections.defaultFSharpList(System.defaultInt32),
  complexList: Microsoft_FSharp_Collections.defaultFSharpList(defaultOtherRecord),
}
"""

[<Fact>]
let ``Render FSharp List`` () =
  let fsharpCoreRendered = renderTypeAndValue typedefof<List<string>>

  Expect.similar
    fsharpCoreRendered
    """export type FSharpList<T> = Array<T>
export const defaultFSharpList: <T>(t:T) => FSharpList<T> = <T>(t:T) => []
"""

[<Fact>]
let ``Render IEnumerable of KeyValue pairs`` () =
  let modules =
    generateModules
      [ typeof<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>>> ]

  let getModule name =
    modules |> List.find (fun v -> v.Name = name)

  let fs = getModule (NamespaceName "System.Collections.Generic")

  let rendered = renderModule fs
  // let rendered = renderTypeAndValue typeof<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string,string>>>

  Expect.similar
    rendered
    """//******************************************
//****This file is auto generated***********
//******************************************
import * as System from "./System"
// skipped TValue
// skipped TKey
export type KeyValuePair<TKey,TValue> = {Key:TKey,Value:TValue}
export const defaultKeyValuePair: <TKey,TValue>(tKey:TKey,tValue:TValue) => KeyValuePair<TKey,TValue> = <TKey,TValue>(tKey:TKey,tValue:TValue) => ({Key:tKey,Value:tValue})
export type IEnumerable<T> = Array<T>
export const defaultIEnumerable: <T>(t:T) => IEnumerable<T> = <T>(t:T) => []
"""

type RecordWithKeyValueList =
  { KeysAndValues: System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, int>> }

[<Fact>]
let ``Render IEnumerable of KeyValue pairs property`` () =
  let rendered = renderTypeAndValue typeof<RecordWithKeyValueList>

  Expect.similar
    rendered
    """export type RecordWithKeyValueList = {
  keysAndValues: System_Collections_Generic.IEnumerable<System_Collections_Generic.KeyValuePair<System.String,System.Int32>>
}
export const defaultRecordWithKeyValueList: RecordWithKeyValueList = {
  keysAndValues: System_Collections_Generic.defaultIEnumerable(System_Collections_Generic.defaultKeyValuePair(System.defaultString,System.defaultInt32)),
}
"""
// keysAndValues: System_Collections_Generic.defaultIEnumerable<System_Collections_Generic.KeyValuePair<System.defaultString,System.defaultInt32>>(System_Collections_Generic.defaultKeyValuePair),