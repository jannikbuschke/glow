module Test.Object

open Expecto
open Glow.TsGen.Domain
open Xunit
open Glow.TsGen.Gen

type A = { Data: System.Object; X: obj }

[<Fact>]
let ``Render record with obj`` () =
  let rendered = renderTypeAndValue typedefof<A>

  Expect.similar
    rendered
    """export type A = {
 data: System.Object
 x: System.Object
}
export var defaultA: A = {
 data: {},
 x: {},
}
"""

[<Fact>]
let ``Render System.Object definition`` () =
  let x = typedefof<A>

  let modules = Glow.TsGen.Gen.generateModules [ typeof<A> ]

  let sysModule = modules |> List.find (fun v -> v.Name = ("System" |> NamespaceName))

  let rendered = renderModule sysModule

  Expect.similar
    rendered
    """//////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////
import {TsType} from "./"
export type Object = any
export var defaultObject: Object = {}
"""
