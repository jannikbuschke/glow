namespace Marten.SystemTextJson

open Microsoft.AspNetCore.Mvc

type Person = { FirstName: string; LastName: string }

type SingleCaseDu = PersonGuid of System.Guid

type SimpleCase =
  | Case1
  | Case2
  | Case3

type ComplexCase =
  | Case1
  | Case2 of Person
  | Case3 of int

type WrappListOfOptions =
  { Persons: Person option list
    Option1: string option
    Option2: string option
    Option3: Person option
    Option4: Person option
    SingleCaseDu: SingleCaseDu
    SimpleCase: SimpleCase
    ComplexCase: ComplexCase
    ComplexCase2: ComplexCase
     }

[<Route("/")>]
type RootController() =
  inherit ControllerBase()

  [<HttpGet>]
  member this.Get() =
    { Option1 = Some("hello world")
      Option2 = None
      Option3 = Some({ FirstName = ""; LastName = "" })
      Option4 = None
      SingleCaseDu = PersonGuid(System.Guid.NewGuid())
      SimpleCase = SimpleCase.Case2
      ComplexCase = Case3 5
      ComplexCase2 = Case2 { FirstName = ""; LastName = "" }
      Persons =
        [ Some({ FirstName = ""; LastName = "" })
          None
          None
          Some(
            { FirstName = "Hello"
              LastName = "World" }
          ) ] }


[<Route("api/test")>]
type TestController() =
  inherit ControllerBase()

  [<HttpGet>]
  member this.Test() = "hello"
