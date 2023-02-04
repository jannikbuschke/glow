namespace Glow.Core.MartenAndPgsql

open System
open System.Collections.Generic
open Glow.Core.Actions
open Marten
open MediatR
open System.Linq
open Microsoft.Extensions.Configuration
open Dapper

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

type EventViewmodel2 =
  { seq_id: System.Int64
    id: System.Guid
    stream_id: System.Guid
    version: System.Int64
    data: System.String
    ``type``: System.String
    timestamp: NodaTime.Instant
    tenant_id: System.String
    mt_dotnet_type: System.String
    headers: System.String
    is_archived: System.Boolean }

module EventViewmodel =
  let fromMartenEvent (v: Marten.Events.IEvent) =
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
      AggregateTypeName = v.AggregateTypeName }

[<Action(Route = "api/es/get-events", Policy = "admin")>]
type GetEsEvents() =
  interface IRequest<EventViewmodel list>

type GetEventsHandler(session: IDocumentSession) =
  interface IRequestHandler<GetEsEvents, EventViewmodel list> with

    member this.Handle(_, _) =
      task {
        let! events =
          session
            .Events
            .QueryAllRawEvents()
            .OrderByDescending(fun x -> x.Sequence)
            .ToListAsync()

        return
          events
          |> Seq.map EventViewmodel.fromMartenEvent
          |> Seq.toList
      }

[<Action(Route = "api/es/get-events-without-validation", Policy = "admin")>]
type GetEsEventsWithoutValidation() =
  interface IRequest<ResizeArray<EventViewmodel2>>

type GetEventsHandler2(session: IDocumentSession, config: IConfiguration) =
  interface IRequestHandler<GetEsEventsWithoutValidation, ResizeArray<EventViewmodel2>> with
    member this.Handle(_, _) =
      task {
        let cs = config.GetValue<string>("PostgresConnectionString")
        use conn = new Npgsql.NpgsqlConnection(cs)
        do! conn.OpenAsync()

        let result =
          conn.Query<EventViewmodel2>(
            "select * from mt_events order by seq_id desc"
          )

        return ResizeArray result
      }
