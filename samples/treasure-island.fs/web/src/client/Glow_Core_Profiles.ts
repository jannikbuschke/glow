//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as System from "./System"
import * as System_Collections_Generic from "./System_Collections_Generic"

export type GetProfile = {
}
export var defaultGetProfile: GetProfile = {
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
  upn: System.String
}
export var defaultProfile: Profile = {
  displayName: '',
  id: '',
  email: '',
  identityName: '',
  isAuthenticated: false,
  objectId: '',
  userId: '',
  scopes: [],
  claims: [],
  authenticationType: '',
  upn: ''
}

