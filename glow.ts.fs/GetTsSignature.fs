module Glow.GetTsSignature

open System
open Glow.TsGen.Domain
open Microsoft.FSharp.Reflection

let rec getTsSignature (t: Type) : TsSignature =
  let args0 = t.GetGenericArguments()

  let args =
    args0
    |> Seq.toList
    |> List.mapi (fun i v -> v.Name)

  let getNameUntilFirstGenericArgument (t: Type) =
    if args.Length > 0 then
      if t.Name.Contains "`" then
        t.Name.Substring(0, (t.Name.IndexOf "`"))
      else
        t.Name
    else
      t.Name

  let name =
    getNameUntilFirstGenericArgument t

  let genericArgumentTypes =
    if args.Length > 0 then
      args0
      |> Seq.toList
      |> List.map (fun v -> getTsSignature v)
    else
      []

  match t.Namespace, t.Name with
  | _ ->
    { TsNamespace = NamespaceName t.Namespace
      TsName = TsName name
      IsGenericParameter = t.IsGenericParameter
      ContainsGenericParameters = t.ContainsGenericParameters
      IsGenericType = t.IsGenericType
      IsGenericTypeDefinition = t.IsGenericTypeDefinition
      GenericArgumentTypes = genericArgumentTypes }

let getProperties (t: Type) : TsProperty seq =
  t.GetProperties()
  |> Seq.map (fun v ->
    { Name = v.Name
      TsType = getTsSignature v.PropertyType })


let getModuleNameAndId (arg: Type) : FullTsTypeId =
  { Id = TsTypeId arg.FullName
    TsSignature = getTsSignature arg
    OriginalName = arg.Name
    OriginalNamespace = NamespaceName arg.Namespace }

let mutable allTypes: System.Collections.Concurrent.ConcurrentDictionary<FullTsTypeId, TsType> =
  System.Collections.Concurrent.ConcurrentDictionary()

let isCollection (t: Type) =
  t.IsGenericType
  && not t.IsGenericTypeDefinition
  && t.GetGenericTypeDefinition() = typedefof<_ list>

let rec toTsType (depth: int) (t: Type) : TsType =
  let id = getModuleNameAndId t

  let toDuCase (v: UnionCaseInfo) =
    { Name = v.Name
      Tag = v.Tag
      Fields =
        v.GetFields()
        |> Seq.map (fun v ->
          { Name = v.Name
            TsType = (toTsType (depth + 1) v.PropertyType) })
        |> Seq.toList }

  if allTypes.ContainsKey(id) then
    allTypes.[id]
  elif depth > 10 then
    TsType.Any(t)
  else

    let getPropertyTypes =
      if t |> isCollection then
        []
      else
        t.GetProperties()

        |> Seq.toList
        |> List.choose (fun v ->
          if v.PropertyType.Equals(t) then
            None
          else
            let result =
              toTsType (depth + 1) v.PropertyType

            Some result)
        |> List.distinct

    let getDuCaseTypes =
      if t |> isCollection then
        []
      elif FSharpType.IsUnion(t) then
        FSharpType.GetUnionCases(t)
        |> Seq.map toDuCase
        |> Seq.toList
      else
        []

    let genericArgs = t.GetGenericArguments()

    let tsArgs =
      genericArgs
      |> Seq.map (toTsType (depth + 1))
      |> Seq.toList

    let toDependencies (t: TsType) =
      // if isGeneric, get generic definition as dependency + extract arguments
      // else, only this type as dependency
      if t.IsGenericType && not t.IsGenericTypeDefinition then
        let result =
          t.Type.GetGenericTypeDefinition()
          |> toTsType (depth + 1)

        result :: t.GenericTypeArguments
      else
        [ t ]

    let duDeps =
      getDuCaseTypes
      |> Seq.collect (fun v -> v.Fields)
      |> Seq.map (fun v -> v.TsType)
      |> Seq.toList

    let genericRuntimeArgumentDeps =
      tsArgs |> List.filter(fun v-> not v.Id.TsSignature.IsGenericParameter)

    let deps =
      duDeps @ getPropertyTypes @ genericRuntimeArgumentDeps
      |> Seq.collect toDependencies

    let result =
      { Id = id
        IsGenericType = t.IsGenericType
        IsGenericTypeDefinition = t.IsGenericTypeDefinition
        GenericTypeArguments = tsArgs
        DuCases = getDuCaseTypes
        Type = t
        Dependencies = deps |> Seq.toList }

    allTypes.TryAdd(id, result) |> ignore
    result

let isNonGenericTypeOrGenericTypeDefinition (t: TsType) =
  let t = t.Type

  let isNotGenericOrGenericTypeDefinition =
    (not t.IsGenericType)
    || (t.IsGenericType && t.IsGenericTypeDefinition)

  isNotGenericOrGenericTypeDefinition
