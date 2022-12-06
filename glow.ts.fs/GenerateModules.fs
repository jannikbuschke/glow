module Glow.TsGen.Gen

open System
open System.Text
open System.Text.Json.Serialization
open Domain
open Glow.Ts
open Microsoft.FSharp.Core

let getDependencies (n: Namespace) =
  n.Items
  |> List.filter Glow.GetTsSignature.isNonGenericTypeOrGenericTypeDefinition
  |> List.collect (fun v -> v.Dependencies)
  |> List.map (fun v -> v.Id)
  |> List.distinctBy (fun v -> v.OriginalNamespace)
  |> List.filter (fun v -> v.OriginalNamespace <> n.Name)

let generateModules (types: Type list) : Namespace list =
  let directTypes =
    types |> List.map (Glow.GetTsSignature.toTsType 0)

  let rec collectDependencies (depth: int) (v: TsType) =
    if depth > 0 then
      v.Dependencies
    else
      (v.Dependencies
       @ (v.Dependencies
          |> List.collect (collectDependencies (depth + 1))))

  let dependencies =
    directTypes
    |> List.collect (collectDependencies 0)
    |> List.rev

  let genericDefinitions =
    (directTypes @ dependencies)
    |> List.filter (fun v -> v.IsGenericType && not v.IsGenericTypeDefinition)
    |> List.map (fun v -> Glow.GetTsSignature.toTsType 0 (v.Type.GetGenericTypeDefinition()))

  let modules =
    (directTypes @ dependencies @ genericDefinitions)
    |> List.distinct
    |> List.groupBy (fun v -> v.Id.TsSignature.TsNamespace)
    |> List.map (fun (namespaceName, types) ->
      { Name = namespaceName
        Items =
          types
          |> Seq.distinctBy (fun v -> v.Id.TsSignature)
          |> Seq.toList })

  modules

let renderPropertyDefinitions (typeToBeRendered: TsType) : string =
  let props =
    typeToBeRendered.Type.GetProperties()

  let nameSpace =
    typeToBeRendered.Id.OriginalNamespace

  let result =
    props
    |> Seq.toList
    |> List.map (fun v ->
      let propertyTsType =
        Glow.GetTsSignature.toTsType 0 v.PropertyType

      let propertySignature =
        propertyTsType.Id.TsSignature

      let propName =
        if propertyTsType.Id.OriginalNamespace = nameSpace then
          propertySignature.Name()
        else
          propertySignature.FullSanitizedName()

      let rec getGenericParameters (t: TsSignature) =
        if t.IsGenericType then
          "<"
          + (t.GenericArgumentTypes
             |> Seq.map (fun v ->
               let name =
                 if nameSpace = v.TsNamespace then
                   v.Name()
                 else
                   v.FullSanitizedName()

               $"{name}{getGenericParameters (v)}")
             |> String.concat ",")
          + ">"
        else
          ""

      $"{Utils.camelize v.Name}: {propName}{getGenericParameters (propertyTsType.Id.TsSignature)}")
    |> String.concat "\n  "

  result

let renderPropertyValues (t: TsType) : string =
  let props = t.Type.GetProperties()

  let nameSpace = t.Id.OriginalNamespace

  let result =
    props
    |> Seq.toList
    |> List.map (fun v ->
      let propertyTsType =
        Glow.GetTsSignature.toTsType 0 v.PropertyType

      let propertySignature =
        propertyTsType.Id.TsSignature

      let defaultPrefix =
        if propertyTsType.Id.TsSignature.IsGenericParameter then
          ""
        else
          "default"

      let propName =
        if propertyTsType.Id.OriginalNamespace = nameSpace then
          $"{defaultPrefix}{propertySignature.Name()}"
        else
          $"{(NamespaceName.sanitize propertySignature.TsNamespace)}.{defaultPrefix}{propertySignature.Name()}"

      let rec getGenericArguments (t: TsSignature) =
        if t.IsGenericType then
          "("
          + (t.GenericArgumentTypes
             |> Seq.map (fun v ->
               let defaultPrefix =
                 if v.IsGenericParameter then
                   ""
                 else
                   "default"

               if nameSpace = v.TsNamespace then
                 $"{defaultPrefix}{v.Name()}{getGenericArguments (v)}"
               else
                 $"{(NamespaceName.sanitize v.TsNamespace)}.{defaultPrefix}{v.Name()}{getGenericArguments (v)}")
             |> String.concat ",")
          + ")"
        else
          ""

      $"{Utils.camelize v.Name}: {propName}{getGenericArguments (propertyTsType.Id.TsSignature)},")
    |> String.concat "\n  "

  result

type RenderedDuCaseDefinitionAndValue =
  { Name: string
    GenericName: string option
    CaseName: string
    Definition: string
    DefaultValue: string }

let getCases (t: TsType) : RenderedDuCaseDefinitionAndValue list =
  let cases = t.DuCases
  let duName = t.Id.TsSignature.GetName()

  match cases with
  | [ case ] ->
    if case.Fields.Length <> 1 then
      failwith "Single case discriminated union with field length <> is not yet supported"

    let fieldSignature = case.Fields.Head

    let ns =
      if fieldSignature.TsType.Id.OriginalNamespace = t.Id.OriginalNamespace then
        ""
      else
        fieldSignature.TsType.Id.TsSignature.TsNamespace
        |> NamespaceName.sanitize

    let propName =
      $"{ns}.{fieldSignature.TsType.Id.TsSignature.Name()}"

    let defaultValue =
      $"{ns}.default{fieldSignature.TsType.Id.TsSignature.Name()}"

    let typeName = $"{case.Name}"

    [ { Name = typeName
        GenericName = Some(fieldSignature.TsType.Id.TsSignature.NameWithGenericArguments())
        CaseName = case.Name
        Definition = propName
        DefaultValue = defaultValue } ]
  | cases ->
    cases
    |> List.map (fun v ->

      let typeName = $"{duName}_Case_{v.Name}"

      let getFieldDefinition (field: DuCaseField) =
        let ns =
          if field.TsType.Id.OriginalNamespace = t.Id.OriginalNamespace then
            ""
          else
            (field.TsType.Id.OriginalNamespace
             |> NamespaceName.sanitize)
            + "."

        let fieldTypeName =
          $"{ns}{field.TsType.Id.TsSignature.Name()}"

        let defaultFieldValue =
          $"{ns}default{field.TsType.Id.TsSignature.Name()}"

        if field.TsType.Id.TsSignature.IsGenericParameter then
          let genericParameterName =
            field.TsType.Id.TsSignature.GetName()

          let genericParameters =
            $"<{genericParameterName}>"

          let nameWithGenericParameter =
            $"{typeName}{genericParameters}"

          let genericArguments =
            $"(default{genericParameterName}:{genericParameterName})"

          {| FieldName = field.Name
             FieldTypeName = fieldTypeName
             DefaultFieldValue = defaultFieldValue
             Generics =
              Some
                {| NameWithGenericParameter = nameWithGenericParameter
                   GenericParamterName = genericParameterName
                   GenericParameters = genericParameters
                   GenericArguments = genericArguments |} |}

        else
          {| FieldName = field.Name
             FieldTypeName = fieldTypeName
             DefaultFieldValue = defaultFieldValue
             Generics = None |}

      match v.Fields with
      | [] ->
        { Name = typeName
          GenericName = None
          CaseName = v.Name
          Definition = @$"{{ Case: ""{v.Name}"" }}"
          DefaultValue = @$"{{ Case: ""{v.Name}"" }}" }
      | [ field ] ->
        let fieldDefinition =
          getFieldDefinition field

        match fieldDefinition.Generics with
        | Some x ->
          { Name = typeName
            GenericName = Some x.NameWithGenericParameter
            CaseName = v.Name
            Definition = @$"{{ Case: ""{v.Name}"", Fields: {fieldDefinition.FieldTypeName} }}"
            DefaultValue = @$"{x.GenericParameters}{x.GenericArguments} => ({{ Case: ""{v.Name}"", Fields: {fieldDefinition.DefaultFieldValue} }})" }
        | None ->
          { Name = typeName
            GenericName = None
            CaseName = v.Name
            Definition = @$"{{ Case: ""{v.Name}"", Fields: {fieldDefinition.FieldTypeName} }}"
            DefaultValue = @$"{{ Case: ""{v.Name}"", Fields: {fieldDefinition.DefaultFieldValue} }}" }

      | fields ->
        let fieldDefinitionsAndValues =
          fields |> List.map getFieldDefinition

        let definitions =
          (fieldDefinitionsAndValues
           |> List.map (fun v -> $"{v.FieldName}: {v.FieldTypeName}")
           |> String.concat ", ")

        let combinedFieldDefinition =
          $"{{ {definitions} }}"

        let values =
          (fieldDefinitionsAndValues
           |> List.map (fun v -> $"{v.FieldName}: {v.DefaultFieldValue}")
           |> String.concat ", ")

        let combinedFieldValue = $"{{ {values} }}"

        { Name = typeName
          GenericName = None
          CaseName = v.Name
          Definition = @$"{{ Case: ""{v.Name}"", Fields: {combinedFieldDefinition} }}"
          DefaultValue = @$"{{ Case: ""{v.Name}"", Fields: {combinedFieldValue} }}" })

// let rec renderGenericDefaultValueFunction (t: TsType) =
//   let firstCase = t.DuCases.Head
//
//   let name =
//     t.Id.TsSignature.NameWithGenericArguments()
//
//   let genericArguments =
//     t.Id.TsSignature.GenericArgumentNames()
//     |> String.concat ","
//
//   let parameters =
//     if t.Id.TsSignature.GenericArgumentTypes.Length = 0 then
//       ""
//     else
//       t.Id.TsSignature.GenericArgumentTypes
//       |> List.map (fun b -> $"{b.PropertyTypeName() |> toLower}:{b.PropertyTypeName()}")
//       |> List.reduce (fun a b -> $"{a},{b}")
//
//   let typeName = $"{name}"
//   $"<{genericArguments}>({parameters}) => null as any"

let renderDuFirstValueAsDefault (t: TsType) =
  let firstHead = t.DuCases |> List.tryHead

  match firstHead with
  | Some head ->
    match head.Fields with
    | [] -> $"null as any // (zero fields on DU case '{t.Id.OriginalName} / {head.Name}' not yet supported)"
    | [ oneField ] ->
      let signature =
        oneField.TsType.Id.TsSignature

      let ns =
        if t.Id.TsSignature.TsNamespace = signature.TsNamespace then
          ""
        else
          (signature.TsNamespace |> NamespaceName.sanitize)
          + "."

      $"{ns}default{oneField.TsType.Id.TsSignature.GetName()}"
    | manyFields -> $"null as any // (many fields on DU case '{t.Id.OriginalName} / {head.Name}' not yet supported)"

  | None -> $"null as any // (zero cases on DU '{t.Id.OriginalName}' not supported)"

let renderDefaultRecordOrClassValue (t: TsType) =
  $"""{{
  {renderPropertyValues t}
}}"""

let renderRecordOrClassDefinition (t: TsType) : string =
  $"""{{
  {renderPropertyDefinitions t}
}}"""

type Renderable =
  | TypeAndValue of name: string * definition: string * defaultValue: string
  | GenericTypeAndFunction of name: string * genericName: string * definition: string * defaultGeneratorSignature: string * defaultGeneratorImpl: string
  | Enum of name: string * values: string list
  | DiscriminatedUnion of name: string * genericName: string option * cases: RenderedDuCaseDefinitionAndValue list
  | NotRenderable

module DefaultTypeDefinitionsAndValues =
  type Dict<'a, 'b> = System.Collections.Concurrent.ConcurrentDictionary<'a, 'b>

  let tryGetExistingTypeDefinition =
    let types =
      Dict<System.Type, string * string option * string * string option * string>()

    let add v = types.TryAdd v |> ignore

    add (typeof<System.String>, ("String", None, "string", None, "\"\""))
    add (typedefof<_ list>, ("FSharpList", Some "FSharpList<T>", "Array<T>", Some "<T>(t:T)", "[]"))
    add (typedefof<_ option>, ("FSharpOption", Some "FSharpOption<T>", "T | null", Some "<T>(t:T)", "null"))
    add (typedefof<Nullable<_>>, ("Nullable", Some "Nullable<T>", "T | null", Some "<T>(t:T)", "null"))
    add (typedefof<System.Collections.Generic.IEnumerable<_>>, ("IEnumerable", Some "IEnumerable<T>", "Array<T>", Some "<T>(t:T)", "[]"))
    add (typedefof<System.Collections.Generic.IList<_>>, ("IList", Some "IList<T>", "Array<T>", Some "<T>(t:T)", "[]"))
    add (typedefof<System.Collections.Generic.ICollection<_>>, ("ICollection", Some "ICollection<T>", "Array<T>", Some "<T>(t:T)", "[]"))
    add (typedefof<System.Collections.Generic.IDictionary<_,_>>, ("IDictionary", Some "IDictionary<TKey, TValue>", "{ [key: string | number]: TValue }", Some "<TKey, TValue>(t:TKey,tValue:TValue)", "({})"))

    add (
      typedefof<System.Collections.Generic.Dictionary<_, _>>,
      ("Dictionary", Some "Dictionary<TKey, TValue>", "{ [key: string | number]: TValue }", Some "<TKey, TValue>(t:TKey,tValue:TValue)", "({})")
    )

    add (typedefof<System.Collections.Generic.IReadOnlyList<_>>, ("IReadOnlyList", Some "IReadOnlyList<T>", "Array<T>", Some "<T>(t:T)", "([])"))
    add (typedefof<System.Collections.Generic.List<_>>, ("List", Some "List<T>", "Array<T>", Some "<T>(t:T)", "([])"))

    add (
      typedefof<System.Collections.Generic.KeyValuePair<_, _>>,
      ("KeyValuePair", Some "KeyValuePair<TKey,TValue>", "{Key:TKey,Value:TValue}", Some "<TKey,TValue>(tKey:TKey,tValue:TValue)", "({Key:tKey,Value:tValue})")
    )

    add (typeof<System.DateTimeOffset>, ("DateTimeOffset", None, "string", None, "\"0000-00-00T00:00:00+00:00\""))
    add (typeof<System.String>, ("String", None, "string", None, "\"\""))
    add (typeof<System.TimeSpan>, ("TimeSpan", None, "string", None, "\"00:00:00\""))
    add (typeof<System.DateTime>, ("DateTime", None, "string", None, "\"0001-01-01T00:00:00\""))
    add (typeof<System.DateTimeOffset>, ("DateTimeOffset", None, "string", None, ""))
    add (typeof<System.Char>, ("Char", None, "string", None, @"''"))
    add (typeof<System.Guid>, ("Guid", None, "string", None, @"""00000000-0000-0000-000000000000"""))
    add (typeof<System.Int32>, ("Int32", None, "number", None, "0"))
    add (typeof<System.Int64>, ("Int64", None, "number", None, "0"))
    add (typeof<System.Double>, ("Double", None, "number", None, "0"))
    add (typeof<System.Boolean>, ("Boolean", None, "boolean", None, "false"))
    add (typeof<System.Object>, ("Object", None, "any", None, "{}"))

    fun (t: System.Type) ->
      if types.ContainsKey t then
        let name, genericName, definition, defaultValueSignature, defaultValue =
          types.[t]

        match genericName with
        | Some genericName ->
          Some(
            Renderable.GenericTypeAndFunction(
              name = name,
              genericName = genericName,
              definition = definition,
              defaultGeneratorSignature = defaultValueSignature.Value,
              defaultGeneratorImpl = defaultValue
            )
          )
        | None -> Some(Renderable.TypeAndValue(name = name, definition = definition, defaultValue = defaultValue))
      else
        None

let getDuDefinitionsAndValues (t: TsType) =
  if
    (TypeCache.getKind t.Type)
    <> System.Text.Json.Serialization.TypeCache.TypeKind.Union
  then
    raise (ArgumentException("Type is not a Union"))

  let cases = t |> getCases

  Renderable.DiscriminatedUnion(
    name = t.Id.TsSignature.GetName(),
    genericName =
      (if t.IsGenericType then
         Some(t.Id.TsSignature.NameWithGenericArguments())
       else
         None),
    cases = cases
  )

let renderKnownTypeAndDefaultValue (t: TsType) (serialize: Serialize) : string option =

  let existingType =
    DefaultTypeDefinitionsAndValues.tryGetExistingTypeDefinition t.Type

  let kind = TypeCache.getKind t.Type

  let renderable =
    match existingType with
    | Some r -> r
    | None ->
      match kind with
      | System.Text.Json.Serialization.TypeCache.TypeKind.Enum ->
        Renderable.Enum(
          name = t.Id.TsSignature.GetName(),
          values = (Enum.GetNames(t.Type) |> Seq.toList)
        )
      | System.Text.Json.Serialization.TypeCache.TypeKind.List ->
        Renderable.GenericTypeAndFunction(
          name = t.Id.TsSignature.GetName(),
          genericName = t.Id.TsSignature.NameWithGenericArguments(),
          definition = "Array<T>",
          defaultGeneratorSignature = "<T>(t:T)",
          defaultGeneratorImpl = "[] "
        )
      | System.Text.Json.Serialization.TypeCache.TypeKind.Map -> raise (NotSupportedException())
      | System.Text.Json.Serialization.TypeCache.TypeKind.Set -> raise (NotSupportedException())
      | System.Text.Json.Serialization.TypeCache.TypeKind.Tuple -> raise (NotSupportedException())
      | System.Text.Json.Serialization.TypeCache.TypeKind.Union -> getDuDefinitionsAndValues t
      | System.Text.Json.Serialization.TypeCache.TypeKind.Record
      | System.Text.Json.Serialization.TypeCache.TypeKind.Other ->
        if t.Id.TsSignature.IsGenericParameter then
          Renderable.NotRenderable
        else if t.IsGenericType && not t.IsGenericTypeDefinition then
          Renderable.NotRenderable
        else if t.IsGenericType && t.IsGenericTypeDefinition then
          let definition =
            renderRecordOrClassDefinition t

          let value =
            renderDefaultRecordOrClassValue t

          Renderable.GenericTypeAndFunction(
            name = t.Id.TsSignature.GetName(),
            genericName = t.Id.TsSignature.NameWithGenericArguments(),
            definition = definition,
            defaultGeneratorSignature = t.Id.TsSignature.GeneratorFunctionSignature(), // "<a>(defaulta:a)",
            defaultGeneratorImpl = $"({value})"
          )
        else
          let definition =
            renderRecordOrClassDefinition t

          let value =
            renderDefaultRecordOrClassValue t

          Renderable.TypeAndValue(name = t.Id.TsSignature.GetName(), definition = definition, defaultValue = value)

  match renderable with
  | NotRenderable -> None
  | Enum(name, values) ->
    let definition = values |> List.map(fun v-> $"\"{v}\"") |> String.concat " | "
    let allValues = values |> List.map(fun v-> $"\"{v}\"") |> String.concat ", "
    Some(
        $"export type {name} = {definition}"
        + "\n"
        + $"export const {name}_AllValues = [{allValues}] as const"
        + "\n"
        + $"export const default{name}: {name} = \"{values.Head}\""
      )
  | DiscriminatedUnion (name, genericName, cases) ->
    match cases with
    | [ singleCase ] ->
      Some(
        $"export type {name} = {singleCase.Definition}"
        + "\n"
        + $"export const default{name}: {name} = {singleCase.DefaultValue}"
      )
    | cases ->
      let renderedCases =
        cases
        |> List.map (fun v ->
          $"export type {(match v.GenericName with
                          | Some genericName -> genericName
                          | None -> v.Name)} = {v.Definition}")
        |> String.concat "\n"

      let casesLiteral =
        cases
        |> List.map (fun v -> $"\"{v.CaseName}\"")
        |> String.concat " | "

      let casesArray =
        "[ "
        + (cases
           |> List.map (fun v -> $"\"{v.CaseName}\"")
           |> String.concat ", ")
        + " ]"

      let discriminatedUnionDefinition =
        cases
        |> List.map (fun v ->
          match v.GenericName with
          | Some genericName -> genericName
          | None -> v.Name)
        |> String.concat " | "

      let defaults =
        cases
        |> List.map (fun v -> $"export const default{v.Name} = {v.DefaultValue}")
        |> String.concat "\n"

      Some(
        renderedCases
        + "\n"
        + if cases.Length <> 1 then
            match genericName with
            | Some genericName -> $"export type {genericName} = {discriminatedUnionDefinition}"
            | None -> $"export type {name} = {discriminatedUnionDefinition}"
          else
            ""
        + "\n"
        + $"export type {name}_Case = {casesLiteral}"
        + "\n"
        + $"export const {name}_AllCases = {casesArray} as const"
        + "\n"
        + defaults
        + "\n"
        + match genericName with
          | Some genericName ->
            $"export const default{name} = {t.Id.TsSignature.GenericParameters()}{t.Id.TsSignature.GenericArguments()} => null as any as {genericName}"
          | None -> $"export const default{name} = null as any as {name}"
      )

  | TypeAndValue (name, definition, defaultValue) ->
    Some(
      $"export type {name} = {definition}\n"
      + $"export const default{name}: {name} = {defaultValue}"
    )
  | GenericTypeAndFunction (name, genericName, definition, defaultGeneratorSignature, defaultGeneratorimpl) ->
    Some(
      $"export type {genericName} = {definition}\n"
      + $"export const default{name}: {defaultGeneratorSignature} => {genericName} = {defaultGeneratorSignature} => {defaultGeneratorimpl}"
    )

let renderTypeDefinitionAndValue t : string option =
  renderKnownTypeAndDefaultValue t DefaultSerialize.serialize

let renderModule (m: Namespace) : string =

  let name = m.Name |> NamespaceName.value
  let deps = getDependencies m

  let builder = StringBuilder()

  builder
    .AppendLine("///////////////////////////////////////////////////////////")
    .AppendLine("//                          This file is auto generated //")
    .AppendLine("//////////////////////////////////////////////////////////")
    .AppendLine("")
  |> ignore

  deps
  |> List.iter (fun v ->
    let name =
      v.OriginalNamespace |> NamespaceName.sanitize

    if v.Id = TsTypeId "Any" || name = null then
      ()
    elif name = "" || name = null then
      builder.AppendLine($"// skipped importing empty namespace (type={v.OriginalName})")
      |> ignore
    else
      let x = $@"import * as {name} from ""./{name}"""
      if x = $"import * as  from \"./\"" then
        ()
      else
        builder.AppendLine(x)
      |> ignore

    ())

  let sorted, cyclics =
    m.Items
    |> List.distinctBy (fun v -> v.Id.TsSignature)
    |> List.filter Glow.GetTsSignature.isNonGenericTypeOrGenericTypeDefinition
    |> Glow.TopologicalSort.topologicalSort (fun v ->
      v.Dependencies
      |> List.filter (fun dependency ->
        dependency <> v
        && dependency.Id.TsSignature.TsNamespace = v.Id.TsSignature.TsNamespace))

  if cyclics.Length > 0 then
    builder
      .AppendLine("//*** Cyclic dependencies dected ***")
      .AppendLine("//*** this can cause problems when generating types and defualt values ***")
      .AppendLine("//*** Please ensure that your types don't have cyclic dependencies ***")
    |> ignore

    cyclics
    |> List.iter (fun v ->
      builder.AppendLine("//" + v.Id.OriginalName)
      |> ignore)

    builder.AppendLine("//*** ******************* ***").AppendLine("")
    |> ignore

  sorted
  |> List.distinctBy (fun v -> v.Id.TsSignature)
  |> List.map (fun v ->
    let x = renderTypeDefinitionAndValue v

    match x with
    | Some x -> x
    | None -> $"// skipped {v.Id.TsSignature.TsName |> TsName.value}")
  |> List.map Utils.cleanTs
  |> List.iter (fun v -> builder.AppendLine(v) |> ignore)

  builder.ToString().Replace("\r\n", "\n")

let findeTsTypeInModules modules t =
  let id =
    (Glow.GetTsSignature.getModuleNameAndId t).Id

  modules
  |> List.collect (fun v -> v.Items)
  |> List.find (fun v -> v.Id.Id = id)
