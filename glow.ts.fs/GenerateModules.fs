module Glow.TsGen.Gen

open System
open Domain

type TsSignature =
    { TsNamespace: TsNamespace
      TsName: TsName
      IsGenericParameter: bool
      GenericArgumentTypes: TsSignature list
      GenericArguments: GenericArguments }
    member this.IsGeneric() = this.GenericArguments.Length > 0

    member this.GetName() =
        let (TsName name) = this.TsName
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
      TsSignature: TsSignature
      IsGenericParameter: bool }

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
    | _ ->
        { TsNamespace = TsNamespace t.Namespace
          TsName = TsName name
          IsGenericParameter = t.IsGenericParameter
          GenericArgumentTypes = genericArgumentTypes
          GenericArguments = args }

let getModuleNameAndId (arg: Type) : FullTsTypeId =
    { Id = TsTypeId arg.FullName
      TsSignature = getTsSignature arg
      OriginalName = arg.Name
      IsGenericParameter = arg.IsGenericParameter
      OriginalNamespace = NamespaceName arg.Namespace }

type TsType =
    { Id: FullTsTypeId
      Type: Type
      Dependencies: FullTsTypeId list }

type Namespace =
    { Name: TsNamespace
      Items: TsType list }

let rec toTsType (depth: int) (t: Type) : TsType list =

    let duDependencies =
        if FSharp.Reflection.FSharpType.IsUnion(t) then
            let cases =
                FSharp.Reflection.FSharpType.GetUnionCases(t)

            let deps =
                cases
                |> Seq.collect (fun v -> v.GetFields())
                |> Seq.map (fun v -> v.PropertyType)
                |> Seq.collect (toTsType (depth + 1))
                |> Seq.toList

            deps
        else
            []

    let properties =
        t.GetProperties()
        |> Seq.toList
        |> List.collect
            (fun v ->
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

            // use generic typedef
//        let t = t.GetGenericTypeDefinition()

            let fullId = getModuleNameAndId t

            ({ Id = fullId
               Type = t
               Dependencies = [] }
             :: tsArgs)
        else
            let fullId = getModuleNameAndId t

            [ { Id = fullId
                Type = t
                Dependencies = [] } ]

    result @ properties @ duDependencies

let generateModules (types: Type list) : Namespace list =
    let result = types |> List.collect (toTsType 0)

    let modules =
        result
        |> List.groupBy (fun v -> v.Id.TsSignature.TsNamespace)
        |> List.map
            (fun (namespaceName, types) ->
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
        |> List.map
            (fun v ->
                let tsType = toTsType 0 v.PropertyType |> List.head
                let signature = tsType.Id.TsSignature

                if signature.IsGeneric() then
                    let name = signature.NameWithGenericArguments()

                    $"{camelize v.Name}: {name}"
                else
                    $"{camelize v.Name}: {v.PropertyType.Name}")
        |> String.concat (System.Environment.NewLine + "  ")

    result

let renderDuCases (t: TsType) : string =
    let cases =
        FSharp.Reflection.FSharpType.GetUnionCases(t.Type)

    let (TsName duName) = t.Id.TsSignature.TsName

    let cases =
        match cases.Length with
        | 1 -> failwith ("not yet supported")
        | x ->
            cases
            |> Seq.toList
            |> List.map
                (fun v ->
                    let declaringType = v.DeclaringType

                    let fieldSignature =
                        v.GetFields()
                        |> Seq.toList
                        |> List.map (fun v -> getTsSignature (v.PropertyType))
                        |> List.head

                    if fieldSignature.IsGenericParameter then
                        let (TsName name) = fieldSignature.TsName

                        let caseSignature =
                            DuCaseSignature $"""{duName}_Case_{v.Name}<{name}>"""

                        caseSignature,
                        $"""export type {(DuCaseSignature.value caseSignature)} = {{ Case: "{name}", Fields: {name} }}"""
                    else
                        let caseSignature =
                            DuCaseSignature $"""{duName}_Case_{v.Name}"""

                        caseSignature, $"export type {DuCaseSignature.value caseSignature}")

    let renderedCases =
        cases
        |> (List.map snd)
        |> String.concat System.Environment.NewLine

    let renderedDuCaseSignatures =
        cases
        |> List.map fst
        |> List.map (fun (DuCaseSignature v) -> v)
        |> String.concat " | "

    let du = $"""export type {t.Id.TsSignature.NameWithGenericArguments()}"""

    renderedCases
    + System.Environment.NewLine
    + $"{du} = {renderedDuCaseSignatures}"
    + System.Environment.NewLine

let renderDu (t: TsType) : string =
    let cases = renderDuCases t
    cases

let renderTypeInternal (t: TsType) : string =
    match t.Type.Namespace, t.Type.Name, FSharp.Reflection.FSharpType.IsUnion(t.Type) with
    | "Microsoft.FSharp.Core", "FSharpOption`1", _ ->
        """
export type FSharpOption<T> = T | null
"""
    | "System", "Guid", _ -> "export type Guid = string"
    | "System", "Int32", _ -> "export type Int32 = number"
    | "System", "Boolean", _ -> "export type Boolean = bool"
    | _, _, true -> System.Environment.NewLine + renderDu t
    | _ ->
        $"""
export type {t.Id.OriginalName} = {{
  {renderProperties t}
}}"""

let renderType t =
    let result = renderTypeInternal t

    let result =
        result.Replace("\r\n", System.Environment.NewLine)

    result

let renderModule (m: Namespace) : string =
    // topological sort
    let deps =
        m.Items
        |> List.collect (fun v -> v.Dependencies)
        |> List.filter (fun v -> v.TsSignature.TsNamespace <> m.Name)

    ""

let findeTsTypeInModules modules t =
    let id = (getModuleNameAndId t).Id

    modules
    |> List.collect (fun v -> v.Items)
    |> List.find (fun v -> v.Id.Id = id)

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
