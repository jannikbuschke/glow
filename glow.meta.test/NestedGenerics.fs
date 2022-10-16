module Test.Generics

open Expecto
open Glow.TsGen.Domain
open Xunit
open Glow.TsGen.Gen

type GenericRecord<'t> = { GenericProperty: 't }

[<Fact>]
let ``Create correct generic TsSignature with runtime argument`` () =

  let def = typeof<GenericRecord<string>>

  let signature =
    Glow.GetTsSignature.getTsSignature def

  Expect.eq
    signature
    { TsSignature.TsName = TsName "GenericRecord"
      TsNamespace = NamespaceName "Test"
      IsGenericType = true
      IsGenericTypeDefinition = false
      IsGenericParameter = false
      ContainsGenericParameters = false
      GenericArgumentTypes =
        [ { IsGenericParameter = false
            TsNamespace = NamespaceName "System"
            IsGenericType = false
            ContainsGenericParameters = false
            IsGenericTypeDefinition = false
            TsName = TsName "String"
            GenericArgumentTypes = [] } ] }


[<Fact>]
let ``Create correct generic type definition TsSignature with generic argument`` () =

  let def = typedefof<GenericRecord<string>>

  let signature =
    Glow.GetTsSignature.getTsSignature def

  Expect.eq
    signature
    { TsSignature.TsName = TsName "GenericRecord"
      TsNamespace = NamespaceName "Test"
      IsGenericType = true
      IsGenericTypeDefinition = true
      ContainsGenericParameters = true
      IsGenericParameter = false
      GenericArgumentTypes =
        [ { TsName = TsName "t"
            TsNamespace = NamespaceName "Test"
            IsGenericParameter = true
            ContainsGenericParameters = true
            IsGenericType = false
            IsGenericTypeDefinition = false
            GenericArgumentTypes = [] } ] }


[<Fact>]
let ``typeof Result<string,string> list`` () =

  let def =
    typeof<Result<string, string> list>

  let signature =
    Glow.GetTsSignature.getTsSignature def

  Expect.eq
    signature
    { TsSignature.TsName = TsName "FSharpList"
      TsNamespace = NamespaceName "Microsoft.FSharp.Collections"
      IsGenericType = true
      IsGenericTypeDefinition = false
      IsGenericParameter = false
      ContainsGenericParameters = false
      GenericArgumentTypes =
        [ { TsNamespace = NamespaceName "Microsoft.FSharp.Core"
            TsName = TsName "FSharpResult"
            IsGenericParameter = false
            IsGenericType = true
            IsGenericTypeDefinition = false
            ContainsGenericParameters = false
            GenericArgumentTypes =
              [ { TsName = TsName "String"
                  TsNamespace = NamespaceName "System"
                  IsGenericParameter = false
                  IsGenericType = false
                  ContainsGenericParameters = false
                  IsGenericTypeDefinition = false
                  GenericArgumentTypes = [] }
                { TsName = TsName "String"
                  TsNamespace = NamespaceName "System"
                  IsGenericParameter = false
                  IsGenericType = false
                  ContainsGenericParameters = false
                  IsGenericTypeDefinition = false
                  GenericArgumentTypes = [] } ] } ] }

[<Fact>]
let ``typedefof Result<string,string> list`` () =

  let def =
    typedefof<Result<string, string> list>

  let signature =
    Glow.GetTsSignature.getTsSignature def

  Expect.eq
    signature
    { TsSignature.TsName = TsName "FSharpList"
      TsNamespace = NamespaceName "Microsoft.FSharp.Collections"
      IsGenericType = true
      IsGenericTypeDefinition = true
      IsGenericParameter = false
      ContainsGenericParameters = true
      GenericArgumentTypes =
        [ { TsName = TsName "T"
            TsNamespace = NamespaceName "Microsoft.FSharp.Collections"
            IsGenericParameter = true
            ContainsGenericParameters = true
            IsGenericType = false
            IsGenericTypeDefinition = false
            GenericArgumentTypes = [] } ] }

type Localizable<'a> =
  { Value: 'a
    Localizations: Map<string, 'a> }

[<Fact>]
let ```not render typedef Localizable<string>`` () =
  // runtime type, should not be rendered (only generic typedefinition)
  let rendered =
    renderTypeAndValue typeof<Localizable<string>>

  Expect.similar rendered ""

[<Fact>]
let ```render typedefof Localizable<string>`` () =
  let rendered =
    renderTypeAndValue typedefof<Localizable<string>>

  Expect.similar
    rendered
    """
export type Localizable<a> = {
  value: a
  localizations: Microsoft_FSharp_Collections.FSharpMap<System.String,a>
}
export const defaultLocalizable: <a>(a:a) => Localizable<a> = <a>(a:a) => ({
 value: a,
 localizations: Microsoft_FSharp_Collections.defaultFSharpMap(System.defaultString,a),
})
"""

[<Fact>]
let ``typedefof Localizable<'a>`` () =

  let def = typedefof<Localizable<_>>

  let signature =
    Glow.GetTsSignature.getTsSignature def

  Expect.eq
    signature
    { TsSignature.TsName = TsName "Localizable"
      TsNamespace = NamespaceName "Test"
      IsGenericType = true
      IsGenericTypeDefinition = true
      IsGenericParameter = false
      ContainsGenericParameters = true
      GenericArgumentTypes =
        [ { TsName = TsName "a"
            TsNamespace = NamespaceName "Test"
            IsGenericParameter = true
            ContainsGenericParameters = true
            IsGenericType = false
            IsGenericTypeDefinition = false
            GenericArgumentTypes = [] } ] }

[<Fact>]
let ``typeof Localizable<'a>`` () =

  let def = typeof<Localizable<string>>

  let signature =
    Glow.GetTsSignature.getTsSignature def

  Expect.eq
    signature
    { TsSignature.TsName = TsName "Localizable"
      TsNamespace = NamespaceName "Test"
      IsGenericType = true
      IsGenericTypeDefinition = false
      IsGenericParameter = false
      ContainsGenericParameters = false
      GenericArgumentTypes =
        [ { TsName = TsName "String"
            TsNamespace = NamespaceName "System"
            IsGenericParameter = false
            IsGenericType = false
            ContainsGenericParameters = false
            IsGenericTypeDefinition = false
            GenericArgumentTypes = [] } ] }

type Localizable2<'a> =
  { Value: 'a
    Localizations: Map<string, 'a> }

[<Fact>]
let ``Properties of typeof Localizable2<'a>`` () =

  let def = typeof<Localizable2<string>>

  let signature =
    ((Glow.GetTsSignature.getProperties def)
     |> Seq.map (fun v -> v.TsType)
     |> Seq.toList)

  Expect.eq
    signature.Head
    { TsSignature.TsName = TsName "String"
      TsNamespace = NamespaceName "System"
      IsGenericType = false
      IsGenericTypeDefinition = false
      IsGenericParameter = false
      ContainsGenericParameters = false
      GenericArgumentTypes = [] }

  Expect.eq
    signature.Tail.Head
    { TsSignature.TsName = TsName "FSharpMap"
      TsNamespace = NamespaceName "Microsoft.FSharp.Collections"
      IsGenericType = true
      IsGenericTypeDefinition = false
      IsGenericParameter = false
      ContainsGenericParameters = false
      GenericArgumentTypes =
        [ { TsSignature.TsName = TsName "String"
            TsNamespace = NamespaceName "System"
            IsGenericType = false
            IsGenericTypeDefinition = false
            IsGenericParameter = false
            ContainsGenericParameters = false
            GenericArgumentTypes = [] }
          { TsSignature.TsName = TsName "String"
            TsNamespace = NamespaceName "System"
            IsGenericType = false
            IsGenericTypeDefinition = false
            ContainsGenericParameters = false
            IsGenericParameter = false
            GenericArgumentTypes = [] }

          ] }

[<Fact>]
let ``Properties of typedefof Localizable2<'a>`` () =

  let def = typedefof<Localizable2<string>>

  let signature =
    ((Glow.GetTsSignature.getProperties def)
     |> Seq.map (fun v -> v.TsType)
     |> Seq.toList)

  Expect.eq
    signature.Head
    { TsSignature.TsName = TsName "a"
      TsNamespace = NamespaceName "Test"
      IsGenericType = false
      IsGenericTypeDefinition = false
      IsGenericParameter = true
      ContainsGenericParameters = true
      GenericArgumentTypes = [] }

  Expect.eq
    signature.Tail.Head
    { TsSignature.TsName = TsName "FSharpMap"
      TsNamespace = NamespaceName "Microsoft.FSharp.Collections"
      IsGenericType = true
      IsGenericTypeDefinition = false
      ContainsGenericParameters = true
      IsGenericParameter = false
      GenericArgumentTypes =
        [ { TsSignature.TsName = TsName "String"
            TsNamespace = NamespaceName "System"
            IsGenericType = false
            IsGenericTypeDefinition = false
            IsGenericParameter = false
            ContainsGenericParameters = false
            GenericArgumentTypes = [] }
          { TsSignature.TsName = TsName "a"
            TsNamespace = NamespaceName "Test"
            IsGenericType = false
            IsGenericTypeDefinition = false
            IsGenericParameter = true
            ContainsGenericParameters = true
            GenericArgumentTypes = [] }

          ] }

[<Fact>]
let ``Properties of typeof <System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<_,_>>>`` () =

  let modules =
    Glow.TsGen.Gen.generateModules [ typeof<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<_, _>>> ]

  let enumerableSignature =
    Glow.GetTsSignature.getTsSignature typedefof<System.Collections.Generic.IEnumerable<_>>

  let keyValueSignature =
    Glow.GetTsSignature.getTsSignature typedefof<System.Collections.Generic.KeyValuePair<_, _>>

  let hasItemWithSignature signature m =
    m.Items
    |> List.exists (fun i -> i.Id.TsSignature = signature)

  "Should containt IEnumerable definition"
  |> Expect.exists modules (hasItemWithSignature enumerableSignature)

  "Should containt KeyValuePair definition"
  |> Expect.exists modules (hasItemWithSignature keyValueSignature)
