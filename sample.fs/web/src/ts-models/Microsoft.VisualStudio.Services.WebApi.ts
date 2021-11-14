/* eslint-disable prettier/prettier */
import { SubjectDescriptor } from "./Microsoft.VisualStudio.Services.Common"
import { defaultSubjectDescriptor } from "./Microsoft.VisualStudio.Services.Common"

export interface ReferenceLinks {
  links: (string | null)[]
}

export const defaultReferenceLinks: ReferenceLinks = {
  links: [],
}

export interface IdentityRef {
  descriptor: SubjectDescriptor
  displayName: string | null
  url: string | null
  links: ReferenceLinks
  id: string | null
  uniqueName: string | null
  directoryAlias: string | null
  profileUrl: string | null
  imageUrl: string | null
  isContainer: boolean
  isAadIdentity: boolean
  inactive: boolean
  isDeletedInOrigin: boolean
  displayNameForXmlSerialization: string | null
  urlForXmlSerialization: string | null
}

export const defaultIdentityRef: IdentityRef = {
  descriptor: {} as any,
  displayName: null,
  url: null,
  links: {} as any,
  id: null,
  uniqueName: null,
  directoryAlias: null,
  profileUrl: null,
  imageUrl: null,
  isContainer: false,
  isAadIdentity: false,
  inactive: false,
  isDeletedInOrigin: false,
  displayNameForXmlSerialization: null,
  urlForXmlSerialization: null,
}

export interface ResourceRef {
  id: string | null
  url: string | null
}

export const defaultResourceRef: ResourceRef = {
  id: null,
  url: null,
}

