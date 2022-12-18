module Test.Arrays

open Expecto
open Glow.TsGen.Domain
open Xunit

open Xunit
open System
open Expecto
open Xunit
open Glow.TsGen.Gen

type A = {
  Title:string
}
type MyRecord = {
  Field1: string[]
  Field2: A[]
}

[<Fact>]
let ``Render MyRecord with arrays`` () =
    let rendered = renderTypeAndValue typedefof<MyRecord>

    Expect.similar rendered """export type MyRecord = {
 field1: System.Array<System.String>
 field2: System.Array<A>
}
export const defaultMyRecord: MyRecord = {
 field1: System.defaultArray(System.defaultString),
 field2: System.defaultArray(defaultA),
}"""

[<Fact>]
let ``Render Array definitions`` () =
    let x = typedefof<string[]>
    let y = Glow.GetTsSignature.toTsType1 0 x
    let modules =
      Glow.TsGen.Gen.generateModules [ typeof<MyRecord> ]
      
    let sysModule = modules |> List.find(fun v->v.Name  = ("System" |> NamespaceName))
    let rendered = renderModule sysModule

    Expect.similar rendered """//////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////
export type Array<TValue> = TValue[]
export const defaultArray: <TValue>(tValue:TValue) => Array<TValue> = <TValue>(tValue:TValue) => []
"""

