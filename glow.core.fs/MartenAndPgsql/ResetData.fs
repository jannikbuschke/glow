namespace Glow.Core.MartenAndPgsql

open System
open Glow.Core.Actions
open MediatR
open Microsoft.AspNetCore.Hosting
open Marten
open FSharp.Control
open Dapper
open Microsoft.Extensions.Configuration
open FsToolkit.ErrorHandling

type ApiErrorType =
  | NotFound
  | BadRequest
  | Unauthorized
  | Forbidden
  | InternalServerError

type ApiError = { Message: string; Type: ApiErrorType }

[<Action(Route = "api/debug/archive-event", Policy = "admin")>]
type ArchiveEvent =
  { EventId: Guid

   }

  interface IRequest<Result<unit, ApiError>>

[<Action(Route = "api/debug/restore-event", Policy = "admin")>]
type RestoreEvent =
  { EventId: Guid }

  interface IRequest<Result<unit, ApiError>>

[<Action(Route = "api/debug/rename-event-dotnet-type", Policy = "admin")>]
type RenameEventDotnetTypeName =
  { OldName: string
    NewName: string }

  interface IRequest<Result<int, ApiError>>

[<Action(Route = "api/application/remove-all-event-data", Policy = "admin")>]
type RemoveAllData() =
  interface IRequest<MediatR.Unit>
  member val ValidationPhrase = Unchecked.defaultof<string> with get, set

type RemoveAllDataHandler(store: IDocumentStore, env: IWebHostEnvironment) =
  interface IRequestHandler<RemoveAllData> with
    member this.Handle(request, cancellationToken) =
      task {
        if env.EnvironmentName = "Development"
           || request.ValidationPhrase = "I know what I am doing" then
          do! store.Advanced.Clean.DeleteAllEventDataAsync()

        return MediatR.Unit.Value
      }

type RenameEventDotnetTypeNameHandler
  (
    session: IDocumentSession,
    config: IConfiguration
  ) =
  interface IRequestHandler<RestoreEvent, Result<unit, ApiError>> with
    member this.Handle(request, token) =
      taskResult {
        // mt_dotnet_type
        // Gertrud.Core.MeetingEvent+MeetingCreated, shared.fs
        let connectionString =
          config.GetValue<string>("PostgresConnectionString")

        use connection = new Npgsql.NpgsqlConnection(connectionString)
        do! connection.OpenAsync()

        let! result =
          connection.ExecuteAsync(
            "UPDATE public.mt_events SET is_archived = false WHERE id = @id",
            {| id = request.EventId |}
          )

        return ()
      }

  interface IRequestHandler<ArchiveEvent, Result<unit, ApiError>> with
    member this.Handle(request, token) =
      taskResult {
        // mt_dotnet_type
        // Gertrud.Core.MeetingEvent+MeetingCreated, shared.fs
        let connectionString =
          config.GetValue<string>("PostgresConnectionString")

        use connection = new Npgsql.NpgsqlConnection(connectionString)
        do! connection.OpenAsync()

        let! result =
          connection.ExecuteAsync(
            "UPDATE public.mt_events SET is_archived = true WHERE id = @id",
            {| id = request.EventId |}
          )

        return ()
      }

  interface IRequestHandler<RenameEventDotnetTypeName, Result<int, ApiError>> with
    member this.Handle(request, token) =
      taskResult {
        //mt_dotnet_type
        // Gertrud.Core.MeetingEvent+MeetingCreated, shared.fs
        let connectionString =
          config.GetValue<string>("PostgresConnectionString")

        use connection = new Npgsql.NpgsqlConnection(connectionString)
        do! connection.OpenAsync()

        let! result =
          connection.ExecuteAsync(
            "UPDATE public.mt_events SET mt_dotnet_type = @newName WHERE mt_dotnet_type = @oldName",
            {| oldName = request.OldName
               newName = request.NewName |}
          )

        return result
      }
