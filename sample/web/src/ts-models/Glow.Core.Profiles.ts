export interface Profile {
  displayName: string | null
  id: string | null
  email: string | null
  identityName: string | null
  isAuthenticated: boolean
  objectId: string | null
  userId: string | null
  scopes: (string | null)[]
  claims: { key: any, value: any }[]
}

export const defaultProfile: Profile = {
  displayName: null,
  id: null,
  email: null,
  identityName: null,
  isAuthenticated: false,
  objectId: null,
  userId: null,
  scopes: [],
  claims: [],
}

