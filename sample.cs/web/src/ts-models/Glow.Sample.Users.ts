export interface User {
  id: string | null
  displayName: string | null
  email: string | null
}

export const defaultUser: User = {
  id: null,
  displayName: null,
  email: null,
}

