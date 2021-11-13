export interface Create {
  path: string | null
  content: string | null
}

export const defaultCreate: Create = {
  path: null,
  content: null,
}

export interface Update {
  path: string | null
  content: string | null
  id: string | null
}

export const defaultUpdate: Update = {
  path: null,
  content: null,
  id: null,
}

export interface GetList {
}

export const defaultGetList: GetList = {
}

export interface GetEntityViewmodel {
  id: string | null
}

export const defaultGetEntityViewmodel: GetEntityViewmodel = {
  id: null,
}

export interface Entity {
  id: string
  path: string | null
  content: string | null
}

export const defaultEntity: Entity = {
  id: "00000000-0000-0000-0000-000000000000",
  path: null,
  content: null,
}

export interface EntityViewmodel {
  id: string
  path: string | null
  content: string | null
  code: string | null
  error: string | null
}

export const defaultEntityViewmodel: EntityViewmodel = {
  id: "00000000-0000-0000-0000-000000000000",
  path: null,
  content: null,
  code: null,
  error: null,
}

