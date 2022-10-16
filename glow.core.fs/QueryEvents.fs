module Glow.Api.EventsQueries

open System
open System.Collections.Generic
open Glow.Core.Actions
open Marten
open MediatR
open System.Linq
// [<Action(Route = "api/es/get-events", AllowAnonymous = true)>]
// type GetEsEvents() =
//   interface IRequest<System.Collections.Generic.IReadOnlyList<Marten.Events.IEvent>>
//
// type GetEventsHandler(session: IDocumentSession) =
//   interface IRequestHandler<GetEsEvents, System.Collections.Generic.IReadOnlyList<Marten.Events.IEvent>> with
//     member this.Handle(request, token) =
//       task {
//         let! events =
//           session
//             .Events
//             .QueryAllRawEvents()
//             .OrderByDescending(fun x -> x.Sequence)
//             .ToListAsync()
//
//         return events
//       }


type EventViewmodel =
  { Id: System.Guid
    Version: int64
    Sequence: int64
    Data: obj
    StreamId: System.Guid
    StreamKey: string
    Timestamp: DateTimeOffset
    TenantId: string
    // Type EventType { get; }
    EventTypeName: string
    DotNetTypeName: string
    CausationId: string
    CorrelationId: string
    Headers: Dictionary<string, obj>

    IsArchived: bool
    AggregateTypeName: string }


[<Action(Route = "api/es/get-events", AllowAnonymous = true)>]
type GetEsEvents() =
  interface IRequest<ResizeArray<EventViewmodel>>

type GetEventsHandler(session: IDocumentSession) =
  interface IRequestHandler<GetEsEvents, ResizeArray<EventViewmodel>> with

    member this.Handle(request, token) =
      task {
        let! events =
          session
            .Events
            .QueryAllRawEvents()
            .OrderByDescending(fun x -> x.Sequence)
            .ToListAsync()

        return
          events
            .Select(fun v ->
              { Id = v.Id
                Version = v.Version
                Sequence = v.Sequence
                Data = v.Data
                StreamId = v.StreamId
                StreamKey = v.StreamKey
                Timestamp = v.Timestamp
                TenantId = v.TenantId
                // Type EventType { get; }
                EventTypeName = v.EventTypeName
                DotNetTypeName = v.DotNetTypeName
                CausationId = v.CausationId
                CorrelationId = v.CorrelationId
                Headers = v.Headers

                IsArchived = v.IsArchived
                AggregateTypeName = v.AggregateTypeName })
            .ToList()
      }
