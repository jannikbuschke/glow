module Glow.TsGen.Domain

open System
open Glow.Ts

type NamespaceName = NamespaceName of string
type TsTypeId = TsTypeId of string
type TsName = TsName of string

module TsName =
  let create raw = TsName raw
  let value (TsName name) = name

type DuCaseSignature = DuCaseSignature of string

module DuCaseSignature =
  let value (DuCaseSignature s) = s

module NamespaceName =
  let sanitize (namespaceName: NamespaceName) : string =
    let (NamespaceName namespaceName) =
      namespaceName

    namespaceName.Replace(".", "_")

  let filenameWithoutExtensions (namespaceName: NamespaceName) : string = $"./{sanitize namespaceName}"

  let filename (namespaceName: NamespaceName) : string = $"./{sanitize namespaceName}.ts"
  let value (NamespaceName namespaceName) : string = namespaceName

type TsSignature =
  { TsNamespace: NamespaceName
    TsName: TsName
    // If this is a generic Parameter T
    IsGenericParameter: bool
    // If this is a generic type with type arguments (could be runtime arguments)
    IsGenericType: bool
    // If this is a generic type without concrete runtime arguments
    IsGenericTypeDefinition: bool
    ContainsGenericParameters: bool
    GenericArgumentTypes: TsSignature list }

  member this.Name() =
    TsName.value this.TsName

  member this.FullSanitizedName() =
    $"{(NamespaceName.sanitize this.TsNamespace)}.{this.Name()}"

  member this.IsGeneric() = this.GenericArgumentTypes.Length > 0

  member this.GetName() =
    let (TsName name) = this.TsName
    name
  member this.FullName() =
    sprintf "%s.%s" (this.TsNamespace |> NamespaceName.value ) (this.GetName())

  member this.GenericArgumentNames() =
    this.GenericArgumentTypes
    |> Seq.map (fun v -> v.Name())

    member this.GenericParametersWithNamespaces() =
    "<"
    + (this.GenericArgumentTypes
       |> Seq.map (fun v ->
         v.FullSanitizedName())
       |> String.concat ",")
    + ">"
  member this.GenericParameters() =
    "<"
    + (this.GenericArgumentTypes
       |> Seq.map (fun v -> v.Name())
       |> String.concat ",")
    + ">"

  member this.GenericArguments() =
    "("
    + (this.GenericArgumentTypes
       |> Seq.map (fun v -> $"{Utils.camelize (v.Name())}:{v.Name()}")
       |> String.concat ",")
    + ")"

  member this.GeneratorFunctionSignature() =
    this.GenericParameters() + this.GenericArguments()

  member this.NameWithFullLengthGenericArguments() =
    let args = this.GenericParametersWithNamespaces()

    let empty = ""
    $"{this.GetName()}{(if this.IsGeneric() then args else empty)}"
  member this.NameWithGenericArguments() =
    let args = this.GenericParameters()

    let empty = ""
    $"{this.GetName()}{(if this.IsGeneric() then args else empty)}"

type TsProperty = { Name: string; TsType: TsSignature }

type FullTsTypeId =
  { Id: TsTypeId
    OriginalName: string
    OriginalNamespace: NamespaceName
    TsSignature: TsSignature }

type DuCaseField = { Name: string; TsType: TsType }

and DuCase =
  { Name: string
    Tag: int
    Fields: DuCaseField list }

and TsType =
  { Id: FullTsTypeId
    IsGenericType: bool
    IsGenericTypeDefinition: bool
    GenericTypeArguments: TsType list
    Type: Type
    DuCases: DuCase list
    Dependencies: TsType list }
  static member Any(t: Type) =
    { Id =
        { Id = TsTypeId "Any"
          OriginalName = ""
          OriginalNamespace = NamespaceName ""
          TsSignature =
            { TsNamespace = NamespaceName ""
              IsGenericType = false
              ContainsGenericParameters = false
              IsGenericTypeDefinition = false
              TsName = TsName "Any"
              IsGenericParameter = false
              GenericArgumentTypes = [] } }
      IsGenericType = false
      IsGenericTypeDefinition = false
      GenericTypeArguments = []
      Type = t
      DuCases = []
      Dependencies = []
    }

type Namespace =
  { Name: NamespaceName
    Items: TsType list }
