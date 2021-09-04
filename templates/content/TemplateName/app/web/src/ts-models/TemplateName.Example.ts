export interface CreatePerson {
  name: string | null
}

export const defaultCreatePerson: CreatePerson = {
  name: null,
}

export interface UpdatePerson {
  name: string | null
  id: string | null
}

export const defaultUpdatePerson: UpdatePerson = {
  name: null,
  id: null,
}

export interface GetPersonList {
}

export const defaultGetPersonList: GetPersonList = {
}

export interface GetPerson {
  id: string | null
}

export const defaultGetPerson: GetPerson = {
  id: null,
}

export interface DeletePerson {
  id: string | null
}

export const defaultDeletePerson: DeletePerson = {
  id: null,
}

export interface Person {
  id: string
  name: string | null
}

export const defaultPerson: Person = {
  id: "00000000-0000-0000-0000-000000000000",
  name: null,
}

