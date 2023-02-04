namespace Glow.Core.MartenAndPgsql

open System
open Marten
open MediatR
open Glow.Core.Actions
open Microsoft.Extensions.Logging

type StreamInfo =
  { LastEdited: DateTimeOffset
    Created: DateTimeOffset
    Version: int64 }

[<Action(Route = "api/events/get-last-modified", Policy = "admin")>]
type GetLastModified =
  { Id: Guid }

  interface IRequest<StreamInfo>

type GetLastModifiedHandler(session: IDocumentSession, logger: ILogger<GetLastModifiedHandler>) =
  interface IRequestHandler<GetLastModified, StreamInfo> with
    member this.Handle(request, _) =
      task {
        logger.LogInformation("Getting stream info for {id}", request.Id)

        // do! System.Threading.Tasks.Task.Delay(1_000)
        let! streamInfo = session.Events.FetchStreamStateAsync(request.Id)
        logger.LogInformation("Information {@info}", streamInfo)

        return
          { Created = streamInfo.Created
            LastEdited = streamInfo.LastTimestamp
            Version = streamInfo.Version }
      }
