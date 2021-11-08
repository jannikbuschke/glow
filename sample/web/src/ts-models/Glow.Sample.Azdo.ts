/* eslint-disable prettier/prettier */
import { GitCommit, GitCommitRef, GitCommitChanges } from "./Microsoft.TeamFoundation.SourceControl.WebApi"
import { defaultGitCommit, defaultGitCommitRef, defaultGitCommitChanges } from "./Microsoft.TeamFoundation.SourceControl.WebApi"

export interface CreateCommit {
  projectName: string | null
  content: string | null
}

export const defaultCreateCommit: CreateCommit = {
  projectName: null,
  content: null,
}

export interface GetCommits {
  projectName: string | null
}

export const defaultGetCommits: GetCommits = {
  projectName: null,
}

export interface CreateLibrary {
  projectName: string | null
}

export const defaultCreateLibrary: CreateLibrary = {
  projectName: null,
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

