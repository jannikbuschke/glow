import { IdentityRef, ReferenceLinks, ResourceRef } from "./Microsoft.VisualStudio.Services.WebApi"
import { defaultIdentityRef, defaultReferenceLinks, defaultResourceRef } from "./Microsoft.VisualStudio.Services.WebApi"
import { TeamProjectReference, TeamProjectCollectionReference } from "./Microsoft.TeamFoundation.Core.WebApi"
import { defaultTeamProjectReference, defaultTeamProjectCollectionReference } from "./Microsoft.TeamFoundation.Core.WebApi"

export type GitObjectType = "Bad" | "Commit" | "Tree" | "Blob" | "Tag" | "Ext2" | "OfsDelta" | "RefDelta"
export const defaultGitObjectType = "Bad"
export const GitObjectTypeValues: { [key in GitObjectType]: GitObjectType } = {
  Bad: "Bad",
  Commit: "Commit",
  Tree: "Tree",
  Blob: "Blob",
  Tag: "Tag",
  Ext2: "Ext2",
  OfsDelta: "OfsDelta",
  RefDelta: "RefDelta",
}
export const GitObjectTypeValuesArray: GitObjectType[] = Object.keys(GitObjectTypeValues) as GitObjectType[]

export type VersionControlChangeType = "None" | "Add" | "Edit" | "Encoding" | "Rename" | "Delete" | "Undelete" | "Branch" | "Merge" | "Lock" | "Rollback" | "SourceRename" | "TargetRename" | "Property" | "All"
export const defaultVersionControlChangeType = "None"
export const VersionControlChangeTypeValues: { [key in VersionControlChangeType]: VersionControlChangeType } = {
  None: "None",
  Add: "Add",
  Edit: "Edit",
  Encoding: "Encoding",
  Rename: "Rename",
  Delete: "Delete",
  Undelete: "Undelete",
  Branch: "Branch",
  Merge: "Merge",
  Lock: "Lock",
  Rollback: "Rollback",
  SourceRename: "SourceRename",
  TargetRename: "TargetRename",
  Property: "Property",
  All: "All",
}
export const VersionControlChangeTypeValuesArray: VersionControlChangeType[] = Object.keys(VersionControlChangeTypeValues) as VersionControlChangeType[]

export type ItemContentType = "RawText" | "Base64Encoded"
export const defaultItemContentType = "RawText"
export const ItemContentTypeValues: { [key in ItemContentType]: ItemContentType } = {
  RawText: "RawText",
  Base64Encoded: "Base64Encoded",
}
export const ItemContentTypeValuesArray: ItemContentType[] = Object.keys(ItemContentTypeValues) as ItemContentType[]

export type GitStatusState = "NotSet" | "Pending" | "Succeeded" | "Failed" | "Error" | "NotApplicable"
export const defaultGitStatusState = "NotSet"
export const GitStatusStateValues: { [key in GitStatusState]: GitStatusState } = {
  NotSet: "NotSet",
  Pending: "Pending",
  Succeeded: "Succeeded",
  Failed: "Failed",
  Error: "Error",
  NotApplicable: "NotApplicable",
}
export const GitStatusStateValuesArray: GitStatusState[] = Object.keys(GitStatusStateValues) as GitStatusState[]

export interface GitRepositoryRef {
  id: string
  name: string | null
  isFork: boolean
  url: string | null
  remoteUrl: string | null
  sshUrl: string | null
  projectReference: TeamProjectReference
  collection: TeamProjectCollectionReference
}

export const defaultGitRepositoryRef: GitRepositoryRef = {
  id: "00000000-0000-0000-0000-000000000000",
  name: null,
  isFork: false,
  url: null,
  remoteUrl: null,
  sshUrl: null,
  projectReference: {} as any,
  collection: {} as any,
}

export interface GitRepository {
  id: string
  name: string | null
  url: string | null
  projectReference: TeamProjectReference
  defaultBranch: string | null
  size: number | null
  remoteUrl: string | null
  sshUrl: string | null
  webUrl: string | null
  validRemoteUrls: (string | null)[]
  isFork: boolean
  parentRepository: GitRepositoryRef
  links: ReferenceLinks
}

export const defaultGitRepository: GitRepository = {
  id: "00000000-0000-0000-0000-000000000000",
  name: null,
  url: null,
  projectReference: {} as any,
  defaultBranch: null,
  size: null,
  remoteUrl: null,
  sshUrl: null,
  webUrl: null,
  validRemoteUrls: [],
  isFork: false,
  parentRepository: {} as any,
  links: {} as any,
}

export interface GitPush {
  commits: GitCommitRef[]
  refUpdates: GitRefUpdate[]
  repository: GitRepository
  pushedBy: IdentityRef
  pushId: number
  pushCorrelationId: string
  date: string
  url: string | null
  links: ReferenceLinks
}

export const defaultGitPush: GitPush = {
  commits: [],
  refUpdates: [],
  repository: {} as any,
  pushedBy: {} as any,
  pushId: 0,
  pushCorrelationId: "00000000-0000-0000-0000-000000000000",
  date: "1/1/0001 12:00:00 AM",
  url: null,
  links: {} as any,
}

export interface GitUserDate {
  name: string | null
  email: string | null
  date: string
  imageUrl: string | null
}

export const defaultGitUserDate: GitUserDate = {
  name: null,
  email: null,
  date: "1/1/0001 12:00:00 AM",
  imageUrl: null,
}

export interface GitPushRef {
  pushedBy: IdentityRef
  pushId: number
  pushCorrelationId: string
  date: string
  url: string | null
  links: ReferenceLinks
}

export const defaultGitPushRef: GitPushRef = {
  pushedBy: {} as any,
  pushId: 0,
  pushCorrelationId: "00000000-0000-0000-0000-000000000000",
  date: "1/1/0001 12:00:00 AM",
  url: null,
  links: {} as any,
}

export interface GitCommitRef {
  commitId: string | null
  author: GitUserDate
  committer: GitUserDate
  comment: string | null
  commentTruncated: boolean
  changeCounts: { key: any, value: any }[]
  changes: GitChange[]
  parents: (string | null)[]
  url: string | null
  remoteUrl: string | null
  links: ReferenceLinks
  statuses: GitStatus[]
  workItems: ResourceRef[]
  push: GitPushRef
}

export const defaultGitCommitRef: GitCommitRef = {
  commitId: null,
  author: {} as any,
  committer: {} as any,
  comment: null,
  commentTruncated: false,
  changeCounts: [],
  changes: [],
  parents: [],
  url: null,
  remoteUrl: null,
  links: {} as any,
  statuses: [],
  workItems: [],
  push: {} as any,
}

export interface GitTemplate {
  name: string | null
  type: string | null
}

export const defaultGitTemplate: GitTemplate = {
  name: null,
  type: null,
}

export interface FileContentMetadata {
  encoding: number
  encodingWithBom: boolean
  contentType: string | null
  fileName: string | null
  extension: string | null
  isBinary: boolean
  isImage: boolean
  visualStudioWebLink: string | null
}

export const defaultFileContentMetadata: FileContentMetadata = {
  encoding: 0,
  encodingWithBom: false,
  contentType: null,
  fileName: null,
  extension: null,
  isBinary: false,
  isImage: false,
  visualStudioWebLink: null,
}

export interface GitItem {
  objectId: string | null
  originalObjectId: string | null
  gitObjectType: GitObjectType
  commitId: string | null
  latestProcessedChange: GitCommitRef
  path: string | null
  isFolder: boolean
  content: string | null
  contentMetadata: FileContentMetadata
  isSymbolicLink: boolean
  url: string | null
  links: ReferenceLinks
}

export const defaultGitItem: GitItem = {
  objectId: null,
  originalObjectId: null,
  gitObjectType: {} as any,
  commitId: null,
  latestProcessedChange: {} as any,
  path: null,
  isFolder: false,
  content: null,
  contentMetadata: {} as any,
  isSymbolicLink: false,
  url: null,
  links: {} as any,
}

export interface ItemContent {
  content: string | null
  contentType: ItemContentType
}

export const defaultItemContent: ItemContent = {
  content: null,
  contentType: {} as any,
}

export interface GitChange {
  originalPath: string | null
  changeId: number
  newContentTemplate: GitTemplate
  item: GitItem
  sourceServerItem: string | null
  changeType: VersionControlChangeType
  newContent: ItemContent
  url: string | null
}

export const defaultGitChange: GitChange = {
  originalPath: null,
  changeId: 0,
  newContentTemplate: {} as any,
  item: {} as any,
  sourceServerItem: null,
  changeType: {} as any,
  newContent: {} as any,
  url: null,
}

export interface GitStatusContext {
  name: string | null
  genre: string | null
}

export const defaultGitStatusContext: GitStatusContext = {
  name: null,
  genre: null,
}

export interface GitStatus {
  id: number
  state: GitStatusState
  description: string | null
  context: GitStatusContext
  creationDate: string
  updatedDate: string
  createdBy: IdentityRef
  targetUrl: string | null
  links: ReferenceLinks
}

export const defaultGitStatus: GitStatus = {
  id: 0,
  state: {} as any,
  description: null,
  context: {} as any,
  creationDate: "1/1/0001 12:00:00 AM",
  updatedDate: "1/1/0001 12:00:00 AM",
  createdBy: {} as any,
  targetUrl: null,
  links: {} as any,
}

export interface GitRefUpdate {
  repositoryId: string
  name: string | null
  oldObjectId: string | null
  newObjectId: string | null
  isLocked: boolean | null
}

export const defaultGitRefUpdate: GitRefUpdate = {
  repositoryId: "00000000-0000-0000-0000-000000000000",
  name: null,
  oldObjectId: null,
  newObjectId: null,
  isLocked: null,
}

export interface GitCommit {
  treeId: string | null
  commitId: string | null
  author: GitUserDate
  committer: GitUserDate
  comment: string | null
  commentTruncated: boolean
  changeCounts: { key: any, value: any }[]
  changes: GitChange[]
  parents: (string | null)[]
  url: string | null
  remoteUrl: string | null
  links: ReferenceLinks
  statuses: GitStatus[]
  workItems: ResourceRef[]
  push: GitPushRef
}

export const defaultGitCommit: GitCommit = {
  treeId: null,
  commitId: null,
  author: {} as any,
  committer: {} as any,
  comment: null,
  commentTruncated: false,
  changeCounts: [],
  changes: [],
  parents: [],
  url: null,
  remoteUrl: null,
  links: {} as any,
  statuses: [],
  workItems: [],
  push: {} as any,
}

export interface GitCommitChanges {
  changeCounts: { key: any, value: any }[]
  changes: GitChange[]
}

export const defaultGitCommitChanges: GitCommitChanges = {
  changeCounts: [],
  changes: [],
}

