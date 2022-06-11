import { IdentityRef } from "./Microsoft.VisualStudio.Services.WebApi"
import { defaultIdentityRef } from "./Microsoft.VisualStudio.Services.WebApi"

export interface VariableGroupProviderData {
}

export const defaultVariableGroupProviderData: VariableGroupProviderData = {
}

export interface VariableGroup {
  id: number
  type: string | null
  name: string | null
  description: string | null
  providerData: VariableGroupProviderData
  createdBy: IdentityRef
  createdOn: string
  modifiedBy: IdentityRef
  modifiedOn: string
  isShared: boolean
  variables: { [key: string]: VariableValue }
}

export const defaultVariableGroup: VariableGroup = {
  id: 0,
  type: null,
  name: null,
  description: null,
  providerData: {} as any,
  createdBy: {} as any,
  createdOn: "1/1/0001 12:00:00 AM",
  modifiedBy: {} as any,
  modifiedOn: "1/1/0001 12:00:00 AM",
  isShared: false,
  variables: {},
}

export interface VariableValue {
  value: string | null
  isSecret: boolean
}

export const defaultVariableValue: VariableValue = {
  value: null,
  isSecret: false,
}

