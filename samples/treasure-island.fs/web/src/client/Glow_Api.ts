//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as System from "./System"
import * as System_Collections_Generic from "./System_Collections_Generic"

export type GetEsEvents = {
}
export var defaultGetEsEvents: GetEsEvents = {
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
export var defaultEventViewmodel: EventViewmodel = {
  id: '00000000-0000-0000-0000-000000000000',
  version: 0,
  sequence: 0,
  data: {},
  streamId: '00000000-0000-0000-0000-000000000000',
  streamKey: '',
  timestamp: "0000-00-00T00:00:00+00:00",
  tenantId: '',
  eventTypeName: '',
  dotNetTypeName: '',
  causationId: '',
  correlationId: '',
  headers: ({}),
  isArchived: false,
  aggregateTypeName: ''
}

