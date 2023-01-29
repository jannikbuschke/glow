module Glow.SecondApproach

open System.Text.Json.Serialization
open Glow.Ts
open Microsoft.FSharp.Reflection
open TypeCache


type PredefinedValues =
  { InlineDefaultValue: string option
    Name: string option
    Definition: string option
    Signature: string option } // Name<Generic,Args>

let d =
  dict
    [ (typeof<string>,
       { InlineDefaultValue = Some "''"
         Name = None
         Definition = None
         Signature = None })
      (typedefof<Option<_>>,
       { InlineDefaultValue = Some "null"
         Definition = Some "T | null"
         Name = None
         Signature = None })
      (typeof<System.Guid>,
       { InlineDefaultValue = Some $"'{(System.Guid.Empty.ToString())}'"
         Name = None
         Definition = None
         Signature = None })
      (typeof<int>,
       { InlineDefaultValue = Some "0"
         Name = None
         Definition = None
         Signature = None })
      (typeof<int64>,
       { InlineDefaultValue = Some "0"
         Name = None
         Definition = None
         Signature = None })
      (typeof<float>,
       { InlineDefaultValue = Some "0.0"
         Name = None
         Definition = None
         Signature = None })
      (typeof<bool>,
       { InlineDefaultValue = Some "false"
         Name = None
         Definition = None
         Signature = None })
      (typeof<obj>,
       { InlineDefaultValue = Some "{}"
         Name = None
         Definition = None
         Signature = None })
      (typeof<unit>,
       { InlineDefaultValue = Some "()"
         Name = None
         Definition = None
         Signature = None })
      (typeof<bigint>,
       { InlineDefaultValue = Some "0"
         Name = None
         Definition = None
         Signature = None })
      (typedefof<System.Collections.Generic.IEnumerable<_>>,
       { InlineDefaultValue = Some "[]"
         Name = None
         Definition = None
         Signature = None }) ]

let defaultTypes =
  System.Collections.Generic.Dictionary<System.Type, PredefinedValues>(d)

let getModuleName (t: System.Type) =
  if t.Namespace <> null then
    t.Namespace.Replace(".", "_")
  else
    failwith "null namespace currently not supported"

let getName (t: System.Type) = t.Name.Split("`").[0]

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

// Module.TypeName
let rec getPropertySignature (callingModule: string) (t: System.Type) =
  let kind = getKind t

  match kind with
  | TypeKind.Array -> getPropertySignature callingModule (typedefof<System.Collections.Generic.IEnumerable<_>>.MakeGenericType (t.GetElementType()))
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

let renderSingleFieldUnionCaseDefinition (callingModule: string) (case: UnionCaseInfo) (fieldInfo: System.Reflection.PropertyInfo) =
  $"""{{ Case: "{case.Name}", Fields: {getPropertySignature callingModule fieldInfo.PropertyType} }}"""

let renderMultiFieldUnionCaseDefinition (callingModule: string) (case: UnionCaseInfo) (fieldInfo: System.Reflection.PropertyInfo list) =
  let fields =
    fieldInfo
    |> List.map (fun v -> v.Name + ": " + getPropertySignature callingModule v.PropertyType)
    |> String.concat ", "

  $"""{{ Case: "{case.Name}", Fields: {{ {fields} }} }}"""

let renderSingleFieldUnionCaseDefaultValue (callingModule: string) (case: UnionCaseInfo) (fieldInfo: System.Reflection.PropertyInfo) =
  let propertyTypeName = fieldInfo.PropertyType.Name

  if fieldInfo.PropertyType.IsGenericParameter then
    $"""({{ Case: "{case.Name}", Fields: default{propertyTypeName} }})"""
  else
    $"""{{ Case: "{case.Name}", Fields: default{propertyTypeName} }}"""

let getDefaultValue (callingModule:string) (propertyType:System.Type)=
  let moduleName = getModuleName propertyType
  let name = getName propertyType
  let isGeneric = propertyType.IsGenericType
  let kind = getKind propertyType
  let prefix = if moduleName = callingModule then "" else moduleName + "."
  let kind = getKind propertyType
  let prefedinedValue =
    match
      defaultTypes.TryGetValue(
        if propertyType.IsGenericType && not propertyType.IsGenericTypeDefinition then
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
      | TypeKind.List -> "[]"
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

let renderMultiFieldUnionCaseDefaultValue (callingModule: string) (case: UnionCaseInfo) (fieldInfo: System.Reflection.PropertyInfo list) =

  let isGeneric =
    fieldInfo |> List.exists (fun v -> v.PropertyType.IsGenericParameter)

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
  | arguments -> "<" + (arguments |> List.map (fun v -> v.Name) |> String.concat ",") + ">"

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
  let isGeneric = fields |> List.exists (fun v -> v.PropertyType.IsGenericParameter)

  let genericParameterPostfix = if isGeneric then failwith "TODO" else ""

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
    cases |> List.map (fun v -> $"\"{v.Name}\"") |> String.concat " | "

  let allCaseNames =
    cases |> List.map (fun v -> $"\"{v.Name}\"") |> String.concat ", "

  let renderedCaseDefinitions =
    match cases with
    | [] -> failwith "todo"
    | [ singleCase ] ->
      let caseFields = singleCase.GetFields() |> Seq.toList
      match caseFields with
      | [] -> failwith "todo"
      | [singleField] ->
        let singleFieldCaseSignature = getSingleFieldCaseSignature name singleField singleCase
        let prop = getPropertySignature callingModule singleField.PropertyType
        $"""export type {singleFieldCaseSignature} = {prop}"""
      | fields -> failwith "todo"
// export var defaultMyRecordId: MyRecordId = System.defaultGuid
    | cases ->
      let renderCase (case: UnionCaseInfo) =

        let caseFields = case.GetFields() |> Seq.toList

        match caseFields with
        | [] -> failwith "todo"
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
      | [singleField] ->
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
        | [] -> failwith "todo"
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
    | [case] -> 
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
    | [] -> failwith "todo"
    | [ singleField ] ->
      let singleFieldCaseSignature = getSingleFieldCaseSignature name singleField case
      singleFieldCaseSignature
    | fields ->
      let singleFieldCaseSignature = getMultiFieldCaseSignature name fields case
      singleFieldCaseSignature
  //$"""export type {name}_Case_{case.Name}<T> = {{ Case: "{case.Name}", Fields: T }} // (multiple fields)"""
  let caseSignatures = cases |> List.map renderCaseSignature |> String.concat " | "

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

let renderRecord (t: System.Type) =
  if t.IsGenericType && not t.IsGenericTypeDefinition then
    failwith "A definition and value for a generic type that is not a generic type definition cannot be rendered"

  let callingModule = getModuleName t
  let name = getName t
  let properties = t.GetProperties()

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
    if t.IsGenericTypeDefinition then
      $"""export var default{name}: {anonymousFunctionSignature} => {namedFunctionSignature} = {anonymousFunctionSignature} => ({{
{fieldValues},
}})"""
    else
      $"""export var default{name}: {name} = {{
{fieldValues},
}}"""

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
export type {name}{genericArguments} = {(predefined.Definition|>Option.defaultValue("unknown"))}
export var default{name}: {anonymousFunctionSignature} => {name}{genericArguments} = {anonymousFunctionSignature} => {predefined.InlineDefaultValue|>Option.defaultValue "unknown"}
"""
  else
    failwith "todo"

let rec renderType (t: System.Type) =
  let kind = getKind t

  let predefinedDefinitionAndValue =
    match
      defaultTypes.TryGetValue(
        if t.IsGenericType && not t.IsGenericTypeDefinition then
          t.GetGenericTypeDefinition()
        else
          t
      )
    with
    | true, value ->
      Some(renderPredefinedTypeFromDefaultValue t value)
    | _ -> None

  if predefinedDefinitionAndValue.IsSome then predefinedDefinitionAndValue.Value
  else
      match kind with
      | TypeKind.List ->
        """export type FSharpList<T> = Array<T>
  export var defaultFSharpList: <T>(t:T) => FSharpList<T> = <T>(t:T) => []
  """

      | TypeKind.Record ->
        if t.IsGenericType && not t.IsGenericTypeDefinition
        then ""
        else renderRecord t
      | TypeKind.Union -> renderDu t
      | TypeKind.Array -> renderType (typeof<System.Collections.Generic.List<_>>.MakeGenericType (t.GetElementType()))
      | TypeKind.Map ->
        //if key type is string, use normal object
        if t.GenericTypeArguments.[0] = typeof<string> then
          """export type FSharpStringMap<TValue> = { [key: string ]: TValue }
  export var defaultFSharpStringMap: <TValue>(t:string,tValue:TValue) => FSharpStringMap<TValue> = <TValue>(t:string,tValue:TValue) => ({})"""
        else
          """export type FSharpMap<TKey, TValue> = [TKey,TValue][]
  export var defaultFSharpMap: <TKey, TValue>(tKey:TKey,tValue:TValue) => FSharpMap<TKey, TValue> = <TKey, TValue>(tKey:TKey,tValue:TValue) => []"""
      | _ ->
        let name = getName t
        let values = t.GetEnumNames() |> Array.map (fun v -> $"\"{v}\"") |> Array.toList

        $"""export type {name} = {values |> String.concat " | "}
  export var {name}_AllValues = [{values |> String.concat ", "}] as const
  export var default{name}: {name} = {values |> List.head}
      """

// export type EntityState = "Detached" | "Unchanged" | "Deleted" | "Modified" | "Added"
// export var EntityState_AllValues = ["Detached", "Unchanged", "Deleted", "Modified", "Added"] as const
// export var defaultEntityState: EntityState = "Detached"
let getDependencies (t: System.Type) =
  let kind = getKind t

  match kind with
  | TypeKind.Record -> t.GetFields() |> Array.map (fun f -> f.FieldType) |> Array.toList
  | _ -> []

type TsModule =
  { Name: string
    Types: System.Type list }

let renderTypes (types: System.Type list) =
  let allTypes = types |> List.collect getDependencies

  let x =
    allTypes
    |> List.groupBy getModuleName
    |> List.map (fun (v, items) -> { Name = v; Types = items })

  ""
