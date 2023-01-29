module GetDependenciesTest

open System.Text.Json.Serialization
open Xunit
open FsUnit.Xunit

type StringDep = {
  Id: string
}

[<Fact>]
let ``single string dep`` () =
   let deps = Glow.SecondApproach.getDependencies typeof<StringDep>
   deps |> should haveLength 1
   deps |> should contain typeof<string>

type StringAndGuidDep = {
  Id: System.Guid
  Name: string
}

[<Fact>]
let ``string+guid dep`` () =
   let deps = Glow.SecondApproach.getDependencies typeof<StringAndGuidDep>
   deps |> should haveLength 2
   deps |> should contain typeof<string>
   deps |> should contain typeof<System.Guid>

type ListIntRecord = {
  List: int list
}

[<Fact>]
let ``list int dep`` () =
   let deps = Glow.SecondApproach.getDependencies typeof<ListIntRecord>
   deps |> should haveLength 2
   deps |> should contain typeof<int>
   deps |> should contain typedefof<list<_>>

type MyType = {
  Id: string
}

type SeqTypeRecord = {
  List: MyType seq
}

[<Fact>]
let ``Record seq MyType dep`` () =
   let deps = Glow.SecondApproach.getDependencies typeof<SeqTypeRecord>
   deps |> should haveLength 2
   deps |> should contain typeof<MyType>
   deps |> should contain typedefof<seq<_>>

type TransitiveGenericRecord = {
  Result: Result<int, string option>
}

[<Fact>]
let ``Record TransitiveGenericRecord`` () =
   let deps = Glow.SecondApproach.getDependencies typeof<TransitiveGenericRecord>
   deps |> should haveLength 4
   deps |> should contain typedefof<Result<_,_>>
   deps |> should contain typedefof<Option<_>>
   deps |> should contain typeof<int>
   deps |> should contain typeof<string>

type ListResultOptionChoice = {
  List: Result<Skippable<string option>,Choice<bool,int64 >> list
}

[<Fact>]
let ``Record ListResultOptionChoice`` () =
   let deps = Glow.SecondApproach.getDependencies typeof<ListResultOptionChoice>
   deps |> should haveLength 8
   deps |> should contain typedefof<Result<_,_>>
   deps |> should contain typedefof<list<_>>
   deps |> should contain typedefof<Option<_>>
   deps |> should contain typeof<int64>
   deps |> should contain typeof<string>
   deps |> should contain typeof<bool>
   deps |> should contain typedefof<Skippable<_>>
   deps |> should contain typedefof<Choice<_,_>>
