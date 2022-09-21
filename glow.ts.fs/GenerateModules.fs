namespace Glow

open System

module TsGen =

  type ModuleName = ModuleName of string
  type NamespaceName = NamespaceName of string

  type TsTypeId = TsTypeId of string

  type TsNamespace = TsNamespace of string
  type TsName = TsName of string
  type GenericArguments = string list

  type TsSignature =
    { TsNamespace: TsNamespace
      TsName: TsName
      GenericArgumentTypes: TsSignature list
      GenericArguments: GenericArguments }
    member this.IsGeneric() = this.GenericArguments.Length > 0
    member this.GetName()=
      let (TsName name)=this.TsName
      name

    member this.NameWithGenericArguments() =
      let args =
        "<"
        + (this.GenericArgumentTypes
           |> Seq.map (fun v -> v.TsName)
           |> Seq.map (fun (TsName name) -> name)
           |> String.concat ",")
        + ">"

      let empty = ""
      $"{this.GetName()}{(if this.IsGeneric() then args else empty)}"

  type FullTsTypeId =
    { Id: TsTypeId
      OriginalName: string
      OriginalNamespace: NamespaceName
      TsSignature: TsSignature }

  let rec getTsSignature (t: Type) : TsSignature =

    let args0 = t.GetGenericArguments()

    let args =
      t.GetGenericArguments()
      |> Seq.toList
      |> List.mapi (fun i v -> $"T{i}")

    let name =
      if args.Length > 0 then
        t.Name.Substring(0, (t.Name.IndexOf "`"))
      else
        t.Name

    let genericArgumentTypes =
      if args.Length > 0 then
        args0
        |> Seq.toList
        |> List.map (fun v -> getTsSignature (v))
      else
        []

    match t.Namespace, t.Name with
    // | "Microsoft.FSharp.Core", "FSharpOption`1" -> { TsNamespace = TsNamespace t.Namespace; TsName = TsName name; GenericArguments = args }
    //
    // | "System", "Guid" -> { TsNamespace = TsNamespace t.Namespace; TsName = TsName t.Name; GenericArguments = args }
    // | "System", "Int32" -> { TsNamespace = TsNamespace t.Namespace; TsName = TsName t.Name; GenericArguments = args }
    // | "System", "Boolean" -> { TsNamespace = TsNamespace t.Namespace; TsName = TsName t.Name; GenericArguments = args }
    | _ ->
      { TsNamespace = TsNamespace t.Namespace
        TsName = TsName name
        GenericArgumentTypes = genericArgumentTypes
        GenericArguments = args }

  let getModuleNameAndId (arg: Type) : FullTsTypeId =
    { Id = TsTypeId arg.FullName
      TsSignature = getTsSignature arg
      OriginalName = arg.Name
      OriginalNamespace = NamespaceName arg.Namespace }

  type TsType =
    { Id: FullTsTypeId
      Type: Type
      Dependencies: FullTsTypeId list }

  type Namespace =
    { Name: TsNamespace
      Items: TsType list }

  let rec toTsType (depth: int) (t: Type) : TsType list =

    let properties =
      t.GetProperties()
      |> Seq.toList
      |> List.collect (fun v ->
        if v.PropertyType.Equals(t) then
          []
        else
          toTsType (depth + 1) v.PropertyType)
      |> List.distinct

    let result =
      if t.IsGenericType then
        let genericArgs = t.GetGenericArguments()

        let tsArgs =
          genericArgs
          |> Seq.toList
          |> List.collect (toTsType (depth + 1))

        let fullId = getModuleNameAndId t

        ({ Id = fullId
           Type = t
           Dependencies = [] }
         :: tsArgs)
        @ properties
      else
        let fullId = getModuleNameAndId t

        [ { Id = fullId
            Type = t
            Dependencies = [] } ]
        @ properties

    result

  let generateModules (types: Type list) : Namespace list =
    let result =
      types |> List.collect (toTsType 0)

    let modules =
      result
      |> List.groupBy (fun v -> v.Id.TsSignature.TsNamespace)
      |> List.map (fun (namespaceName, types) ->
        { Name = namespaceName
          Items = types |> Seq.distinct |> Seq.toList })

    modules

  let camelize (arg: string) : string =
    System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(arg)

  let renderProperties (t: TsType) : string =
    let props = t.Type.GetProperties()

    let result =
      props
      |> Seq.toList
      |> List.map (fun v ->
        let tsType = toTsType 0 v.PropertyType
        let signature = tsType.Head.Id.TsSignature

        if signature.IsGeneric() then
          let name =
            signature.NameWithGenericArguments()

          $"{camelize v.Name}: {name}"
        else
          $"{camelize v.Name}: {v.PropertyType.Name}")
      |> String.concat "\n  "

    result

  let renderType (t: TsType) : string =
    match t.Type.Namespace, t.Type.Name with
    | "Microsoft.FSharp.Core", "FSharpOption`1" ->
      """
export type FSharpOption<T> = T | null
"""
    | "System", "Guid" -> "export type Guid = string"
    | "System", "Int32" -> "export type Int32 = number"
    | "System", "Boolean" -> "export type Boolean = bool"
    | _ ->
      $"""
export type {t.Id.OriginalName} = {{
  {renderProperties t}
}}"""

  let renderModule (m: Namespace) : string =
    // topological sort
    let deps =
      m.Items
      |> List.collect (fun v -> v.Dependencies)
      |> List.filter (fun v -> v.TsSignature.TsNamespace <> m.Name)

    ""

  let FsharpResult =
    """

  type Ok<T> = {
    Case: "Ok"
    Fields: T
  }

  type Error<T> = {
    Case: "Error"
    Fields: T
  }

  type Result = Ok<TOk> | Error<TError>

  """

  let FsharpOption =
    """

  type Option<T> = T | null

  """
