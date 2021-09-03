export interface GetAvailableFakeUsers {
}

export const defaultGetAvailableFakeUsers: GetAvailableFakeUsers = {
}

export interface FakeUsers {
  values: FakeUser[]
}

export const defaultFakeUsers: FakeUsers = {
  values: [],
}

export interface FakeUser {
  userName: string | null
  password: string | null
}

export const defaultFakeUser: FakeUser = {
  userName: null,
  password: null,
}

