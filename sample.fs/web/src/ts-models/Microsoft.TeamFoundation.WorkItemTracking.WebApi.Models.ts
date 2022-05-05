import { ReferenceLinks } from "./Microsoft.VisualStudio.Services.WebApi"
import { defaultReferenceLinks } from "./Microsoft.VisualStudio.Services.WebApi"
import { SubjectDescriptor } from "./Microsoft.VisualStudio.Services.Common"
import { defaultSubjectDescriptor } from "./Microsoft.VisualStudio.Services.Common"

export type TreeNodeStructureType = "Area" | "Iteration"
export const defaultTreeNodeStructureType = "Area"
export const TreeNodeStructureTypeValues: { [key in TreeNodeStructureType]: TreeNodeStructureType } = {
  Area: "Area",
  Iteration: "Iteration",
}
export const TreeNodeStructureTypeValuesArray: TreeNodeStructureType[] = Object.keys(TreeNodeStructureTypeValues) as TreeNodeStructureType[]

export interface WorkItemClassificationNode {
  id: number
  identifier: string
  name: string | null
  structureType: TreeNodeStructureType
  hasChildren: boolean | null
  children: WorkItemClassificationNode[]
  attributes: { [key: string]: any }
  path: string | null
  links: ReferenceLinks
  url: string | null
}

export const defaultWorkItemClassificationNode: WorkItemClassificationNode = {
  id: 0,
  identifier: "00000000-0000-0000-0000-000000000000",
  name: null,
  structureType: {} as any,
  hasChildren: null,
  children: [],
  attributes: {},
  path: null,
  links: {} as any,
  url: null,
}

export interface WorkItemCommentVersionRef {
  commentId: number
  version: number
  text: string | null
  createdInRevision: number | null
  isDeleted: boolean | null
  url: string | null
}

export const defaultWorkItemCommentVersionRef: WorkItemCommentVersionRef = {
  commentId: 0,
  version: 0,
  text: null,
  createdInRevision: null,
  isDeleted: null,
  url: null,
}

export interface WorkItem {
  id: number | null
  rev: number | null
  fields: { [key: string]: any }
  relations: WorkItemRelation[]
  commentVersionRef: WorkItemCommentVersionRef
  links: ReferenceLinks
  url: string | null
}

export const defaultWorkItem: WorkItem = {
  id: null,
  rev: null,
  fields: {},
  relations: [],
  commentVersionRef: {} as any,
  links: {} as any,
  url: null,
}

export interface WorkItemRelation {
  rel: string | null
  url: string | null
  title: string | null
  attributes: { [key: string]: any }
}

export const defaultWorkItemRelation: WorkItemRelation = {
  rel: null,
  url: null,
  title: null,
  attributes: {},
}

export interface WorkItemIcon {
  id: string | null
  url: string | null
}

export const defaultWorkItemIcon: WorkItemIcon = {
  id: null,
  url: null,
}

export interface WorkItemType {
  name: string | null
  referenceName: string | null
  description: string | null
  color: string | null
  icon: WorkItemIcon
  isDisabled: boolean
  xmlForm: string | null
  fields: WorkItemTypeFieldInstance[]
  fieldInstances: WorkItemTypeFieldInstance[]
  transitions: { [key: string]: WorkItemStateTransition[] }
  states: WorkItemStateColor[]
  links: ReferenceLinks
  url: string | null
}

export const defaultWorkItemType: WorkItemType = {
  name: null,
  referenceName: null,
  description: null,
  color: null,
  icon: {} as any,
  isDisabled: false,
  xmlForm: null,
  fields: [],
  fieldInstances: [],
  transitions: {},
  states: [],
  links: {} as any,
  url: null,
}

export interface WorkItemFieldReference {
  referenceName: string | null
  name: string | null
  url: string | null
}

export const defaultWorkItemFieldReference: WorkItemFieldReference = {
  referenceName: null,
  name: null,
  url: null,
}

export interface WorkItemTypeFieldInstance {
  defaultValue: string | null
  allowedValues: (string | null)[]
  field: WorkItemFieldReference
  helpText: string | null
  alwaysRequired: boolean
  dependentFields: WorkItemFieldReference[]
  isIdentity: boolean
  referenceName: string | null
  name: string | null
  url: string | null
}

export const defaultWorkItemTypeFieldInstance: WorkItemTypeFieldInstance = {
  defaultValue: null,
  allowedValues: [],
  field: {} as any,
  helpText: null,
  alwaysRequired: false,
  dependentFields: [],
  isIdentity: false,
  referenceName: null,
  name: null,
  url: null,
}

export interface WorkItemStateTransition {
  to: string | null
  actions: (string | null)[]
}

export const defaultWorkItemStateTransition: WorkItemStateTransition = {
  to: null,
  actions: [],
}

export interface WorkItemStateColor {
  name: string | null
  color: string | null
  category: string | null
}

export const defaultWorkItemStateColor: WorkItemStateColor = {
  name: null,
  color: null,
  category: null,
}

export interface WorkItemComments {
  totalCount: number
  fromRevisionCount: number
  count: number
  comments: WorkItemComment[]
  links: ReferenceLinks
  url: string | null
}

export const defaultWorkItemComments: WorkItemComments = {
  totalCount: 0,
  fromRevisionCount: 0,
  count: 0,
  comments: [],
  links: {} as any,
  url: null,
}

export interface IdentityReference {
  id: string
  name: string | null
  descriptor: SubjectDescriptor
  displayName: string | null
  url: string | null
  links: ReferenceLinks
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

export const defaultIdentityReference: IdentityReference = {
  id: "00000000-0000-0000-0000-000000000000",
  name: null,
  descriptor: {} as any,
  displayName: null,
  url: null,
  links: {} as any,
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

export interface WorkItemComment {
  revision: number
  text: string | null
  revisedBy: IdentityReference
  revisedDate: string
  links: ReferenceLinks
  url: string | null
}

export const defaultWorkItemComment: WorkItemComment = {
  revision: 0,
  text: null,
  revisedBy: {} as any,
  revisedDate: "1/1/0001 12:00:00 AM",
  links: {} as any,
  url: null,
}

