export interface CreateMdx {
  path: string | null
  content: string | null
}

export const defaultCreateMdx: CreateMdx = {
  path: null,
  content: null,
}

export interface UpdateMdx {
  path: string | null
  content: string | null
  id: string | null
}

export const defaultUpdateMdx: UpdateMdx = {
  path: null,
  content: null,
  id: null,
}

export interface GetMdxList {
}

export const defaultGetMdxList: GetMdxList = {
}

export interface GetEntityViewmodel {
  id: string | null
}

export const defaultGetEntityViewmodel: GetEntityViewmodel = {
  id: null,
}

export interface Transpile {
  source: string | null
}

export const defaultTranspile: Transpile = {
  source: null,
}

export interface Mdx {
  id: string
  path: string | null
  content: string | null
}

export const defaultMdx: Mdx = {
  id: "00000000-0000-0000-0000-000000000000",
  path: null,
  content: null,
}

export interface MdxViewmodel {
  id: string
  path: string | null
  content: string | null
  code: string | null
  error: string | null
  frontmatter: { [key: string]: string }
}

export const defaultMdxViewmodel: MdxViewmodel = {
  id: "00000000-0000-0000-0000-000000000000",
  path: null,
  content: null,
  code: null,
  error: null,
  frontmatter: {},
}

export interface TranspileResult {
  code: string | null
  frontmatter: { [key: string]: string }
}

export const defaultTranspileResult: TranspileResult = {
  code: null,
  frontmatter: {},
}

