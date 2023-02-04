namespace Glow.Core.MartenAndPgsql

open System.Collections.Generic
open FSharp.Control
open Glow.Core.Actions
open Marten
open Marten.Events.Projections
open MediatR
open Microsoft.Extensions.Logging
open System.Linq


[<Action(Route = "api/debug/rebuild-projection", Policy = "admin")>]
type RebuildProjections =
  { ProjectionName: string }

  interface IRequest<MediatR.Unit>

[<Action(Route = "api/debug/rebuild-all-projections", Policy = "admin")>]
type RebuildAllProjections =
  { Payload: MediatR.Unit }

  interface IRequest<MediatR.Unit>

type RebuildProjectionsHandler
  (
    store: IDocumentStore,
    document: IDocumentSession,
    logger: ILogger<RebuildProjectionsHandler>
  ) =
  interface IRequestHandler<RebuildProjections, MediatR.Unit> with
    member this.Handle(request, token) =
      task {
        let! daemon = store.BuildProjectionDaemonAsync()
        do! daemon.RebuildProjection(request.ProjectionName, token)
        return MediatR.Unit.Value
      }

  interface IRequestHandler<RebuildAllProjections, MediatR.Unit> with
    member this.Handle(_, token) =
      task {
        let! daemon = store.BuildProjectionDaemonAsync()

        let projections =
          store.Options.Events.Projections()
          |> Seq.filter (fun v -> v.Lifecycle = ProjectionLifecycle.Inline)
          |> Seq.map (fun v -> v.ProjectionName)
          |> Seq.toList

        logger.LogInformation("Projections {@projections}", projections)

        let tasks =
          projections
          |> List.map (fun v -> daemon.RebuildProjection(v, token))

        let! finished = System.Threading.Tasks.Task.WhenAll tasks
        // do!
        //   projections
        //   |> AsyncSeq.ofSeq
        //   |> AsyncSeq.iterAsync (fun v ->
        //     async {
        //       do!
        //         daemon.RebuildProjection(v, token)
        //         |> Async.AwaitTask
        //     })
        //   |> Async.StartAsTask

        return MediatR.Unit.Value
      }

[<Action(Route = "api/gebug/get-document-names", Policy = "admin")>]
type GetKnownDocumentNames =
  { Dummy: MediatR.Unit }

  interface IRequest<string list>


[<Action(Route = "api/debug/get-documents", Policy = "admin")>]
type GetDocuments() =
  interface IRequest<IEnumerable<obj>>
  member val DocumentName = Unchecked.defaultof<string> with get, set

type Handler
  (
    session: IDocumentSession,
    logger: ILogger<Handler>,
    store: IDocumentStore
  ) =
  let loadDocuments (session: IDocumentSession) =
    task {
      let! result = session.Query<'DocumentType>().ToListAsync()
      return result.Cast<obj>()
    }

  interface IRequestHandler<GetKnownDocumentNames, string list> with
    member this.Handle(_, _) =
      task {
        return
          store.Options.AllKnownDocumentTypes()
          |> Seq.map (fun v -> v.Alias)
          |> Seq.toList
      }

  interface IRequestHandler<GetDocuments, IEnumerable<obj>> with
    member this.Handle(request, _) =

      task {

        let! query =
          match request.DocumentName.ToLower() with
          | "meetingitem" -> loadDocuments session
          | "meeting" -> loadDocuments session
          | "user" -> loadDocuments session
          | "committee" -> loadDocuments session
          | "member" -> loadDocuments session
          | "domainconfiguration" -> loadDocuments session
          | "workspacesingletonconfiguration" -> loadDocuments session
          | "mynotificationsettings" -> loadDocuments session
          | "file" -> loadDocuments session
          | _ -> failwith "not supported"

        return query
      }
