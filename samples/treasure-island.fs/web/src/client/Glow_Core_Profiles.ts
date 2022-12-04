///////////////////////////////////////////////////////////
//                          This file is auto generated //
//////////////////////////////////////////////////////////

import * as System from "./System"
import * as System_Collections_Generic from "./System_Collections_Generic"
export type GetProfile = {
  
}
export const defaultGetProfile: GetProfile = {
  
}
export type Profile = {
  displayName: System.String
  id: System.String
  email: System.String
  identityName: System.String
  isAuthenticated: System.Boolean
  objectId: System.String
  userId: System.String
  scopes: System_Collections_Generic.IEnumerable<System.String>
  claims: System_Collections_Generic.IEnumerable<System_Collections_Generic.KeyValuePair<System.String,System.String>>
  authenticationType: System.String
}
export const defaultProfile: Profile = {
  displayName: System.defaultString,
  id: System.defaultString,
  email: System.defaultString,
  identityName: System.defaultString,
  isAuthenticated: System.defaultBoolean,
  objectId: System.defaultString,
  userId: System.defaultString,
  scopes: System_Collections_Generic.defaultIEnumerable(System.defaultString),
  claims: System_Collections_Generic.defaultIEnumerable(System_Collections_Generic.defaultKeyValuePair(System.defaultString,System.defaultString)),
  authenticationType: System.defaultString,
}
