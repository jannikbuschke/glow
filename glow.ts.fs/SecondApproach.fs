module Glow.SecondApproach

open System.Reflection
open System.Text
open System.Text.Json.Serialization
open Glow.Ts
open Microsoft.FSharp.Reflection
open TypeCache


type PredefinedValues =
  { InlineDefaultValue: string option
    Name: string option
    Definition: string option
    Signature: string option
    Dependencies: System.Type list } // Name<Generic,Args>

let emptyPredefinedValues =
  { InlineDefaultValue = Some "''"
    Name = None
    Definition = None
    Signature = None
    Dependencies = [] }

let d =
  dict [ (typeof<string>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "''"
              Definition = Some "string" })
         (typeof<System.Byte>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "0"
              Definition = Some "number" })

         (typedefof<Option<_>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "null"
              Definition = Some "T | null" })
         (typeof<System.Guid>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some $"'{(System.Guid.Empty.ToString())}'"
              Definition = Some "`${number}-${number}-${number}-${number}-${number}`" })
         (typeof<int>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "0"
              Definition = Some "number" })
         (typeof<int64>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "0"
              Definition = Some "number" })
         (typeof<float>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "0.0"
              Definition = Some "number" })
         (typeof<double>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "0.0"
              Definition = Some "number" })
         (typeof<bigint>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "0"
              Definition = Some "number" })
         (typeof<bool>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "false"
              Definition = Some "boolean" })
         (typeof<obj>, { emptyPredefinedValues with InlineDefaultValue = Some "{}" })
         (typeof<unit>, { emptyPredefinedValues with InlineDefaultValue = Some "({})" })
         (typedefof<System.Tuple<_, _>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "[defaultT1,defaultT2]"
              Definition = Some "[T1,T2]" })
         (typedefof<System.Collections.Generic.IEnumerable<_>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "[]"
              Definition = Some "Array<T>" })
         (typedefof<System.Collections.Generic.IList<_>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "[]"
              Definition = Some "Array<T>" })
         (typedefof<System.Collections.Generic.IReadOnlyCollection<_>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "[]"
              Definition = Some "Array<T>" })
         (typedefof<System.Collections.Generic.IReadOnlyList<_>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "[]"
              Definition = Some "Array<T>" })
         (typedefof<System.Collections.Generic.IReadOnlySet<_>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "[]"
              Definition = Some "Array<T>" })
         (typedefof<System.Collections.Generic.List<_>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "[]"
              Definition = Some "Array<T>" })
         (typedefof<Skippable<_>>, { emptyPredefinedValues with InlineDefaultValue = Some "undefined" })
         (typedefof<obj>, { emptyPredefinedValues with InlineDefaultValue = Some "{}" })
         (typedefof<System.DateTime>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "\"0001-01-01T00:00:00\""
              Definition = Some "`${number}-${number}-${number}T${number}:${number}:${number}`" })
         (typedefof<System.DateTimeOffset>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "\"0000-00-00T00:00:00+00:00\""
              Definition =
                Some "`${number}-${number}-${number}T${number}:${number}:${number}${\"+\"|\"-\"}${number}:${number}`" })
         (typedefof<System.TimeSpan>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "\"00:00:00\""
              Definition = Some "`${number}:${number}:${number}`" })
         (typedefof<System.Collections.Generic.KeyValuePair<_, _>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "({Key:defaultTKey,Value:defaultTValue})"
              Definition = Some "({Key:TKey,Value:TValue})" })
         (typedefof<System.Collections.Generic.Dictionary<_, _>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "({})"
              Definition = Some "{ [key: string]: TValue }" })
         (typedefof<System.Type>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "({})"
              Definition = Some "{  }" })
         (typedefof<System.Nullable<_>>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "null"
              Definition = Some "T | null" })
         (typedefof<NodaTime.LocalDate>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "\"2022-12-18\""
              Definition = Some "`${number}-${number}-${number}`" })
         (typedefof<NodaTime.LocalTime>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "\"00:00:00\""
              Definition = Some "`${number}:${number}:${number}`" })
         (typedefof<NodaTime.Instant>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "\"9999-12-31T23:59:59.999999999Z\""
              Definition = Some "`${number}-${number}-${number}T${number}:${number}:${number}.${number}Z`" })
         (typedefof<NodaTime.Duration>,
          { emptyPredefinedValues with
              InlineDefaultValue = Some "\"0:00:00\""
              Definition = Some "`${number}:${number}:${number}`" }) ]

let defaultTypes =
  System.Collections.Generic.Dictionary<System.Type, PredefinedValues>(d)

let getModuleName (t: System.Type) =
  if t.Namespace <> null then
    t.Namespace.Replace(".", "_")
  else
    failwith "null namespace currently not supported"

let getName (t: System.Type) =
  // if t.IsArray then
  //   let split = t.Name.Replace("[]", "Array").Split("`")
  //   t.Name.Replace("[]", "Array").Split("`").[0]
  // else
  let split = t.Name.Split("`")
  let name = t.Name.Split("`").[0]

  if t.IsArray then
    name.Replace("[]", "") + "Array"
  // name + "Array"
  else
    name

let getSignature (t: System.Type) =
  let modulName = getModuleName t
  let name = getName t
  // TODO: add generic arguments
  modulName, name

//
// Microsoft_FSharp_Core.FSharpResult<Microsoft_FSharp_Core.FSharpResult<Microsoft_FSharp_Collections.FSharpList>,Microsoft_FSharp_Core.FSharpResult>



// <T>
let rec getGenericParameters (callingModule: string) (t: System.Type) =
  if t.IsGenericType then
    "<"
    + (t.GenericTypeArguments
       |> Seq.map (fun v ->
         let modulName = getModuleName v
         let name = getName v

         let name =
           if modulName = callingModule then
             name
           else
             modulName + "." + name

         $"{name}{getGenericParameters callingModule v}")
       |> String.concat ",")
    + ">"
  else
    ""

// (defaultT:T)
let rec getGenericParameterValues (callingModule: string) (t: System.Type) =
  if t.IsGenericType then
    "("
    + (t.GenericTypeArguments
       |> Seq.map (fun v ->
         let modulName = getModuleName v
         let name = "default" + (getName v)

         let name =
           if modulName = callingModule then
             name
           else
             modulName + "." + name

         $"{name}{getGenericParameterValues callingModule v}")
       |> String.concat ",")
    + ")"
  else
    ""

let getFullTypeName (t: System.Type) = t.FullName

// Module.TypeName
let rec getPropertySignature (callingModule: string) (t: System.Type) =
  let kind = getKind t

  match kind with
  | TypeKind.Array ->
    getPropertySignature
      callingModule
      (typedefof<System.Collections.Generic.IEnumerable<_>>.MakeGenericType (t.GetElementType()))
  | _ ->
    let moduleName, name = getSignature t

    let name =
      if moduleName = callingModule then
        name
      else
        moduleName + "." + name

    if t.IsGenericType then
      name + (getGenericParameters callingModule t)
    else
      let modulName = getModuleName t
      let name = getName t

      if modulName = callingModule then
        name
      else
        modulName + "." + name

let renderPropertyNameAndDefinition (callingModule: string) (fieldInfo: System.Reflection.PropertyInfo) =
  let signature = getPropertySignature callingModule fieldInfo.PropertyType
  let name = fieldInfo.Name
  $"""  {Utils.camelize name}: {signature}"""

let renderSingleFieldUnionCaseDefinition
  (callingModule: string)
  (case: UnionCaseInfo)
  (fieldInfo: System.Reflection.PropertyInfo)
  =
  $"""{{ Case: "{case.Name}", Fields: {getPropertySignature callingModule fieldInfo.PropertyType} }}"""

let renderMultiFieldUnionCaseDefinition
  (callingModule: string)
  (case: UnionCaseInfo)
  (fieldInfo: System.Reflection.PropertyInfo list)
  =
  let fields =
    fieldInfo
    |> List.map (fun v ->
      (Utils.camelize v.Name)
      + ": "
      + getPropertySignature callingModule v.PropertyType)
    |> String.concat ", "

  $"""{{ Case: "{case.Name}", Fields: {{ {fields} }} }}"""

let getDefaultValue (callingModule: string) (propertyType: System.Type) =
  let moduleName = getModuleName propertyType
  let name = getName propertyType
  let isGeneric = propertyType.IsGenericType
  let kind = getKind propertyType

  let prefix =
    if moduleName = callingModule then
      ""
    else
      moduleName + "."

  let kind = getKind propertyType

  let prefedinedValue =
    match
      defaultTypes.TryGetValue
        (
          if propertyType.IsGenericType
             && not propertyType.IsGenericTypeDefinition then
            propertyType.GetGenericTypeDefinition()
          else
            propertyType
        )
      with
    | true, value -> value.InlineDefaultValue
    | _ -> None

  let postfix =
    if isGeneric then
      (getGenericParameterValues callingModule propertyType)
    else
      ""

  let value =
    prefedinedValue
    |> Option.defaultValue (
      match kind with
      | TypeKind.Record -> $"""{prefix}default{name}{postfix}"""
      | TypeKind.List -> "[] "
      | TypeKind.Array -> "[]"
      | TypeKind.Map ->
        if propertyType.GenericTypeArguments.[0] = typeof<string> then
          "({})"
        else
          "[]"
      | TypeKind.Union -> $"""{prefix}default{name}{postfix}"""
      | _ -> $"""{prefix}default{name}{postfix}"""
    )

  value

let renderSingleFieldUnionCaseDefaultValue
  (callingModule: string)
  (case: UnionCaseInfo)
  (fieldInfo: System.Reflection.PropertyInfo)
  =
  let defaultValue = getDefaultValue callingModule fieldInfo.PropertyType
  let propertyTypeName = fieldInfo.PropertyType.Name

  if fieldInfo.PropertyType.IsGenericParameter then
    $"""({{ Case: "{case.Name}", Fields: {defaultValue} }})"""
  else
    $"""{{ Case: "{case.Name}", Fields: {defaultValue} }}"""

let renderPropertyNameAndValue (camelize: bool) (callingModule: string) (fieldInfo: System.Reflection.PropertyInfo) =
  let propertyType = fieldInfo.PropertyType
  // let moduleName = getModuleName fieldInfo.PropertyType
  // let name = getName propertyType
  // let isGeneric = propertyType.IsGenericType
  // let kind = getKind fieldInfo.PropertyType
  // let prefix = if moduleName = callingModule then "" else moduleName + "."
  //
  // let postfix =
  //   if isGeneric then
  //     (getGenericParameterValues callingModule propertyType)
  //   else
  //     ""
  //
  // let prefedinedValue =
  //   match
  //     defaultTypes.TryGetValue(
  //       if propertyType.IsGenericType && not propertyType.IsGenericTypeDefinition then
  //         propertyType.GetGenericTypeDefinition()
  //       else
  //         propertyType
  //     )
  //   with
  //   | true, value -> value.InlineDefaultValue
  //   | _ -> None
  //
  // let value =
  //   prefedinedValue
  //   |> Option.defaultValue (
  //     match kind with
  //     | TypeKind.Record -> $"""{prefix}default{name}{postfix}"""
  //     | TypeKind.List -> "[]"
  //     | TypeKind.Array -> "[]"
  //     | TypeKind.Map ->
  //       if propertyType.GenericTypeArguments.[0] = typeof<string> then
  //         "({})"
  //       else
  //         "[]"
  //     | TypeKind.Union -> $"""{prefix}default{name}{postfix}"""
  //     | _ -> $"""{prefix}default{name}{postfix}"""
  //   )

  let value = getDefaultValue callingModule propertyType

  if camelize then
    $"""  {Utils.camelize fieldInfo.Name}: {value}"""
  else
    $"""  {fieldInfo.Name}: {value}"""

let renderMultiFieldUnionCaseDefaultValue
  (callingModule: string)
  (case: UnionCaseInfo)
  (fieldInfo: System.Reflection.PropertyInfo list)
  =

  let isGeneric =
    fieldInfo
    |> List.exists (fun v -> v.PropertyType.IsGenericParameter)

  let fields =
    fieldInfo
    |> List.map (fun v -> renderPropertyNameAndValue false callingModule v)
    |> String.concat ", "

  if isGeneric then
    $"""({{ Case: "{case.Name}", Fields: {{ {fields} }})"""
  else
    $"""{{ Case: "{case.Name}", Fields: {{ {fields} }}  }}"""


let genericArgumentList (t: System.Type) =
  match t.GetGenericArguments() |> Seq.toList with
  | [] -> ""
  | arguments ->
    "<"
    + (arguments
       |> List.map (fun v -> v.Name)
       |> String.concat ",")
    + ">"

let genericArgumentListAsParameters (t: System.Type) =
  "("
  + (t.GetGenericArguments()
     |> Array.map (fun v -> "default" + v.Name + ":" + v.Name)
     |> String.concat ",")
  + ")"

let genericArgumentListAsParametersCall (t: System.Type) =
  "("
  + (t.GetGenericArguments()
     |> Array.map (fun v -> "default" + v.Name)
     |> String.concat ",")
  + ")"

// let getSingleFieldCaseName (name: string) (singleField: System.Reflection.PropertyInfo) (case: UnionCaseInfo) =
//   let name = $"{name}_Case_{case.Name}"
//   name

let getFieldCaseName (name: string) (case: UnionCaseInfo) =
  let name = $"{name}_Case_{case.Name}"
  name

let getSingleFieldCaseSignature (name: string) (singleField: System.Reflection.PropertyInfo) (case: UnionCaseInfo) =
  let name = getFieldCaseName name case

  let genericParameterPostfix =
    if singleField.PropertyType.IsGenericParameter then
      "<" + singleField.PropertyType.Name + ">"
    else
      ""

  let name = $"{name}{genericParameterPostfix}"
  name

let getMultiFieldCaseSignature (name: string) (fields: System.Reflection.PropertyInfo list) (case: UnionCaseInfo) =
  let name = getFieldCaseName name case

  let isGeneric =
    fields
    |> List.exists (fun v -> v.PropertyType.IsGenericParameter)

  let genericParameterPostfix =
    if isGeneric then
      failwith "TODO"
    else
      ""

  let name = $"{name}{genericParameterPostfix}"
  name

let getAnonymousFunctionSignatureForDefaultValue (t: System.Type) =
  let genericArguments = genericArgumentList t
  let parameters = genericArgumentListAsParameters t
  genericArguments + parameters

let getNamedFunctionSignatureForDefaultValue (t: System.Type) =
  let name = getName t
  // let genericArguments = genericArgumentList t
  // let parameters = genericArgumentListAsParameters t
  // genericArguments + parameters
  let genericArguments = genericArgumentList t
  let signature = name + genericArguments
  signature

let renderDu (t: System.Type) =

  let callingModule = getModuleName t

  let name = getName t
  let cases = (FSharpType.GetUnionCases t) |> Seq.toList

  let caseNameLiteral =
    cases
    |> List.map (fun v -> $"\"{v.Name}\"")
    |> String.concat " | "

  let allCaseNames =
    cases
    |> List.map (fun v -> $"\"{v.Name}\"")
    |> String.concat ", "

  let renderedCaseDefinitions =
    match cases with
    | [] -> failwith "todo"
    | [ singleCase ] ->
      let caseFields = singleCase.GetFields() |> Seq.toList

      match caseFields with
      | [] -> $"""export type {singleCase.Name} = {singleCase.Name} // DU single case no fields"""
      | [ singleField ] ->
        let singleFieldCaseSignature =
          getSingleFieldCaseSignature name singleField singleCase

        let prop = getPropertySignature callingModule singleField.PropertyType
        $"""export type {singleFieldCaseSignature} = {prop}"""
      | fields -> failwith "todo"
    // export var defaultMyRecordId: MyRecordId = System.defaultGuid
    | cases ->
      let renderCase (case: UnionCaseInfo) =

        let caseFields = case.GetFields() |> Seq.toList

        match caseFields with
        | [] ->
          let fieldNames =
            caseFields
            |> List.map (fun v -> v.Name)
            |> String.concat ", "

          $"""export type {name}_Case_{case.Name} = {{ Case: "{case.Name}" }}"""
        | [ singleField ] ->
          let singleFieldCaseSignature = getSingleFieldCaseSignature name singleField case

          let singleFieldUnionCaseDefinition =
            renderSingleFieldUnionCaseDefinition callingModule case singleField

          $"""export type {singleFieldCaseSignature} = {singleFieldUnionCaseDefinition}"""
        | fields ->
          let singleFieldCaseSignature = getMultiFieldCaseSignature name fields case

          let multiFieldUnionCaseDefinition =
            renderMultiFieldUnionCaseDefinition callingModule case fields

          $"""export type {singleFieldCaseSignature} = {multiFieldUnionCaseDefinition}"""

      //$"""export type {name}_Case_{case.Name}<T> = {{ Case: "{case.Name}", Fields: T }} // (multiple fields)"""
      cases |> List.map renderCase |> String.concat "\r"

  // let parameters = genericArgumentListAsParameters t
  let anonymousFunctionSignature = getAnonymousFunctionSignatureForDefaultValue t

  let renderedCaseDefaultNamesAndValues =
    match cases with
    | [] -> failwith "todo"
    | [ singleCase ] ->
      let caseFields = singleCase.GetFields() |> Seq.toList

      match caseFields with
      | [] -> failwith "todo"
      | [ singleField ] ->
        let singleFieldCaseSignature = getFieldCaseName name singleCase
        let unwrappedValue = getDefaultValue callingModule singleField.PropertyType

        let singleFieldUnionCaseDefaultValue =
          renderSingleFieldUnionCaseDefaultValue callingModule singleCase singleField
        // unrwap
        $"""export var default{singleFieldCaseSignature} = {unwrappedValue}"""
      | fields -> failwith "todo"
    | cases ->
      let renderCase (case: UnionCaseInfo) =
        let caseFields = case.GetFields() |> Seq.toList

        match caseFields with
        | [] ->
          //$"""export type {name}_Case_{case.Name} = {{ Case: "{case.Name}" }}"""
          $"export var default{name}_Case_{case.Name} = {{ Case: \"{case.Name}\" }}"
        // failwith "todo"
        | [ singleField ] ->
          let singleFieldCaseSignature = getFieldCaseName name case

          let singleFieldUnionCaseDefaultValue =
            renderSingleFieldUnionCaseDefaultValue callingModule case singleField

          if t.IsGenericType then
            $"""export var default{singleFieldCaseSignature} = {anonymousFunctionSignature} => {singleFieldUnionCaseDefaultValue}"""
          else
            $"""export var default{singleFieldCaseSignature} = {singleFieldUnionCaseDefaultValue}"""
        // $"""export var default{singleFieldCaseSignature} = {anonymousFunctionSignature} => {singleFieldUnionCaseDefaultValue}"""
        | fields ->
          let signature = getFieldCaseName name case

          let multiFieldUnionCaseDefaultValue =
            renderMultiFieldUnionCaseDefaultValue callingModule case fields

          if t.IsGenericType then
            $"""export var default{signature} = {anonymousFunctionSignature} => {multiFieldUnionCaseDefaultValue}"""
          else
            $"""export var default{signature} = {multiFieldUnionCaseDefaultValue}"""

      cases |> List.map renderCase |> String.concat "\r"

  let firstCaseName =
    match cases with
    | [] -> failwith "todo"
    | [ case ] ->
      let singleFieldCaseSignature = getFieldCaseName name case
      $"""default{singleFieldCaseSignature}"""
    | cases ->
      let case = cases.Head
      let singleFieldCaseSignature = getFieldCaseName name case
      $"""default{singleFieldCaseSignature}"""
  // match cases with
  // | [] -> failwith "todo"
  // | [ case ] ->
  //     let singleFieldCaseSignature = getFieldCaseName name case
  //     $"""default{singleFieldCaseSignature}"""
  // | cases ->
  //   let case = cases |> List.head
  //   let caseFields = case.GetFields() |> Seq.toList
  //   match caseFields with
  //   | [] -> failwith "todo"
  //   | [ singleField ] ->
  //     let singleFieldCaseSignature = getFieldCaseName name case
  //     $"""default{singleFieldCaseSignature}"""
  //   | fields ->
  //     let singleFieldCaseSignature = getFieldCaseName name case
  //     $"""default{singleFieldCaseSignature}"""

  let renderCaseSignature (case: UnionCaseInfo) =
    let caseFields = case.GetFields() |> Seq.toList

    match caseFields with
    | [] -> $"""{name}_Case_{case.Name}"""
    // failwith "todo"
    | [ singleField ] ->
      let singleFieldCaseSignature = getSingleFieldCaseSignature name singleField case
      singleFieldCaseSignature
    | fields ->
      let singleFieldCaseSignature = getMultiFieldCaseSignature name fields case
      singleFieldCaseSignature
  //$"""export type {name}_Case_{case.Name}<T> = {{ Case: "{case.Name}", Fields: T }} // (multiple fields)"""
  let caseSignatures =
    cases
    |> List.map renderCaseSignature
    |> String.concat " | "

  let callParameters = genericArgumentListAsParametersCall t

  let genericArguments = genericArgumentList t
  let signature = name + genericArguments
  let signature = getNamedFunctionSignatureForDefaultValue t
  let defaultCase = firstCaseName

  let renderedDefaultCase =
    if t.IsGenericType then
      $"""export var default{name} = {anonymousFunctionSignature} => {defaultCase}{callParameters} as {signature}"""
    else
      $"""export var default{name} = {defaultCase} as {signature}"""

  $"""{renderedCaseDefinitions}
export type {signature} = {caseSignatures}
export type {name}_Case = {caseNameLiteral}
export var {name}_AllCases = [ {allCaseNames} ] as const
{renderedCaseDefaultNamesAndValues}
{renderedDefaultCase}
"""
//
type RenderStrategy =
  | RenderDefinitionAndValue
  | RenderDefinition
  | RenderValue

let renderRecord (t: System.Type) (strategy: RenderStrategy) =
  if t.IsGenericType && not t.IsGenericTypeDefinition then
    failwith "A definition and value for a generic type that is not a generic type definition cannot be rendered"

  let callingModule = getModuleName t
  let name = getName t
  let properties = t.GetProperties(BindingFlags.Public ||| BindingFlags.Instance)

  let fields =
    properties
    |> Array.map (renderPropertyNameAndDefinition callingModule)
    |> String.concat "\r"

  let genericArguments = genericArgumentList t

  let fieldValues =
    properties
    |> Array.map (renderPropertyNameAndValue true callingModule)
    |> String.concat ",\r"

  let anonymousFunctionSignature = getAnonymousFunctionSignatureForDefaultValue t
  let namedFunctionSignature = getNamedFunctionSignatureForDefaultValue t

  let value =
    match strategy with
    | RenderDefinition -> ""
    | RenderValue -> $"{{{fieldValues}}}"
    | _ ->
      if t.IsGenericTypeDefinition then
        $"""export var default{name}: {anonymousFunctionSignature} => {namedFunctionSignature} = {anonymousFunctionSignature} => ({{
{fieldValues}
}})"""
      else
        $"""export var default{name}: {name} = {{
{fieldValues}
}}"""

  match strategy with
  | RenderValue -> value
  | _ ->
    $"""export type {name}{genericArguments} = {{
{fields}
}}

{value}
"""

let renderPredefinedTypeFromDefaultValue (t: System.Type) (predefined: PredefinedValues) =
  let name = getName t
  let genericArguments = genericArgumentList t
  let callingModule = getModuleName t
  let parameterCall = genericArgumentListAsParametersCall t

  if t.IsGenericType then
    let anonymousFunctionSignature = getAnonymousFunctionSignatureForDefaultValue t
    //
    // let singleFieldCaseSignature = getSingleFieldCaseName name singleField case
    //
    // let singleFieldUnionCaseDefaultValue =
    //   renderSingleFieldUnionCaseDefaultValue callingModule case singleField
    //
    // $"""export var default{singleFieldCaseSignature} = {anonymousFunctionSignature} => {singleFieldUnionCaseDefaultValue}"""

    $"""
export type {name}{genericArguments} = {(predefined.Definition
                                         |> Option.defaultValue "unknown // renderPredefinedTypeFromDefaultValue (generic)")}
export var default{name}: {anonymousFunctionSignature} => {name}{genericArguments} = {anonymousFunctionSignature} => {predefined.InlineDefaultValue
                                                                                                                      |> Option.defaultValue "unknown"}
"""
  else
    $"""
export type {name} = {(predefined.Definition |> Option.defaultValue "any")}
export var default{name}: {name} = {predefined.InlineDefaultValue
                                    |> Option.defaultValue "unknown // renderPredefinedTypeFromDefaultValue (not generic)"}
"""

let renderStubValue (t: System.Type) =
  let name = getName t
  $"""export var default{name}: {name} = {{ }} as any as {name}"""

let rec renderType (t: System.Type) (strategy: RenderStrategy) =
  let kind = getKind t

  let predefinedDefinitionAndValue =
    match
      defaultTypes.TryGetValue
        (
          if t.IsGenericType && not t.IsGenericTypeDefinition then
            t.GetGenericTypeDefinition()
          else
            t
        )
      with
    | true, value ->
      if t.IsGenericType && not t.IsGenericTypeDefinition then
        Some(renderPredefinedTypeFromDefaultValue (t.GetGenericTypeDefinition()) value)
      else
        Some(renderPredefinedTypeFromDefaultValue t value)
    | _ -> None

  if predefinedDefinitionAndValue.IsSome then
    predefinedDefinitionAndValue.Value
  else
    match kind with
    | TypeKind.List ->
      $"""export type FSharpList<T> = Array<T> // fullname {t.FullName}
export var defaultFSharpList: <T>(t:T) => FSharpList<T> = <T>(t:T) => []
"""

    | TypeKind.Record ->
      if t.IsGenericType && not t.IsGenericTypeDefinition then
        ""
      else
        renderRecord t strategy
    | TypeKind.Union -> renderDu t
    | TypeKind.Array ->
      let name = getName t

      $"""export type {name}<T> = Array<T> // fullname {t.FullName}
export var default{name}: <T>(t:T) => {name}<T> = <T>(t:T) => []
"""

    // renderType (typedefof<System.Collections.Generic.List<_>>.MakeGenericType (t.GetElementType()))
    | TypeKind.Map ->
      //if key type is string, use normal object

      if not t.IsGenericTypeDefinition
         && t.GenericTypeArguments.[0] = typeof<string> then
        """export type FSharpStringMap<TValue> = { [key: string ]: TValue }
  export var defaultFSharpStringMap: <TValue>(t:string,tValue:TValue) => FSharpStringMap<TValue> = <TValue>(t:string,tValue:TValue) => ({})"""
      else
        """export type FSharpMap<TKey, TValue> = [TKey,TValue][]
  export var defaultFSharpMap: <TKey, TValue>(tKey:TKey,tValue:TValue) => FSharpMap<TKey, TValue> = <TKey, TValue>(tKey:TKey,tValue:TValue) => []"""
    | TypeKind.Enum ->
      let name = getName t

      let values =
        t.GetEnumNames()
        |> Array.map (fun v -> $"\"{v}\"")
        |> Array.toList

      $"""export type {name} = {values |> String.concat " | "}
  export var {name}_AllValues = [{values |> String.concat ", "}] as const
  export var default{name}: {name} = {values |> List.head}
      """
    | x ->
      if t.IsGenericParameter then
        ""
      else
        // probably a class, try to use same strategy as record
        renderRecord t strategy

// export type EntityState = "Detached" | "Unchanged" | "Deleted" | "Modified" | "Added"
// export var EntityState_AllValues = ["Detached", "Unchanged", "Deleted", "Modified", "Added"] as const
// export var defaultEntityState: EntityState = "Detached"

let rec getGenericDefinitionAndArgumentsAsDependencies (t: System.Type) =
  if t.IsGenericTypeDefinition then
    failwith "t is a generic type definition but should not be"

  if not t.IsGenericType then
    failwith "t is not a generic type but should be"

  let genericDefinition = t.GetGenericTypeDefinition()

  let args =
    t.GenericTypeArguments
    |> Seq.collect (fun v ->
      if v.IsGenericType && not v.IsGenericTypeDefinition then
        getGenericDefinitionAndArgumentsAsDependencies v
      else
        [ v ])
    |> Seq.toList

  [ genericDefinition ] @ args

let _getDependencies (t: System.Type) =
  match defaultTypes.TryGetValue t with
  | true, value -> value.Dependencies
  | _ ->
    let kind = getKind t

    let result =
      match kind with
      | TypeKind.Other
      | TypeKind.Record ->
        (t.GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
         |> Seq.collect (fun f ->
           if not f.PropertyType.IsGenericType
              || (f.PropertyType.IsGenericType
                  && f.PropertyType.IsGenericTypeDefinition) then
             [ f.PropertyType ]
           else
             getGenericDefinitionAndArgumentsAsDependencies f.PropertyType)
         |> Seq.toList)


      | TypeKind.Union ->
        let x =
          (FSharpType.GetUnionCases t)
          |> Seq.collect (fun c ->
            c.GetFields()
            |> Array.map (fun f -> f.PropertyType)
            |> Array.toList)
          |> Seq.toList

        x
      | TypeKind.Array -> [ t.GetElementType() ]
      | _ -> []

    let genericArgs =
      if not t.IsGenericType
         || (t.IsGenericType && t.IsGenericTypeDefinition) then
        []
      else
        getGenericDefinitionAndArgumentsAsDependencies t

    result @ genericArgs

let getDependencies (t: System.Type) = _getDependencies t |> List.distinct

type TsModule =
  { Name: string
    Types: System.Type list }

let getFilteredDeps (moduleName: string) (t: System.Type) =
  getDependencies t
  |> List.filter (fun v ->
    let name = getModuleName v
    name = moduleName)

let allTypes = System.Collections.Generic.HashSet<System.Type>()

let rec collectDependenciesTransitively (depth: int) (t: System.Type) =
  if depth > 20 then failwith "too deep"

  if allTypes.Contains t then
    ()
  else
    allTypes.Add t |> ignore

    t
    |> getDependencies
    |> List.iter (collectDependenciesTransitively (depth + 1))

    ()


let collectModules (types: System.Type list) =
  (typedefof<obj> :: typedefof<System.Byte> :: types)
  |> List.iter (collectDependenciesTransitively (0))
  // let deps =
  //     types
  //     |> List.collect getDependencies

  allTypes
  |> Seq.toList
  |> List.groupBy getModuleName
  |> List.map (fun (v, items) ->
    let items =
      items
      |> List.map (fun v ->
        if v.IsGenericType && not v.IsGenericTypeDefinition then
          v.GetGenericTypeDefinition()
        else
          v)

    let x =
      items
      |> List.map (fun v -> v.FullName)
      |> String.concat "\n"

    let sorted, cyclics =
      Glow.GenericTopologicalSort.topologicalSort (getFilteredDeps v) (items |> List.distinct)

    let sorted2 = sorted |> List.distinct
    { Name = v; Types = sorted2 })

let getModuleDependencies (n: TsModule) =
  let deps =
    n.Types
    |> List.filter (fun t ->
      (not t.IsGenericType)
      || (t.IsGenericType && t.IsGenericTypeDefinition))
    |> List.collect getDependencies
    |> List.map getModuleName

  (deps @ [ "System_Collections_Generic" ]
   |> List.distinct
   |> List.filter (fun v -> v <> n.Name))

let renderModule (m: TsModule) =

  let deps = getModuleDependencies m

  let builder = StringBuilder()

  builder
    .AppendLine("//////////////////////////////////////")
    .AppendLine("//   This file is auto generated   //")
    .AppendLine("//////////////////////////////////////")
    .AppendLine("")
  |> ignore

  // builder.AppendLine("import * as TsType from \"./TsType\"") |> ignore
  // builder.AppendLine("import {TsType} from \"./\"") |> ignore

  deps
  |> List.iter (fun v ->
    builder.AppendLine($"import * as {v} from \"./{v}\"")
    |> ignore)

  // deps
  // |> List.iter (fun v ->
  //   let name = v.TsSignature.TsNamespace |> NamespaceName.sanitize
  //
  //   if v.Id = TsTypeId "Any" || name = null then
  //     ()
  //   elif name = "" || name = null then
  //     builder.AppendLine($"// skipped importing empty namespace (type={v.OriginalName})")
  //     |> ignore
  //   else
  //     let x = $@"import {{{name}}} from ""./"""
  //
  //     if x = "import * as  from \"./\"" then
  //       ()
  //     else
  //       builder.AppendLine(x) |> ignore
  //
  //   ())


  builder.AppendLine() |> ignore
  let sorted = m.Types

  let sorted, cyclics =
    Glow.GenericTopologicalSort.topologicalSort
      (fun v ->
        let deps = getDependencies v

        // Nullable remove
        deps
        |> List.filter (fun x -> x.Namespace = v.Namespace))


      m.Types

  if cyclics.Length > 0 then
    builder
      .AppendLine("//*** Cyclic dependencies dected ***")
      .AppendLine("//*** this can cause problems when generating types and defualt values ***")
      .AppendLine("//*** Please ensure that your types don't have cyclic dependencies ***")
    |> ignore

    cyclics
    |> List.iter (fun v -> builder.AppendLine("// " + v.Name) |> ignore)

    builder
      .AppendLine("//*** ******************* ***")
      .AppendLine("")
    |> ignore


  sorted
  |> List.distinct
  |> List.map (fun v ->

    let isCyclic = cyclics |> List.contains v

    let result =
      // workaround for Bullable<Guid>
      if
        v.IsGenericType && not v.IsGenericTypeDefinition
        && v.Name.Contains("Nullable")
      then
        ""
      else if isCyclic then
        $"""
// This type has cyclic dependencies: {v.FullName}
// in general this should be avoided. We render a 'stub' value here that will be changed at the bottom of this file
{renderType v RenderStrategy.RenderDefinition}
{renderStubValue v}
"""
      else
        renderType v RenderStrategy.RenderDefinitionAndValue

    result)

  |> List.map Utils.cleanTs
  |> List.iter (fun v -> builder.AppendLine(v) |> ignore)

  if cyclics.Length > 0 then
    builder.AppendLine("// Render cyclic fixes")
    |> ignore

  cyclics
  |> List.distinct
  |> List.map (fun v ->
    let name = getName v

    $"""//
// the type {v.FullName} has cyclic dependencies
// in general this should be avoided
//
Object.assign(default{name}, ({renderType v RenderStrategy.RenderValue}))
"""
  // let x =
  //   renderKnownTypeAndDefaultValue v RenderCyclicDefault.Fix DefaultSerialize.serialize
  //
  // match x with
  // | Some x -> x
  // | None -> $"// skipped {v.Id.TsSignature.TsName |> TsName.value}"
  )
  |> List.map Utils.cleanTs
  |> List.iter (fun v -> builder.AppendLine(v) |> ignore)

  builder.ToString().Replace("\r\n", "\n")


// let renderTypes (types: System.Type list) =
//   let allTypes = types |> List.collect getDependencies
//
//   let x =
//     allTypes
//     |> List.groupBy getModuleName
//     |> List.map (fun (v, items) -> { Name = v; Types = items })
//
//   ""