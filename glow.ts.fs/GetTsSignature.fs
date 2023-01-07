module Glow.GetTsSignature

open System
open Glow.TsGen.Domain
open Microsoft.FSharp.Reflection

// GERTRUD.GENERIC.FIELDS imports are missing
// AgendaItemVariant is skipped

let rec getTsSignature (t: Type) : TsSignature =
  if t.IsArray then
    let elementType = t.GetElementType()

    { TsNamespace = NamespaceName "System"
      TsName = TsName "Array"
      IsGenericParameter = false
      ContainsGenericParameters = true
      IsGenericType = true
      IsGenericTypeDefinition = false
      GenericArgumentTypes = [getTsSignature elementType]}
  else
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
  if arg.IsArray then
    { Id = TsTypeId "System.Array"
      TsSignature = getTsSignature arg
      OriginalName = arg.Name
      OriginalNamespace = arg.Namespace
       }
  else
    { Id = TsTypeId arg.FullName
      TsSignature = getTsSignature arg
      OriginalName = arg.Name
      OriginalNamespace = arg.Namespace }

type RenderedDuCaseDefinitionAndValue =
  { Name: string
    GenericName: string option
    CaseName: string
    Definition: string
    DefaultValue: string }

type Renderable =
  | TypeAndValue of name: string * definition: string * defaultValue: string * inlineValue: string option
  | CyclicTypeAndStubValue of name: string * definition: string * stubValue: string
  | CyclicFixValue of name: string * fixReferences: string
  | GenericTypeAndFunction of
    name: string *
    genericName: string *
    definition: string *
    defaultGeneratorSignature: string *
    defaultGeneratorImpl: string *
    inlineValue: string option
  | Enum of name: string * values: string list
  | DiscriminatedUnion of name: string * genericName: string option * cases: RenderedDuCaseDefinitionAndValue list
  | NotRenderable

let mutable allTypes: System.Collections.Concurrent.ConcurrentDictionary<FullTsTypeId, TsType> =
  System.Collections.Concurrent.ConcurrentDictionary()

module DefaultTypeDefinitionsAndValues =
  type Dict<'a, 'b> = System.Collections.Concurrent.ConcurrentDictionary<'a, 'b>

  let tryGetExistingTypeDefinition =
    let anyType = TsType.Any(typedefof<_>)
    allTypes.TryAdd(anyType.Id, anyType) |> ignore

    let types =
      Dict<System.Type, string * string option * string * string option * string * bool>()

    let add v = types.TryAdd v |> ignore
    add (typeof<System.String>, ("String", None, "string", None, "\"\"", true))

    add (
      typeof<NodaTime.LocalDate>,
      ("LocalDate", None, "`${number}-${number}-${number}`", None, "\"\2022-12-18\"", true)
    )

    add (typeof<NodaTime.LocalTime>, ("LocalTime", None, "`${number}:${number}:${number}`", None, "\"00:00:00\"", true))

    add (
      typeof<NodaTime.Instant>,
      ("Instant",
       None,
       "`${number}-${number}-${number}T${number}:${number}:${number}.${number}Z`",
       None,
       "\"9999-12-31T23:59:59.999999999Z\"",
       true)
    )

    add (typeof<System.Byte>, ("Byte", None, "number", None, "0", true))
    add (typedefof<System.Int16>, ("Int16", None, "number", None, "0", true))
    add (typeof<System.Decimal>, ("Decimal", None, "number", None, "0", true))
    add (typedefof<_ list>, ("FSharpList", Some "FSharpList<T>", "Array<T>", Some "<T>(t:T)", "[]", true))
    add (typedefof<_ option>, ("FSharpOption", Some "FSharpOption<T>", "T | null", Some "<T>(t:T)", "null", true))
    add (typedefof<Nullable<_>>, ("Nullable", Some "Nullable<T>", "T | null", Some "<T>(t:T)", "null", true))

    add (typedefof<System.Type>, ("Type", None, "{}", None, "{}", true))

    add (
      typedefof<System.Collections.Generic.IEnumerable<_>>,
      ("IEnumerable", Some "IEnumerable<T>", "Array<T>", Some "<T>(t:T)", "[]", true)
    )

    add (
      typedefof<System.Collections.Generic.IList<_>>,
      ("IList", Some "IList<T>", "Array<T>", Some "<T>(t:T)", "[]", true)
    )

    add (
      typedefof<System.Collections.Generic.ICollection<_>>,
      ("ICollection", Some "ICollection<T>", "Array<T>", Some "<T>(t:T)", "[]", true)
    )

    add (
      typedefof<System.Collections.Generic.IDictionary<_, _>>,
      ("IDictionary",
       Some "IDictionary<TKey, TValue>",
       "{ [key: string]: TValue }",
       Some "<TKey, TValue>(t:TKey,tValue:TValue)",
       "({})",
       true)
    )

    add (
      typedefof<FSharp.Collections.Map<_, _>>,
      ("FSharpMap",
       Some "FSharpMap<TKey, TValue>",
       "[TKey,TValue][]",
       Some "<TKey, TValue>(tKey:TKey,tValue:TValue)",
       "[]",
       true)
    )

    add (
      typedefof<System.Text.Json.Serialization.Skippable<_>>,
      ("Skippable", Some "Skippable<T>", "T | undefined", Some "<T>(t:T)", "undefined", true)
    )

    add (
      typedefof<System.Collections.Generic.Dictionary<_, _>>,
      ("Dictionary",
       Some "Dictionary<TKey, TValue>",
       "{ [key: string]: TValue }",
       Some "<TKey, TValue>(t:TKey,tValue:TValue)",
       "({})",
       true)
    )

    add (
      typedefof<System.Collections.Generic.IReadOnlyList<_>>,
      ("IReadOnlyList", Some "IReadOnlyList<T>", "Array<T>", Some "<T>(t:T)", "([])", true)
    )

    add (
      typedefof<System.Collections.Generic.List<_>>,
      ("List", Some "List<T>", "Array<T>", Some "<T>(t:T)", "([])", true)
    )

    add (
      typedefof<System.Collections.Generic.KeyValuePair<_, _>>,
      ("KeyValuePair",
       Some "KeyValuePair<TKey,TValue>",
       "{Key:TKey,Value:TValue}",
       Some "<TKey,TValue>(tKey:TKey,tValue:TValue)",
       "({Key:tKey,Value:tValue})",
       false)
    )

    add (
      typeof<System.DateTimeOffset>,
      ("DateTimeOffset",
       None,
       "`${number}-${number}-${number}T${number}:${number}:${number}${\"+\"|\"-\"}${number}:${number}`",
       None,
       "\"0000-00-00T00:00:00+00:00\"",
       true)
    )

    add (typeof<System.String>, ("String", None, "string", None, "\"\"", true))
    add (typeof<System.TimeSpan>, ("TimeSpan", None, "`${number}:${number}:${number}`", None, "\"00:00:00\"", true))

    add (
      typeof<System.DateTime>,
      ("DateTime",
       None,
       "`${number}-${number}-${number}T${number}:${number}:${number}`",
       None,
       "\"0001-01-01T00:00:00\"",
       true)
    )

    add (typeof<System.DateTimeOffset>, ("DateTimeOffset", None, "string", None, "", true))
    add (typeof<System.Char>, ("Char", None, "string", None, @"''", true))

    add (
      typeof<System.Guid>,
      ("Guid", None, "`${number}-${number}-${number}-${number}`", None, @"""00000000-0000-0000-000000000000""", true)
    )

    add (typeof<System.Int32>, ("Int32", None, "number", None, "0", true))
    add (typeof<System.Int64>, ("Int64", None, "number", None, "0", true))
    add (typeof<System.Double>, ("Double", None, "number", None, "0", true))
    add (typeof<System.Boolean>, ("Boolean", None, "boolean", None, "false", true))
    add (typeof<System.Object>, ("Object", None, "any", None, "{}", true))

    fun (t: System.Type) ->
      let isFSharpStringMap (t: System.Type) =
        t.Name.StartsWith("FSharpMap")
        && t.GenericTypeArguments.Length > 0
        && t.GenericTypeArguments.[0].Name = "String"

      let isArray (t: System.Type) = t.IsArray

      if isArray t then
        Some(
          Renderable.GenericTypeAndFunction(
            name = "Array",
            genericName = "Array<TValue>",
            definition = "TValue[]",
            defaultGeneratorSignature = "<TValue>(tValue:TValue)",
            defaultGeneratorImpl = "[]",
            inlineValue = Some "[]"
          )
        )
      else if isFSharpStringMap t then
        Some(
          Renderable.GenericTypeAndFunction(
            name = "FSharpStringMap",
            genericName = "FSharpStringMap<TValue>",
            definition = "{ [key: string ]: TValue }",
            defaultGeneratorSignature = "<TValue>(t:string,tValue:TValue)",
            defaultGeneratorImpl = "({})",
            inlineValue = Some "({})"
          )
        )

      elif types.ContainsKey t then
        let name, genericName, definition, defaultValueSignature, defaultValue, isInlinable =
          types.[t]

        match genericName with
        | Some genericName ->
          Some(
            Renderable.GenericTypeAndFunction(
              name = name,
              genericName = genericName,
              definition = definition,
              defaultGeneratorSignature = defaultValueSignature.Value,
              defaultGeneratorImpl = defaultValue,
              inlineValue = if isInlinable then Some defaultValue else None
            )
          )
        | None ->
          Some(
            Renderable.TypeAndValue(
              name = name,
              definition = definition,
              defaultValue = defaultValue,
              inlineValue = if isInlinable then Some defaultValue else None
            )
          )
      else
        None

let isCollection (t: Type) =
  t.IsGenericType
  && not t.IsGenericTypeDefinition
  && t.GetGenericTypeDefinition() = typedefof<_ list>

let mutable visiting = System.Collections.Concurrent.ConcurrentDictionary<FullTsTypeId,bool>()
type Dependency =
  | CicylcDependency of TsType
  | AlreadyVisited of TsType

let rec getArgumentsRecursively (t: Type) =
  if t.IsGenericType && not t.IsGenericTypeDefinition then
    let genericDefinition = t.GetGenericTypeDefinition()
    let genericArgs = t.GetGenericArguments()
    genericDefinition::(genericArgs |> Seq.toList) @ (genericArgs
                    |> Seq.collect getArgumentsRecursively
                    |> Seq.toList)
  else
    []


let rec toTsType (depth: int) (t: Type) : Result<TsType, Dependency> =
  let tName = t.Name
  let ns = t.Namespace
  let id = getModuleNameAndId t

  let alreadyVisited, inProcess =
    visiting.TryGetValue(id)
  
  if id.Id = TsTypeId "System.Array" then
    let elType = t.GetElementType()
    let replacementType = typedefof<System.Collections.Generic.IEnumerable<_>>.MakeGenericType(elType)
    toTsType (depth + 1) replacementType
  elif alreadyVisited then
      if inProcess then
        // early abort
        Result.Error(Dependency.CicylcDependency { Id = id
                                                   IsGenericType = t.IsGenericType
                                                   IsGenericTypeDefinition = t.IsGenericTypeDefinition
                                                   GenericTypeArguments = []
                                                   DuCases = []
                                                   Type = t
                                                   Dependencies = []
                                                   HasCyclicDependency = true
                                                  })
      else
        if allTypes.ContainsKey(id) then
          Result.Ok allTypes.[id]
        else
          failwith("fail")
          Result.Error(Dependency.AlreadyVisited allTypes.[id])
  else
    visiting[id] <- true

    let toDuCase (v: UnionCaseInfo) =
      { Name = v.Name
        Tag = v.Tag
        Fields =
          v.GetFields()
          |> Seq.map (fun v ->
            let result = (toTsType (depth + 1) v.PropertyType)
            match result with
            | Ok r -> 
              { Name = v.Name
                TsType = r }
            | Error dependency ->
              match dependency with
              | AlreadyVisited tsType ->
                { Name = v.Name
                  TsType = tsType
                }
              | CicylcDependency tsType->
                // 
              { Name = v.Name
                TsType = tsType }
                
            )
          |> Seq.toList }

    if allTypes.ContainsKey(id) then
      Result.Ok allTypes.[id]
    elif depth > 10 then
      Result.Ok (TsType.Any(t))
    else

      let chooseOks v =
        match v with
          | Result.Ok v -> Some v
          | Result.Error _ -> None
      let unwrapType v =
        match v with
          | Result.Ok v -> v
          | Result.Error e ->
            match e with
            | Dependency.AlreadyVisited v -> v
            | Dependency.CicylcDependency v -> v
        
      let getPropertyTypes =
        if t |> isCollection then
          []
        else
          let properties = t.GetProperties()
          let p1 =
            properties
            |> Seq.toList
            |> List.choose (fun v ->
              if v.PropertyType.Equals(t) then
                None
              else
                let result =
                  toTsType (depth + 1) v.PropertyType

                Some result)
            |> List.distinct
            // |> List.choose chooseOks
          p1

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
        |> Seq.choose chooseOks
        |> Seq.toList

      let toDependencies (t: TsType) =
        // if isGeneric, get generic definition as dependency + extract arguments
        // else, only this type as dependency
        let x =
          if t.IsGenericType && not t.IsGenericTypeDefinition then
            let genericDefinition =
              t.Type.GetGenericTypeDefinition()
              |> toTsType (depth + 1)
            
            let genericArgs = (getArgumentsRecursively t.Type) |> List.map(toTsType(depth + 1)) 

            genericDefinition :: genericArgs
          else
            [  ]
        t :: (x
              |> List.choose (fun v ->
              match v with
              | Result.Ok v -> Some v
              | Result.Error x -> match x with
                                  | Dependency.AlreadyVisited v -> Some v
                                  | Dependency.CicylcDependency v -> Some v
        ))

      let duDeps =
        getDuCaseTypes
        |> Seq.collect (fun v -> v.Fields)
        |> Seq.map (fun v -> v.TsType)
        |> Seq.toList

      let genericRuntimeArgumentDeps =
        tsArgs |> List.filter(fun v-> not v.Id.TsSignature.IsGenericParameter)

      let deps =
        (duDeps @ (getPropertyTypes |> List.map unwrapType) @ genericRuntimeArgumentDeps)|>Seq.toList

      let deps = deps |> List.collect toDependencies

      let isPredefined = DefaultTypeDefinitionsAndValues.tryGetExistingTypeDefinition t
      let result =
        match isPredefined with
        | Some _ ->
          { Id = id
            IsGenericType = t.IsGenericType
            IsGenericTypeDefinition = t.IsGenericTypeDefinition
            GenericTypeArguments = []
            DuCases = []
            Type = t
            Dependencies = []
            HasCyclicDependency = false
          }
        | None -> 
          { Id = id
            IsGenericType = t.IsGenericType
            IsGenericTypeDefinition = t.IsGenericTypeDefinition
            GenericTypeArguments = tsArgs
            DuCases = getDuCaseTypes
            Type = t
            Dependencies = deps
            HasCyclicDependency = getPropertyTypes |> List.exists(fun v -> match v with | Result.Error _ -> true | _ -> false)
          }

      allTypes.TryAdd(id, result) |> ignore
      visiting[id] <- false
      Result.Ok result

let toTsType1 (depth: int) (t: Type) =
  
  let result = toTsType depth t
  
  match result with
  | Ok v -> v
  | Error _ ->
    let id = getModuleNameAndId t
    if allTypes.ContainsKey(id)
    then
      allTypes.[id]
    else
      TsType.Any(t)

let isNonGenericTypeOrGenericTypeDefinition (t: TsType) =
  let t = t.Type

  let isNotGenericOrGenericTypeDefinition =
    (not t.IsGenericType)
    || (t.IsGenericType && t.IsGenericTypeDefinition)

  isNotGenericOrGenericTypeDefinition
