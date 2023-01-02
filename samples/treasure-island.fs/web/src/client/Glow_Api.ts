//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as TsType from "./TsType"
import * as System from "./System"
import * as System_Collections_Generic from "./System_Collections_Generic"

export type GetEsEvents = {
  
}
export const defaultGetEsEvents: GetEsEvents = {
  
}
export type EventViewmodel = {
  id: System.Guid
  version: System.Int64
  sequence: System.Int64
  data: System.Object
  streamId: System.Guid
  streamKey: System.String
  timestamp: System.DateTimeOffset
  tenantId: System.String
  eventTypeName: System.String
  dotNetTypeName: System.String
  causationId: System.String
  correlationId: System.String
  headers: System_Collections_Generic.Dictionary<System.String,System.Object>
  isArchived: System.Boolean
  aggregateTypeName: System.String
}
export const defaultEventViewmodel: EventViewmodel = {
  id: System.defaultGuid,
  version: System.defaultInt64,
  sequence: System.defaultInt64,
  data: System.defaultObject,
  streamId: System.defaultGuid,
  streamKey: System.defaultString,
  timestamp: System.defaultDateTimeOffset,
  tenantId: System.defaultString,
  eventTypeName: System.defaultString,
  dotNetTypeName: System.defaultString,
  causationId: System.defaultString,
  correlationId: System.defaultString,
  headers: System_Collections_Generic.defaultDictionary(System.defaultString,System.defaultObject),
  isArchived: System.defaultBoolean,
  aggregateTypeName: System.defaultString,
}
