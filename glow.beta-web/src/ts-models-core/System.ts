import { MethodInfo, MemberTypes, Assembly, Module, MethodBase, GenericParameterAttributes, TypeAttributes, ConstructorInfo, CustomAttributeData, ParameterInfo, ICustomAttributeProvider, MethodAttributes, MethodImplAttributes, CallingConventions, TypeInfo } from "./System.Reflection"
import { defaultMethodInfo, defaultMemberTypes, defaultAssembly, defaultModule, defaultMethodBase, defaultGenericParameterAttributes, defaultTypeAttributes, defaultConstructorInfo, defaultCustomAttributeData, defaultParameterInfo, defaultICustomAttributeProvider, defaultMethodAttributes, defaultMethodImplAttributes, defaultCallingConventions, defaultTypeInfo } from "./System.Reflection"
import { StructLayoutAttribute } from "./System.Runtime.InteropServices"
import { defaultStructLayoutAttribute } from "./System.Runtime.InteropServices"
import { SecurityRuleSet } from "./System.Security"
import { defaultSecurityRuleSet } from "./System.Security"

export interface IntPtr {
}

export const defaultIntPtr: IntPtr = {
}

export interface RuntimeMethodHandle {
  value: IntPtr
}

export const defaultRuntimeMethodHandle: RuntimeMethodHandle = {
  value: {} as any,
}

export interface ModuleHandle {
  mDStreamVersion: number
}

export const defaultModuleHandle: ModuleHandle = {
  mDStreamVersion: 0,
}

export interface RuntimeFieldHandle {
  value: IntPtr
}

export const defaultRuntimeFieldHandle: RuntimeFieldHandle = {
  value: {} as any,
}

export interface RuntimeTypeHandle {
  value: IntPtr
}

export const defaultRuntimeTypeHandle: RuntimeTypeHandle = {
  value: {} as any,
}

export interface Func1Task {
  target: any
  method: MethodInfo
}


export interface Type {
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

export const defaultType: Type = {
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

