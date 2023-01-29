module Test.EntityFrameworkTypes

open System
open Expecto
open Xunit

[<Fact>]
let ``EntityState enum`` () =
  let rendered =
    renderTypeAndValue2 typedefof<Microsoft.EntityFrameworkCore.EntityState>

  Expect.similar
    rendered
    """export type EntityState = "Detached" | "Unchanged" | "Deleted" | "Modified" | "Added"
export var EntityState_AllValues = ["Detached", "Unchanged", "Deleted", "Modified", "Added"] as const
export var defaultEntityState: EntityState = "Detached"
 """
