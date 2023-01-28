namespace MyNamespace1

type A1={Id:int}
type A2={Id:int;A1:A1}
type A3={Id:int;A2:A2}
type A4={Id:int;A3:A3}
type A5={Id:int;  A4:A4}
type A6={Id:int;A5:A5}
type A7={Id:int;A6:A6}
type B={Result:Result<A1,string>}
type B1={Result:Result<A1,string>}
type B2={Result:Result<A1,string>}
type B3={Result:Result<A1,string>}
type B4={Result:Result<A1,string>}

type MyType5 =
  { Id: string
    Items: MyType1 list
    Option: Option<Generic<string>> }
namespace MyNamespace2

type A1={Id:int;X:MyNamespace1.A7;Y:MyNamespace1.B4 list}
type A2={Id:int;A1:A1}
type A3={Id:int;A2:A2}
type A4={Id:int;A3:A3}
type A5={Id:int;  A4:A4}
type A6={Id:int;A5:A5}
type A7={Id:int;A6:A6}
type B={Result:Result<A1,string>}
type B1={Result:Result<A1,string>}
type B2={Result:Result<A1,string>}
type B3={Result:Result<A1,string>}
type B4={Result:Result<A1,string>}

namespace MyNamespace3

type ApiError =
  |Forbidden of string
  |BadRequest of string
  |Configuration of string

type Record1 = { Id: int
                 A1: MyNamespace1.A1
                 A2: MyNamespace1.A2
                 A3: MyNamespace1.A3
                 A4: Result<MyNamespace1.A4, ApiError>
                 A5: MyNamespace1.A5 option
                 A6: MyNamespace2.A6 list
                  }

type Generic<'a> = { Id: 'a }

namespace MyNamespace2

module Tests =

  open Glow.TsGen.Domain
  open Glow.TsGen.Gen
  open Xunit

  let types = [ typedefof<MyType2>; typedefof<MyNamespace3.Record1>; ]
  
  let types2 = (typedefof<Microsoft.Graph.Account>.Assembly.GetExportedTypes() |> Seq.toList ) @ (typedefof<MeetingDomain.MeetingItemEvent>.Assembly.GetExportedTypes() |> Seq.toList )@ types

  
  let modules = generateModules types2

  let getModule name =
    modules |> List.find (fun v -> v.Name = name)

  [<Fact>]
  let ``Render many types/modules`` () =

    let x = modules |> List.map renderModule
    ()
    
