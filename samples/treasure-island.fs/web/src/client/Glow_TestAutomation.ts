//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as TsType from "./TsType"
import * as System_Collections_Generic from "./System_Collections_Generic"
import * as System from "./System"

export type GetAvailableFakeUsers = {
  
}
export const defaultGetAvailableFakeUsers: GetAvailableFakeUsers = {
  
}
export type FakeUser = {
  userName: System.String
  password: System.String
}
export const defaultFakeUser: FakeUser = {
  userName: System.defaultString,
  password: System.defaultString,
}
export type FakeUsers = {
  values: System_Collections_Generic.IEnumerable<FakeUser>
}
export const defaultFakeUsers: FakeUsers = {
  values: System_Collections_Generic.defaultIEnumerable(defaultFakeUser),
}
