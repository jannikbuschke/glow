/* eslint-disable prettier/prettier */
export interface CreateUser {
  displayName: string | null
  email: string | null
}

export const defaultCreateUser: CreateUser = {
  displayName: null,
  email: null,
}

