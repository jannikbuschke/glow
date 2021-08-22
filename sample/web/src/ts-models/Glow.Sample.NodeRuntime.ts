export interface CreateSourceFile {
  path: string | null
  content: string | null
}

export const defaultCreateSourceFile: CreateSourceFile = {
  path: null,
  content: null,
}

export interface UpdateSourceFile {
  path: string | null
  content: string | null
  id: string | null
}

export const defaultUpdateSourceFile: UpdateSourceFile = {
  path: null,
  content: null,
  id: null,
}

export interface GetSourceFileList {
}

export const defaultGetSourceFileList: GetSourceFileList = {
}

export interface GetSourceFileViewmodel {
  id: string | null
}

export const defaultGetSourceFileViewmodel: GetSourceFileViewmodel = {
  id: null,
}

export interface SourceFile {
  id: string
  path: string | null
  content: string | null
}

export const defaultSourceFile: SourceFile = {
  id: "00000000-0000-0000-0000-000000000000",
  path: null,
  content: null,
}

export interface SourceFileViewmodel {
  id: string
  path: string | null
  content: string | null
  code: string | null
  error: string | null
}

export const defaultSourceFileViewmodel: SourceFileViewmodel = {
  id: "00000000-0000-0000-0000-000000000000",
  path: null,
  content: null,
  code: null,
  error: null,
}

