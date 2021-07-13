import { Type, RuntimeMethodHandle, RuntimeTypeHandle, ModuleHandle, RuntimeFieldHandle } from "./System"
import { defaultType, defaultRuntimeMethodHandle, defaultRuntimeTypeHandle, defaultModuleHandle, defaultRuntimeFieldHandle } from "./System"
import { SecurityRuleSet } from "./System.Security"
import { defaultSecurityRuleSet } from "./System.Security"
import { StructLayoutAttribute } from "./System.Runtime.InteropServices"
import { defaultStructLayoutAttribute } from "./System.Runtime.InteropServices"

export type MemberTypes = "Constructor" | "Event" | "Field" | "Method" | "Property" | "TypeInfo" | "Custom" | "NestedType" | "All"
export const defaultMemberTypes = "Constructor"

export type ParameterAttributes = "None" | "In" | "Out" | "Lcid" | "Retval" | "Optional" | "HasDefault" | "HasFieldMarshal" | "Reserved3" | "Reserved4" | "ReservedMask"
export const defaultParameterAttributes = "None"

export type MethodAttributes = "ReuseSlot" | "ReuseSlot" | "Private" | "FamANDAssem" | "Assembly" | "Family" | "FamORAssem" | "Public" | "MemberAccessMask" | "UnmanagedExport" | "Static" | "Final" | "Virtual" | "HideBySig" | "NewSlot" | "NewSlot" | "CheckAccessOnOverride" | "Abstract" | "SpecialName" | "RTSpecialName" | "PinvokeImpl" | "HasSecurity" | "RequireSecObject" | "ReservedMask"
export const defaultMethodAttributes = "ReuseSlot"

export type MethodImplAttributes = "Managed" | "Managed" | "Native" | "OPTIL" | "CodeTypeMask" | "CodeTypeMask" | "Unmanaged" | "Unmanaged" | "NoInlining" | "ForwardRef" | "Synchronized" | "NoOptimization" | "PreserveSig" | "AggressiveInlining" | "AggressiveOptimization" | "InternalCall" | "MaxMethodImplVal"
export const defaultMethodImplAttributes = "Managed"

export type CallingConventions = "Standard" | "VarArgs" | "Any" | "HasThis" | "ExplicitThis"
export const defaultCallingConventions = "Standard"

export type EventAttributes = "None" | "SpecialName" | "RTSpecialName" | "RTSpecialName"
export const defaultEventAttributes = "None"

export type FieldAttributes = "PrivateScope" | "Private" | "FamANDAssem" | "Assembly" | "Family" | "FamORAssem" | "Public" | "FieldAccessMask" | "Static" | "InitOnly" | "Literal" | "NotSerialized" | "HasFieldRVA" | "SpecialName" | "RTSpecialName" | "HasFieldMarshal" | "PinvokeImpl" | "HasDefault" | "ReservedMask"
export const defaultFieldAttributes = "PrivateScope"

export type PropertyAttributes = "None" | "SpecialName" | "RTSpecialName" | "HasDefault" | "Reserved2" | "Reserved3" | "Reserved4" | "ReservedMask"
export const defaultPropertyAttributes = "None"

export type GenericParameterAttributes = "None" | "Covariant" | "Contravariant" | "VarianceMask" | "ReferenceTypeConstraint" | "NotNullableValueTypeConstraint" | "DefaultConstructorConstraint" | "SpecialConstraintMask"
export const defaultGenericParameterAttributes = "None"

export type TypeAttributes = "Class" | "Class" | "Class" | "Class" | "Public" | "NestedPublic" | "NestedPrivate" | "NestedFamily" | "NestedAssembly" | "NestedFamANDAssem" | "VisibilityMask" | "VisibilityMask" | "SequentialLayout" | "ExplicitLayout" | "LayoutMask" | "Interface" | "Interface" | "Abstract" | "Sealed" | "SpecialName" | "RTSpecialName" | "Import" | "Serializable" | "WindowsRuntime" | "UnicodeClass" | "AutoClass" | "StringFormatMask" | "StringFormatMask" | "HasSecurity" | "ReservedMask" | "BeforeFieldInit" | "CustomFormatMask"
export const defaultTypeAttributes = "Class"

export interface ICustomAttributeProvider {
}

export const defaultICustomAttributeProvider: ICustomAttributeProvider = {
}

export interface MethodInfo {
  memberType: MemberTypes
  returnParameter: ParameterInfo
  returnType: Type
  returnTypeCustomAttributes: ICustomAttributeProvider
  attributes: MethodAttributes
  methodImplementationFlags: MethodImplAttributes
  callingConvention: CallingConventions
  isAbstract: boolean
  isConstructor: boolean
  isFinal: boolean
  isHideBySig: boolean
  isSpecialName: boolean
  isStatic: boolean
  isVirtual: boolean
  isAssembly: boolean
  isFamily: boolean
  isFamilyAndAssembly: boolean
  isFamilyOrAssembly: boolean
  isPrivate: boolean
  isPublic: boolean
  isConstructedGenericMethod: boolean
  isGenericMethod: boolean
  isGenericMethodDefinition: boolean
  containsGenericParameters: boolean
  methodHandle: RuntimeMethodHandle
  isSecurityCritical: boolean
  isSecuritySafeCritical: boolean
  isSecurityTransparent: boolean
  name: string | null
  declaringType: Type
  reflectedType: Type
  module: Module
  customAttributes: CustomAttributeData[]
  isCollectible: boolean
  metadataToken: number
}

export const defaultMethodInfo: MethodInfo = {
  memberType: {} as any,
  returnParameter: {} as any,
  returnType: {} as any,
  returnTypeCustomAttributes: {} as any,
  attributes: {} as any,
  methodImplementationFlags: {} as any,
  callingConvention: {} as any,
  isAbstract: false,
  isConstructor: false,
  isFinal: false,
  isHideBySig: false,
  isSpecialName: false,
  isStatic: false,
  isVirtual: false,
  isAssembly: false,
  isFamily: false,
  isFamilyAndAssembly: false,
  isFamilyOrAssembly: false,
  isPrivate: false,
  isPublic: false,
  isConstructedGenericMethod: false,
  isGenericMethod: false,
  isGenericMethodDefinition: false,
  containsGenericParameters: false,
  methodHandle: {} as any,
  isSecurityCritical: false,
  isSecuritySafeCritical: false,
  isSecurityTransparent: false,
  name: null,
  declaringType: {} as any,
  reflectedType: {} as any,
  module: {} as any,
  customAttributes: [],
  isCollectible: false,
  metadataToken: 0,
}

export interface ParameterInfo {
  attributes: ParameterAttributes
  member: MemberInfo
  name: string | null
  parameterType: Type
  position: number
  isIn: boolean
  isLcid: boolean
  isOptional: boolean
  isOut: boolean
  isRetval: boolean
  defaultValue: any
  rawDefaultValue: any
  hasDefaultValue: boolean
  customAttributes: CustomAttributeData[]
  metadataToken: number
}

export const defaultParameterInfo: ParameterInfo = {
  attributes: {} as any,
  member: {} as any,
  name: null,
  parameterType: {} as any,
  position: 0,
  isIn: false,
  isLcid: false,
  isOptional: false,
  isOut: false,
  isRetval: false,
  defaultValue: null,
  rawDefaultValue: null,
  hasDefaultValue: false,
  customAttributes: [],
  metadataToken: 0,
}

export interface MemberInfo {
  memberType: MemberTypes
  name: string | null
  declaringType: Type
  reflectedType: Type
  module: Module
  customAttributes: CustomAttributeData[]
  isCollectible: boolean
  metadataToken: number
}

export const defaultMemberInfo: MemberInfo = {
  memberType: {} as any,
  name: null,
  declaringType: {} as any,
  reflectedType: {} as any,
  module: {} as any,
  customAttributes: [],
  isCollectible: false,
  metadataToken: 0,
}

export interface Assembly {
  definedTypes: TypeInfo[]
  exportedTypes: Type[]
  codeBase: string | null
  entryPoint: MethodInfo
  fullName: string | null
  imageRuntimeVersion: string | null
  isDynamic: boolean
  location: string | null
  reflectionOnly: boolean
  isCollectible: boolean
  isFullyTrusted: boolean
  customAttributes: CustomAttributeData[]
  escapedCodeBase: string | null
  manifestModule: Module
  modules: Module[]
  globalAssemblyCache: boolean
  hostContext: number
  securityRuleSet: SecurityRuleSet
}

export const defaultAssembly: Assembly = {
  definedTypes: [],
  exportedTypes: [],
  codeBase: null,
  entryPoint: {} as any,
  fullName: null,
  imageRuntimeVersion: null,
  isDynamic: false,
  location: null,
  reflectionOnly: false,
  isCollectible: false,
  isFullyTrusted: false,
  customAttributes: [],
  escapedCodeBase: null,
  manifestModule: {} as any,
  modules: [],
  globalAssemblyCache: false,
  hostContext: 0,
  securityRuleSet: {} as any,
}

export interface TypeInfo {
  genericTypeParameters: Type[]
  declaredConstructors: ConstructorInfo[]
  declaredEvents: EventInfo[]
  declaredFields: FieldInfo[]
  declaredMembers: MemberInfo[]
  declaredMethods: MethodInfo[]
  declaredNestedTypes: TypeInfo[]
  declaredProperties: PropertyInfo[]
  implementedInterfaces: Type[]
  isInterface: boolean
  memberType: MemberTypes
  namespace: string | null
  assemblyQualifiedName: string | null
  fullName: string | null
  assembly: Assembly
  module: Module
  isNested: boolean
  declaringType: Type
  declaringMethod: MethodBase
  reflectedType: Type
  underlyingSystemType: Type
  isTypeDefinition: boolean
  isArray: boolean
  isByRef: boolean
  isPointer: boolean
  isConstructedGenericType: boolean
  isGenericParameter: boolean
  isGenericTypeParameter: boolean
  isGenericMethodParameter: boolean
  isGenericType: boolean
  isGenericTypeDefinition: boolean
  isSZArray: boolean
  isVariableBoundArray: boolean
  isByRefLike: boolean
  hasElementType: boolean
  genericTypeArguments: Type[]
  genericParameterPosition: number
  genericParameterAttributes: GenericParameterAttributes
  attributes: TypeAttributes
  isAbstract: boolean
  isImport: boolean
  isSealed: boolean
  isSpecialName: boolean
  isClass: boolean
  isNestedAssembly: boolean
  isNestedFamANDAssem: boolean
  isNestedFamily: boolean
  isNestedFamORAssem: boolean
  isNestedPrivate: boolean
  isNestedPublic: boolean
  isNotPublic: boolean
  isPublic: boolean
  isAutoLayout: boolean
  isExplicitLayout: boolean
  isLayoutSequential: boolean
  isAnsiClass: boolean
  isAutoClass: boolean
  isUnicodeClass: boolean
  isCOMObject: boolean
  isContextful: boolean
  isEnum: boolean
  isMarshalByRef: boolean
  isPrimitive: boolean
  isValueType: boolean
  isSignatureType: boolean
  isSecurityCritical: boolean
  isSecuritySafeCritical: boolean
  isSecurityTransparent: boolean
  structLayoutAttribute: StructLayoutAttribute
  typeInitializer: ConstructorInfo
  typeHandle: RuntimeTypeHandle
  gUID: string
  baseType: Type
  isSerializable: boolean
  containsGenericParameters: boolean
  isVisible: boolean
  name: string | null
  customAttributes: CustomAttributeData[]
  isCollectible: boolean
  metadataToken: number
}

export const defaultTypeInfo: TypeInfo = {
  genericTypeParameters: [],
  declaredConstructors: [],
  declaredEvents: [],
  declaredFields: [],
  declaredMembers: [],
  declaredMethods: [],
  declaredNestedTypes: [],
  declaredProperties: [],
  implementedInterfaces: [],
  isInterface: false,
  memberType: {} as any,
  namespace: null,
  assemblyQualifiedName: null,
  fullName: null,
  assembly: {} as any,
  module: {} as any,
  isNested: false,
  declaringType: {} as any,
  declaringMethod: {} as any,
  reflectedType: {} as any,
  underlyingSystemType: {} as any,
  isTypeDefinition: false,
  isArray: false,
  isByRef: false,
  isPointer: false,
  isConstructedGenericType: false,
  isGenericParameter: false,
  isGenericTypeParameter: false,
  isGenericMethodParameter: false,
  isGenericType: false,
  isGenericTypeDefinition: false,
  isSZArray: false,
  isVariableBoundArray: false,
  isByRefLike: false,
  hasElementType: false,
  genericTypeArguments: [],
  genericParameterPosition: 0,
  genericParameterAttributes: {} as any,
  attributes: {} as any,
  isAbstract: false,
  isImport: false,
  isSealed: false,
  isSpecialName: false,
  isClass: false,
  isNestedAssembly: false,
  isNestedFamANDAssem: false,
  isNestedFamily: false,
  isNestedFamORAssem: false,
  isNestedPrivate: false,
  isNestedPublic: false,
  isNotPublic: false,
  isPublic: false,
  isAutoLayout: false,
  isExplicitLayout: false,
  isLayoutSequential: false,
  isAnsiClass: false,
  isAutoClass: false,
  isUnicodeClass: false,
  isCOMObject: false,
  isContextful: false,
  isEnum: false,
  isMarshalByRef: false,
  isPrimitive: false,
  isValueType: false,
  isSignatureType: false,
  isSecurityCritical: false,
  isSecuritySafeCritical: false,
  isSecurityTransparent: false,
  structLayoutAttribute: {} as any,
  typeInitializer: {} as any,
  typeHandle: {} as any,
  gUID: "00000000-0000-0000-0000-000000000000",
  baseType: {} as any,
  isSerializable: false,
  containsGenericParameters: false,
  isVisible: false,
  name: null,
  customAttributes: [],
  isCollectible: false,
  metadataToken: 0,
}

export interface ConstructorInfo {
  memberType: MemberTypes
  attributes: MethodAttributes
  methodImplementationFlags: MethodImplAttributes
  callingConvention: CallingConventions
  isAbstract: boolean
  isConstructor: boolean
  isFinal: boolean
  isHideBySig: boolean
  isSpecialName: boolean
  isStatic: boolean
  isVirtual: boolean
  isAssembly: boolean
  isFamily: boolean
  isFamilyAndAssembly: boolean
  isFamilyOrAssembly: boolean
  isPrivate: boolean
  isPublic: boolean
  isConstructedGenericMethod: boolean
  isGenericMethod: boolean
  isGenericMethodDefinition: boolean
  containsGenericParameters: boolean
  methodHandle: RuntimeMethodHandle
  isSecurityCritical: boolean
  isSecuritySafeCritical: boolean
  isSecurityTransparent: boolean
  name: string | null
  declaringType: Type
  reflectedType: Type
  module: Module
  customAttributes: CustomAttributeData[]
  isCollectible: boolean
  metadataToken: number
}

export const defaultConstructorInfo: ConstructorInfo = {
  memberType: {} as any,
  attributes: {} as any,
  methodImplementationFlags: {} as any,
  callingConvention: {} as any,
  isAbstract: false,
  isConstructor: false,
  isFinal: false,
  isHideBySig: false,
  isSpecialName: false,
  isStatic: false,
  isVirtual: false,
  isAssembly: false,
  isFamily: false,
  isFamilyAndAssembly: false,
  isFamilyOrAssembly: false,
  isPrivate: false,
  isPublic: false,
  isConstructedGenericMethod: false,
  isGenericMethod: false,
  isGenericMethodDefinition: false,
  containsGenericParameters: false,
  methodHandle: {} as any,
  isSecurityCritical: false,
  isSecuritySafeCritical: false,
  isSecurityTransparent: false,
  name: null,
  declaringType: {} as any,
  reflectedType: {} as any,
  module: {} as any,
  customAttributes: [],
  isCollectible: false,
  metadataToken: 0,
}

export interface Module {
  assembly: Assembly
  fullyQualifiedName: string | null
  name: string | null
  mDStreamVersion: number
  moduleVersionId: string
  scopeName: string | null
  moduleHandle: ModuleHandle
  customAttributes: CustomAttributeData[]
  metadataToken: number
}

export const defaultModule: Module = {
  assembly: {} as any,
  fullyQualifiedName: null,
  name: null,
  mDStreamVersion: 0,
  moduleVersionId: "00000000-0000-0000-0000-000000000000",
  scopeName: null,
  moduleHandle: {} as any,
  customAttributes: [],
  metadataToken: 0,
}

export interface CustomAttributeData {
  attributeType: Type
  constructor: ConstructorInfo
  constructorArguments: CustomAttributeTypedArgument[]
  namedArguments: CustomAttributeNamedArgument[]
}

export const defaultCustomAttributeData: CustomAttributeData = {
  attributeType: {} as any,
  constructor: {} as any,
  constructorArguments: [],
  namedArguments: [],
}

export interface CustomAttributeTypedArgument {
  argumentType: Type
  value: any
}

export const defaultCustomAttributeTypedArgument: CustomAttributeTypedArgument = {
  argumentType: {} as any,
  value: null,
}

export interface CustomAttributeNamedArgument {
  memberInfo: MemberInfo
  typedValue: CustomAttributeTypedArgument
  memberName: string | null
  isField: boolean
}

export const defaultCustomAttributeNamedArgument: CustomAttributeNamedArgument = {
  memberInfo: {} as any,
  typedValue: {} as any,
  memberName: null,
  isField: false,
}

export interface EventInfo {
  memberType: MemberTypes
  attributes: EventAttributes
  isSpecialName: boolean
  addMethod: MethodInfo
  removeMethod: MethodInfo
  raiseMethod: MethodInfo
  isMulticast: boolean
  eventHandlerType: Type
  name: string | null
  declaringType: Type
  reflectedType: Type
  module: Module
  customAttributes: CustomAttributeData[]
  isCollectible: boolean
  metadataToken: number
}

export const defaultEventInfo: EventInfo = {
  memberType: {} as any,
  attributes: {} as any,
  isSpecialName: false,
  addMethod: {} as any,
  removeMethod: {} as any,
  raiseMethod: {} as any,
  isMulticast: false,
  eventHandlerType: {} as any,
  name: null,
  declaringType: {} as any,
  reflectedType: {} as any,
  module: {} as any,
  customAttributes: [],
  isCollectible: false,
  metadataToken: 0,
}

export interface FieldInfo {
  memberType: MemberTypes
  attributes: FieldAttributes
  fieldType: Type
  isInitOnly: boolean
  isLiteral: boolean
  isNotSerialized: boolean
  isPinvokeImpl: boolean
  isSpecialName: boolean
  isStatic: boolean
  isAssembly: boolean
  isFamily: boolean
  isFamilyAndAssembly: boolean
  isFamilyOrAssembly: boolean
  isPrivate: boolean
  isPublic: boolean
  isSecurityCritical: boolean
  isSecuritySafeCritical: boolean
  isSecurityTransparent: boolean
  fieldHandle: RuntimeFieldHandle
  name: string | null
  declaringType: Type
  reflectedType: Type
  module: Module
  customAttributes: CustomAttributeData[]
  isCollectible: boolean
  metadataToken: number
}

export const defaultFieldInfo: FieldInfo = {
  memberType: {} as any,
  attributes: {} as any,
  fieldType: {} as any,
  isInitOnly: false,
  isLiteral: false,
  isNotSerialized: false,
  isPinvokeImpl: false,
  isSpecialName: false,
  isStatic: false,
  isAssembly: false,
  isFamily: false,
  isFamilyAndAssembly: false,
  isFamilyOrAssembly: false,
  isPrivate: false,
  isPublic: false,
  isSecurityCritical: false,
  isSecuritySafeCritical: false,
  isSecurityTransparent: false,
  fieldHandle: {} as any,
  name: null,
  declaringType: {} as any,
  reflectedType: {} as any,
  module: {} as any,
  customAttributes: [],
  isCollectible: false,
  metadataToken: 0,
}

export interface PropertyInfo {
  memberType: MemberTypes
  propertyType: Type
  attributes: PropertyAttributes
  isSpecialName: boolean
  canRead: boolean
  canWrite: boolean
  getMethod: MethodInfo
  setMethod: MethodInfo
  name: string | null
  declaringType: Type
  reflectedType: Type
  module: Module
  customAttributes: CustomAttributeData[]
  isCollectible: boolean
  metadataToken: number
}

export const defaultPropertyInfo: PropertyInfo = {
  memberType: {} as any,
  propertyType: {} as any,
  attributes: {} as any,
  isSpecialName: false,
  canRead: false,
  canWrite: false,
  getMethod: {} as any,
  setMethod: {} as any,
  name: null,
  declaringType: {} as any,
  reflectedType: {} as any,
  module: {} as any,
  customAttributes: [],
  isCollectible: false,
  metadataToken: 0,
}

export interface MethodBase {
  attributes: MethodAttributes
  methodImplementationFlags: MethodImplAttributes
  callingConvention: CallingConventions
  isAbstract: boolean
  isConstructor: boolean
  isFinal: boolean
  isHideBySig: boolean
  isSpecialName: boolean
  isStatic: boolean
  isVirtual: boolean
  isAssembly: boolean
  isFamily: boolean
  isFamilyAndAssembly: boolean
  isFamilyOrAssembly: boolean
  isPrivate: boolean
  isPublic: boolean
  isConstructedGenericMethod: boolean
  isGenericMethod: boolean
  isGenericMethodDefinition: boolean
  containsGenericParameters: boolean
  methodHandle: RuntimeMethodHandle
  isSecurityCritical: boolean
  isSecuritySafeCritical: boolean
  isSecurityTransparent: boolean
  memberType: MemberTypes
  name: string | null
  declaringType: Type
  reflectedType: Type
  module: Module
  customAttributes: CustomAttributeData[]
  isCollectible: boolean
  metadataToken: number
}

export const defaultMethodBase: MethodBase = {
  attributes: {} as any,
  methodImplementationFlags: {} as any,
  callingConvention: {} as any,
  isAbstract: false,
  isConstructor: false,
  isFinal: false,
  isHideBySig: false,
  isSpecialName: false,
  isStatic: false,
  isVirtual: false,
  isAssembly: false,
  isFamily: false,
  isFamilyAndAssembly: false,
  isFamilyOrAssembly: false,
  isPrivate: false,
  isPublic: false,
  isConstructedGenericMethod: false,
  isGenericMethod: false,
  isGenericMethodDefinition: false,
  containsGenericParameters: false,
  methodHandle: {} as any,
  isSecurityCritical: false,
  isSecuritySafeCritical: false,
  isSecurityTransparent: false,
  memberType: {} as any,
  name: null,
  declaringType: {} as any,
  reflectedType: {} as any,
  module: {} as any,
  customAttributes: [],
  isCollectible: false,
  metadataToken: 0,
}

