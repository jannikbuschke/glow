module SystemTypeTests

type X = {Id:string}

open Expecto
open Xunit

[<Fact>]
let ``Render many types/modules`` () =
  Expect.isTrue (typeof<System.String> = typeof<System.String>) "should be equal"
  Expect.isTrue("asd".GetType() = typeof<System.String>) "should be equal"
  let resultType = typedefof<Result<string,string>>
  let x = typedefof<X>
  let dt = typedefof<System.DateTime>
  let y = typedefof<System.Collections.Generic.IEnumerable<int>>
  ()
