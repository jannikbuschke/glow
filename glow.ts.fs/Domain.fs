module Glow.TsGen.Domain

type ModuleName = ModuleName of string
type NamespaceName = NamespaceName of string
type TsTypeId = TsTypeId of string
type TsNamespace = TsNamespace of string
type TsName = TsName of string

type GenericArguments = string list
type DuCaseSignature = DuCaseSignature of string

module DuCaseSignature =
  let value (DuCaseSignature s) = s