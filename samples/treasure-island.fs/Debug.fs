namespace TreasureIsland

open System
open System.Linq
open Marten
open System.Collections.Generic
open Glow.Core.Actions
open MediatR
open Microsoft.Extensions.Logging

module Debug =

  [<Action(Route = "api/debug/rebuild-projections", AllowAnonymous = true)>]
  type RebuildProjections =
    { ProjectionName: string }
    interface IRequest<Unit>

  type RebuildProjectionsHandler(store: IDocumentStore) =
    interface IRequestHandler<RebuildProjections, Unit> with
      member this.Handle(request, token) =
        task {
          let! daemon = store.BuildProjectionDaemonAsync()
          do! daemon.RebuildProjection(request.ProjectionName, token)
          return Unit.Value
        }

  [<Action(Route = "api/debug/get-documents", AllowAnonymous = true)>]
  type GetDocuments() =
    interface IRequest<IEnumerable<obj>>
    member val DocumentName = Unchecked.defaultof<string> with get, set

  let loadDocuments<'DocumentType> (session: IDocumentSession) =
    task {
      let! result = session.Query<'DocumentType>().ToListAsync()
      return result.Cast<obj>()
    }

  type Handler(session: IDocumentSession, logger: ILogger<Handler>) =

    interface IRequestHandler<GetDocuments, IEnumerable<obj>> with
      member this.Handle(request, token) =

        task {
          let! query =
            match request.DocumentName with
            // | "MeetingItem" -> loadDocuments<Gertrud.Core.MeetingItem> session
            // | "Meeting" -> loadDocuments<Gertrud.Core.Meeting> session
            // | "User" -> loadDocuments<Gertrud.Core.User> session
            // | "Committee" -> loadDocuments<Gertrud.Core.Board> session
            // | "Member" -> loadDocuments<Gertrud.Core.Member> session
            // | "GlobalDomainConfiguration" -> loadDocuments<Gertrud.Core.GlobalDomainConfiguration> session
            | _ -> failwith "not supported"


          return query
        }

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


[<Action(Route = "api/es/get-events2", AllowAnonymous = true)>]
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
