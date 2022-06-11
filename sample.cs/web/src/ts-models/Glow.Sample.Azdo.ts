import { GitCommit, GitCommitRef, GitCommitChanges } from "./Microsoft.TeamFoundation.SourceControl.WebApi"
import { defaultGitCommit, defaultGitCommitRef, defaultGitCommitChanges } from "./Microsoft.TeamFoundation.SourceControl.WebApi"

export interface CreateLibrary {
  projectName: string | null
}

export const defaultCreateLibrary: CreateLibrary = {
  projectName: null,
}

export interface CreatePullRequest {
  projectId: string | null
  path: string | null
  content: string | null
  name: string | null
  description: string | null
}

export const defaultCreatePullRequest: CreatePullRequest = {
  projectId: null,
  path: null,
  content: null,
  name: null,
  description: null,
}

export interface GetCommits {
  projectName: string | null
}

export const defaultGetCommits: GetCommits = {
  projectName: null,
}

export interface GetItems {
  projectId: string | null
}

export const defaultGetItems: GetItems = {
  projectId: null,
}

export interface GetItem {
  projectId: string | null
  path: string | null
}

export const defaultGetItem: GetItem = {
  projectId: null,
  path: null,
}

export interface GetProjects {
}

export const defaultGetProjects: GetProjects = {
}

export interface Commit {
  gitCommit: GitCommit
  gitCommitRef: GitCommitRef
  changes: GitCommitChanges
}

export const defaultCommit: Commit = {
  gitCommit: {} as any,
  gitCommitRef: {} as any,
  changes: {} as any,
}

export interface StringWrapper {
  value: string | null
}

export const defaultStringWrapper: StringWrapper = {
  value: null,
}

