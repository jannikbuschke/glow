module Glow.TsGen.Gen

open System
open System.Text
open System.Text.Json.Serialization
open Domain
open Glow.GetTsSignature
open Glow.Ts
open Microsoft.FSharp.Core

let getDependencies (n: Namespace) =
  let deps =
    n.Items
    |> List.filter Glow.GetTsSignature.isNonGenericTypeOrGenericTypeDefinition
    |> List.collect (fun v -> v.Dependencies)
    |> List.map (fun v -> v.Id)
    |> List.distinctBy (fun v -> v.OriginalNamespace)
    |> List.filter (fun v -> v.TsSignature.TsNamespace <> n.Name)

  deps

let getModuleDependencies (n: Namespace) =
  n.Items
  |> List.filter Glow.GetTsSignature.isNonGenericTypeOrGenericTypeDefinition
  |> List.collect (fun v -> v.Dependencies)
  |> List.map (fun v -> v.Id)
  |> List.distinctBy (fun v -> v.OriginalNamespace)
  |> List.filter (fun v -> v.TsSignature.TsNamespace <> n.Name)
  |> List.map (fun v -> v.TsSignature.TsNamespace)
  |> List.distinct

let rec collectDependencies (depth: int) (v: TsType) =
  let name = v.Id.OriginalName

  if depth > 2 then
    v.Dependencies
  else
    (v.Dependencies
     @ (v.Dependencies |> List.collect (collectDependencies (depth + 1))))

let getDependenciesFor items =
  items |> List.collect (collectDependencies 0) |> List.rev

let getGenericDefinitions items =
  items
  |> List.filter (fun v -> v.IsGenericType && not v.IsGenericTypeDefinition)
  |> List.map (fun v -> Glow.GetTsSignature.toTsType1 0 (v.Type.GetGenericTypeDefinition()))

let getDirectTypes (items: Type list) =
  items |> List.map (Glow.GetTsSignature.toTsType1 0)

let getAllTypes items =
  let directTypes = getDirectTypes items
  let dependencies = getDependenciesFor directTypes
  let genericDefinitions = getGenericDefinitions (directTypes @ dependencies)
  directTypes @ dependencies @ genericDefinitions

let groupToModules items =
  let reg = items |> List.filter (fun v -> v.Id.OriginalName = "Registration")

  items
  |> List.distinct
  |> List.groupBy (fun v -> v.Id.TsSignature.TsNamespace)
  |> List.map (fun (namespaceName, types) ->
    let name = namespaceName |> NamespaceName.value

    let result =
      { Name = namespaceName
        Items =
          types
          // |> Seq.filter(fun v -> not v.HasCyclicDependency)
          |> Seq.distinctBy (fun v -> v.Id.TsSignature, v.HasCyclicDependency, v.Dependencies.Length)
          |> Seq.toList }

    result)

let generateModules (types: Type list) : Namespace list =
  let allTsTypes = getAllTypes (typedefof<System.Object> :: types)

  groupToModules (TsType.Any(typeof<System.Object>) :: allTsTypes)

let renderPropertyDefinitions (typeToBeRendered: TsType) : string =
  let props = typeToBeRendered.Type.GetProperties()

  let nameSpace = typeToBeRendered.Id.TsSignature.TsNamespace

  let result =
    props
    |> Seq.toList
    |> List.map (fun v ->
      let propertyTsType = Glow.GetTsSignature.toTsType1 0 v.PropertyType

      let propertySignature = propertyTsType.Id.TsSignature

      let propName =
        if propertyTsType.Id.TsSignature.TsNamespace = nameSpace then
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

               $"{name}{getGenericParameters v}")
             |> String.concat ",")
          + ">"
        else
          ""

      $"{Utils.camelize v.Name}: {propName}{getGenericParameters propertyTsType.Id.TsSignature}")
    |> String.concat "\n  "

  result


let renderValueStub (t: TsType) : string =
  let props = t.Type.GetProperties()
  let nameSpace = t.Id.TsSignature.TsNamespace

  let result =
    props
    |> Seq.toList
    |> List.map (fun v ->
      let result = $"{Utils.camelize v.Name}: undefined as any,"
      result)
    |> String.concat "\n  "

  result

let getDefaultValue (nameSpace: NamespaceName) (v: Reflection.PropertyInfo) =
  let wellKnownType =
    if v.PropertyType.IsGenericType && not v.PropertyType.IsGenericTypeDefinition then
      DefaultTypeDefinitionsAndValues.tryGetExistingTypeDefinition (v.PropertyType.GetGenericTypeDefinition())
    else
      DefaultTypeDefinitionsAndValues.tryGetExistingTypeDefinition v.PropertyType

  let wellknownInlineValue =
    match wellKnownType with
    | Some wellKnownType ->
      match wellKnownType with
      | TypeAndValue (_, _, _, inlineValue) ->
        match inlineValue with
        | Some value -> Some value
        | None -> None
      | GenericTypeAndFunction (_, _, _, _, _, inlineValue) ->
        match inlineValue with
        | Some value -> Some value
        | None -> None
      | _ -> None
    | None -> None

  match wellknownInlineValue with
  | Some value -> value
  | None ->
    let propertyTsType = Glow.GetTsSignature.toTsType1 0 v.PropertyType

    let propertySignature = propertyTsType.Id.TsSignature

    let defaultPrefix =
      if propertyTsType.Id.TsSignature.IsGenericParameter then
        ""
      else
        "default"

    let propName =
      if propertyTsType.Id.TsSignature.IsGenericParameter then
        $"{defaultPrefix}{propertySignature.Name().ToLower()}"
      elif propertyTsType.Id.TsSignature.TsNamespace = nameSpace then
        $"{defaultPrefix}{propertySignature.Name()}"
      else
        $"{(NamespaceName.sanitize propertySignature.TsNamespace)}.{defaultPrefix}{propertySignature.Name()}"

    let rec getGenericArguments (t: TsSignature) =
      if t.IsGenericType then
        "("
        + (t.GenericArgumentTypes
           |> Seq.map (fun v ->

             let defaultPrefix = if v.IsGenericParameter then "" else "default"

             let name =
               if v.IsGenericParameter then
                 v.Name().ToLower()
               else
                 v.Name()

             if nameSpace = v.TsNamespace then
               $"{defaultPrefix}{name}{getGenericArguments v}"
             else

               $"{(NamespaceName.sanitize v.TsNamespace)}.{defaultPrefix}{name}{getGenericArguments v}")
           |> String.concat ",")
        + ")"
      else
        ""

    $"{propName}{getGenericArguments propertyTsType.Id.TsSignature}"

let renderValueFix (t: TsType) : string =
  let props = t.Type.GetProperties()
  let nameSpace = t.Id.TsSignature.TsNamespace

  let result =
    props
    |> Seq.toList
    |> List.map (fun v ->
      let value = getDefaultValue nameSpace v

      let result =
        $"default{t.Id.TsSignature.GetName()}.{Utils.camelize v.Name} = {value}"

      result)
    |> String.concat "\n"

  result

let renderPropertyValues (t: TsType) : string =
  let props = t.Type.GetProperties()

  let nameSpace = t.Id.TsSignature.TsNamespace

  let result =
    props
    |> Seq.toList
    |> List.map (fun v ->
      let wellKnownType =
        DefaultTypeDefinitionsAndValues.tryGetExistingTypeDefinition v.PropertyType

      let r = if wellKnownType.IsNone then "None" else "Some"

      let renderDefaut () =
        let value = getDefaultValue nameSpace v
        let result = $"{Utils.camelize v.Name}: {value}, // wellknown type {(r)}"
        result

      match wellKnownType with
      | Some x ->
        match x with
        | GenericTypeAndFunction (name,
                                  genericName,
                                  definition,
                                  defaultGeneratorSignature,
                                  defaultGeneratorImpl,
                                  inlineValue) ->
          match inlineValue with
          | Some inlineValue ->
            let value = getDefaultValue nameSpace v
            let result = $"{Utils.camelize v.Name}: {inlineValue},"
            result
          | None -> renderDefaut ()
        | TypeAndValue (name, definition, defaultValue, inlineValue) ->
          match inlineValue with
          | Some inlineValue ->
            let value = getDefaultValue nameSpace v
            let result = $"{Utils.camelize v.Name}: {inlineValue},"
            result
          | None -> renderDefaut ()
        | _ -> renderDefaut ()
      | None -> renderDefaut ())
    |> String.concat "\n  "

  result

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
        fieldSignature.TsType.Id.TsSignature.TsNamespace |> NamespaceName.sanitize

    let propName = $"{ns}.{fieldSignature.TsType.Id.TsSignature.Name()}"

    let defaultValue = $"{ns}.default{fieldSignature.TsType.Id.TsSignature.Name()}"

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
            (field.TsType.Id.TsSignature.TsNamespace |> NamespaceName.sanitize) + "."

        let fieldTypeName = $"{ns}{field.TsType.Id.TsSignature.Name()}"

        let defaultFieldValue = $"{ns}default{field.TsType.Id.TsSignature.Name()}"

        if field.TsType.Id.TsSignature.IsGenericParameter then
          let genericParameterName = field.TsType.Id.TsSignature.GetName()

          let genericParameters = $"<{genericParameterName}>"

          let nameWithGenericParameter = $"{typeName}{genericParameters}"

          let genericArguments = $"(default{genericParameterName}:{genericParameterName})"

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
        let fieldDefinition = getFieldDefinition field

        match fieldDefinition.Generics with
        | Some x ->
          { Name = typeName
            GenericName = Some x.NameWithGenericParameter
            CaseName = v.Name
            Definition = @$"{{ Case: ""{v.Name}"", Fields: {fieldDefinition.FieldTypeName} }}"
            DefaultValue =
              @$"{x.GenericParameters}{x.GenericArguments} => ({{ Case: ""{v.Name}"", Fields: {fieldDefinition.DefaultFieldValue} }})" }
        | None ->
          { Name = typeName
            GenericName = None
            CaseName = v.Name
            Definition = @$"{{ Case: ""{v.Name}"", Fields: {fieldDefinition.FieldTypeName} }}"
            DefaultValue = @$"{{ Case: ""{v.Name}"", Fields: {fieldDefinition.DefaultFieldValue} }}" }

      | fields ->
        let fieldDefinitionsAndValues = fields |> List.map getFieldDefinition

        let definitions =
          (fieldDefinitionsAndValues
           |> List.map (fun v -> $"{v.FieldName}: {v.FieldTypeName}")
           |> String.concat ", ")

        let combinedFieldDefinition = $"{{ {definitions} }}"

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
      let signature = oneField.TsType.Id.TsSignature

      let ns =
        if t.Id.TsSignature.TsNamespace = signature.TsNamespace then
          ""
        else
          (signature.TsNamespace |> NamespaceName.sanitize) + "."

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

type RenderCyclicDefault =
  | NoCycle
  | Stub
  | Fix

let renderKnownTypeAndDefaultValue (t: TsType) (cyclic: RenderCyclicDefault) (serialize: Serialize) : string option =

  let existingType =
    DefaultTypeDefinitionsAndValues.tryGetExistingTypeDefinition t.Type

  let kind = TypeCache.getKind t.Type

  let renderable =
    match existingType with
    | Some r ->
      match cyclic with
      | NoCycle -> r
      | _ -> Renderable.NotRenderable

    | None ->
      match kind with
      | System.Text.Json.Serialization.TypeCache.TypeKind.Enum ->
        Renderable.Enum(name = t.Id.TsSignature.GetName(), values = (Enum.GetNames(t.Type) |> Seq.toList))
      | System.Text.Json.Serialization.TypeCache.TypeKind.List ->
        Renderable.GenericTypeAndFunction(
          name = t.Id.TsSignature.GetName(),
          genericName = t.Id.TsSignature.NameWithGenericArguments(),
          definition = "Array<T>",
          defaultGeneratorSignature = "<T>(t:T)",
          defaultGeneratorImpl = "[]",
          inlineValue = Some "[]"
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
          let definition = renderRecordOrClassDefinition t

          let value = renderDefaultRecordOrClassValue t

          Renderable.GenericTypeAndFunction(
            name = t.Id.TsSignature.GetName(),
            genericName = t.Id.TsSignature.NameWithGenericArguments(),
            definition = definition,
            defaultGeneratorSignature = t.Id.TsSignature.GeneratorFunctionSignature(), // "<a>(defaulta:a)",
            defaultGeneratorImpl = $"({value})",
            inlineValue = None
          )
        else
          match cyclic with
          | Fix ->
            let definition = renderRecordOrClassDefinition t
            let value = "stub"
            let fix = renderValueFix t
            Renderable.CyclicFixValue(name = t.Id.TsSignature.GetName(), fixReferences = fix)
          | Stub ->
            let stub = renderValueStub t
            let definition = renderRecordOrClassDefinition t
            let value = renderDefaultRecordOrClassValue t

            Renderable.CyclicTypeAndStubValue(
              name = t.Id.TsSignature.GetName(),
              definition = definition,
              stubValue = stub
            )
          | NoCycle ->
            let definition = renderRecordOrClassDefinition t
            let value = renderDefaultRecordOrClassValue t

            Renderable.TypeAndValue(
              name = t.Id.TsSignature.GetName(),
              definition = definition,
              defaultValue = value,
              inlineValue = None
            )

  match renderable with
  | NotRenderable -> None
  | CyclicFixValue (name, fixValue) ->
    Some(
      $"// the type {name} has cyclic dependencies\n"
      + "// in general this should be avoided\n"
      + "// fill all props\n"
      + $"{fixValue}"
    )
  | CyclicTypeAndStubValue (name, definition, stubValue) ->
    Some(
      $"// the type {name} has cyclic dependencies\n"
      + "// in general this should be avoided\n"
      + "// Render an empty object to be filled later at end of file\n"
      + "// to prevent typescript errors (reference used before declaration)\n"
      + $"export type {name} = {definition} \n\n"
      + $"export var default{name}: {name} = {{
  {stubValue} }}
"
    )
  | Enum (name, values) ->
    let definition = values |> List.map (fun v -> $"\"{v}\"") |> String.concat " | "
    let allValues = values |> List.map (fun v -> $"\"{v}\"") |> String.concat ", "

    Some(
      $"export type {name} = {definition}"
      + "\n"
      + $"export var {name}_AllValues = [{allValues}] as const"
      + "\n"
      + $"export var default{name}: {name} = \"{values.Head}\""
    )
  | DiscriminatedUnion (name, genericName, cases) ->
    match cases with
    | [ singleCase ] ->
      Some(
        $"export type {name} = {singleCase.Definition}"
        + "\n"
        + $"export var default{name}: {name} = {singleCase.DefaultValue}"
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
        cases |> List.map (fun v -> $"\"{v.CaseName}\"") |> String.concat " | "

      let casesArray =
        "[ "
        + (cases |> List.map (fun v -> $"\"{v.CaseName}\"") |> String.concat ", ")
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
        |> List.map (fun v -> $"export var default{v.Name} = {v.DefaultValue}")
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
        + $"export var {name}_AllCases = {casesArray} as const"
        + "\n"
        + defaults
        + "\n"
        + match genericName with
          | Some genericName ->
            $"export var default{name} = {t.Id.TsSignature.GenericParameters()}{t.Id.TsSignature.GenericArguments()} => null as any as {genericName}"
          | None -> $"export var default{name} = null as any as {name}"
      )

  | TypeAndValue (name, definition, defaultValue, inlineValue) ->
    Some(
      $"export type {name} = {definition}\n"
      + $"export var default{name}: {name} = {defaultValue}"
    )
  | GenericTypeAndFunction (name, genericName, definition, defaultGeneratorSignature, defaultGeneratorimpl, _) ->
    Some(
      $"export type {genericName} = {definition}\n"
      + $"export var default{name}: {defaultGeneratorSignature} => {genericName} = {defaultGeneratorSignature} => {defaultGeneratorimpl}"
    )

let renderTypeAndDefaultValue (t: TsType) =
  renderKnownTypeAndDefaultValue t RenderCyclicDefault.NoCycle DefaultSerialize.serialize

let sortItemsTopologically (items: TsType list) =
  items
  |> List.distinctBy (fun v -> v.Id.TsSignature)
  |> List.filter Glow.GetTsSignature.isNonGenericTypeOrGenericTypeDefinition
  |> Glow.TopologicalSort.topologicalSort (fun v ->
    let name = v.Id.OriginalName

    v.Dependencies
    |> List.filter (fun dependency ->

      dependency.Id.TsSignature.TsNamespace = v.Id.TsSignature.TsNamespace))

let renderModule (m: Namespace) : string =

  let name = m.Name |> NamespaceName.value
  let deps = getDependencies m

  let builder = StringBuilder()

  builder
    .AppendLine("//////////////////////////////////////")
    .AppendLine("//   This file is auto generated   //")
    .AppendLine("//////////////////////////////////////")
    .AppendLine("")
  |> ignore

  // builder.AppendLine("import * as TsType from \"./TsType\"") |> ignore
  builder.AppendLine("import {TsType} from \"./\"") |> ignore

  deps
  |> List.iter (fun v ->
    let name = v.TsSignature.TsNamespace |> NamespaceName.sanitize

    if v.Id = TsTypeId "Any" || name = null then
      ()
    elif name = "" || name = null then
      builder.AppendLine($"// skipped importing empty namespace (type={v.OriginalName})")
      |> ignore
    else
      // let x = $@"import * as {name} from ""./{name}"""
      let x = $@"import {{{name}}} from ""./"""

      if x = "import * as  from \"./\"" then
        ()
      else
        builder.AppendLine(x) |> ignore

    ())

  builder.AppendLine() |> ignore
  let sorted, cyclics = sortItemsTopologically m.Items

  if cyclics.Length > 0 then
    builder
      .AppendLine("//*** Cyclic dependencies dected ***")
      .AppendLine("//*** this can cause problems when generating types and defualt values ***")
      .AppendLine("//*** Please ensure that your types don't have cyclic dependencies ***")
    |> ignore

    cyclics
    |> List.iter (fun v -> builder.AppendLine("//" + v.Id.OriginalName) |> ignore)

    builder.AppendLine("//*** ******************* ***").AppendLine("") |> ignore

  sorted
  |> List.distinctBy (fun v -> v.Id.TsSignature)
  |> List.map (fun v ->
    let isCyclic = cyclics |> List.contains (v)

    let cycle =
      if isCyclic then
        RenderCyclicDefault.Stub
      else
        RenderCyclicDefault.NoCycle

    let x = renderKnownTypeAndDefaultValue v cycle DefaultSerialize.serialize

    match x with
    | Some x -> x
    | None -> $"// skipped {v.Id.TsSignature.TsName |> TsName.value}")
  |> List.map Utils.cleanTs
  |> List.iter (fun v -> builder.AppendLine(v) |> ignore)

  if cyclics.Length > 0 then
    builder.AppendLine("// Render cyclic fixes") |> ignore

  cyclics
  |> List.distinctBy (fun v -> v.Id.TsSignature)
  |> List.map (fun v ->

    let x =
      renderKnownTypeAndDefaultValue v RenderCyclicDefault.Fix DefaultSerialize.serialize

    match x with
    | Some x -> x
    | None -> $"// skipped {v.Id.TsSignature.TsName |> TsName.value}")
  |> List.map Utils.cleanTs
  |> List.iter (fun v -> builder.AppendLine(v) |> ignore)

  builder.ToString().Replace("\r\n", "\n")

let findeTsTypeInModules modules t =
  let id = (Glow.GetTsSignature.getModuleNameAndId t).Id

  modules |> List.collect (fun v -> v.Items) |> List.find (fun v -> v.Id.Id = id)
