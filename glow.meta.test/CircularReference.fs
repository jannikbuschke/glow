module Test.CircularReference

open Xunit
open System
open Expecto
open Xunit
open Glow.TsGen.Gen

// MeetingTemplate
//   -- categories: List<Category>
// CategoryTemplate
type A = { B: B }
and B = { A: A }

[<Fact>]
let ``Topological sort`` () =
  let modules =
    Glow.TsGen.Gen.generateModules [ typeof<A> ]

  let m = modules.Head

  let sorted, cyclics =
    sortItemsTopologically m.Items

  let rendered = renderModule m

  Expect.similar
    rendered
    """///////////////////////////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////////////////////////
//*** Cyclic dependencies dected ***
//*** this can cause problems when generating types and defualt values ***
//*** Please ensure that your types don't have cyclic dependencies ***
//B
//B
//*** ******************* ***
// the type B has cyclic dependencies
// in general this should be avoided
// Render an empty object to be filled later at end of file
// to prevent typescript errors (reference used before declaration)
export type B = {
 a: A
} 
export const defaultB: B = {
 a: undefined as any, }
export type A = {
 b: B
}
export const defaultA: A = {
 b: defaultB,
}
// the type B has cyclic dependencies
// in general this should be avoided
// fill all propsdefaultB.a = defaultA"""
