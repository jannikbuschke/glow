module Test.Arrays

open Expecto
open Glow.TsGen.Domain
open Xunit

open Xunit
open System
open Expecto
open Xunit
open Glow.TsGen.Gen

type A = { Title: string }
type MyRecord = { Field1: string[]; Field2: A[] }

[<Fact>]
let ``Render MyRecord with arrays`` () =
  let rendered = renderTypeAndValue typedefof<MyRecord>

  Expect.similar
    rendered
    """
export type MyRecord = {
 field1: System_Collections_Generic.IEnumerable<System.String>
 field2: System_Collections_Generic.IEnumerable<A>
}
export var defaultMyRecord: MyRecord = {
 field1: [],
 field2: [],
}
"""

[<Fact>]
let ``Render Array definitions`` () =
  let x = typedefof<string[]>
  let y = Glow.GetTsSignature.toTsType1 0 x
  let modules = Glow.TsGen.Gen.generateModules [ typeof<MyRecord> ]

  let sysModule = modules |> List.find (fun v -> v.Name = ("System" |> NamespaceName))
  let rendered = renderModule sysModule

  Expect.similar
    rendered
    """
//////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////
import {TsType} from "./"
export type Object = any
export var defaultObject: Object = {}
export type String = string
export var defaultString: String = ""
"""
