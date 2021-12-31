namespace AzdoTasks

open System.Collections.Generic
open Microsoft.Extensions.Logging
open Microsoft.TeamFoundation.Core.WebApi
open Microsoft.TeamFoundation.WorkItemTracking.WebApi
open Microsoft.VisualStudio.Services.WebApi
open System
open Glow.Azdo.Authentication
open Marten
open System.Linq
open MediatR
open Glow.Core.Actions
open Microsoft.VisualStudio.Services.WebApi.Patch
open Microsoft.VisualStudio.Services.WebApi.Patch.Json
open Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models

module AzdoTasks =

  [<Action(Route = "api/get-area-paths", AllowAnonymous = true)>]
  type GetAreaPaths() =
    interface IRequest<WorkItemClassificationNode>
    member val ProjectId: Nullable<Guid> = Nullable() with get, set

  type GetAreaPathsHandler(clients: AzdoClients) =
    interface IRequestHandler<GetAreaPaths, WorkItemClassificationNode> with

      member  this.Handle(request, token) =
        task {
          let! client = clients.GetAppClient<WorkItemTrackingHttpClient>()
          let! areaPathNode = client.GetClassificationNodeAsync(request.ProjectId.ToString(), TreeStructureGroup.Areas, depth = 5)
          return areaPathNode
        }

  [<CLIMutable>]
  type Workspace =
    { Id: Guid
      DisplayName: string
      ProjectId: Guid
      AreaPath: string
      ApiKeys: List<string> }

  [<Action(Route = "api/delete-workspace", AllowAnonymous = true)>]
  type DeleteWorkspace() =
    interface IRequest<Workspace>
    member val Id = Guid.Empty with get, set

  type DeleteWorkspaceHandler(session: IDocumentSession) =
    interface IRequestHandler<DeleteWorkspace, Workspace> with
      member this.Handle(request, token) =
        task {
          let entity =
            session
              .Query<Workspace>()
              .Single(fun v -> v.Id = request.Id)

          session.Delete entity
          session.SaveChanges()
          return entity
        }

  [<Action(Route = "api/get-workspaces", AllowAnonymous = true)>]
  type GetAreas() =
    interface IRequest<List<Workspace>>

  type GetAreasHandler(store: IDocumentStore) =
    interface IRequestHandler<GetAreas, List<Workspace>> with
      member this.Handle(request, token) =
        task {
          use session = store.LightweightSession()
          let entities = session.Query<Workspace>().ToList()
          return entities
        }

  [<Action(Route = "api/get-workspace", AllowAnonymous = true)>]
  type GetArea() =
    interface IRequest<Workspace>
    member val Id = Guid.Empty with get, set

  type GetAreaHandler(session: IDocumentSession) =
    interface IRequestHandler<GetArea, Workspace> with
      member this.Handle(request, token) =
        task {
          let entity =
            session
              .Query<Workspace>()
              .Single(fun v -> v.Id = request.Id)

          return entity
        }

  [<Action(Route = "api/upsert-workspace", AllowAnonymous = true)>]
  type UpsertWorkspace() =
    interface IRequest<Workspace>
    member val Id = Guid.Empty with get, set
    member val DisplayName: string = String.Empty with get, set
    member val ProjectId: Nullable<Guid> = Nullable() with get, set
    //    member val ProjectName: string with get, set
    member val AreaPath = String.Empty with get, set
    //    member val AreaPath: string with get,set
    member val ApiKeys: List<string> = List<string>() with get, set

  type UpsertAreaHandler(session: IDocumentSession) =
    interface IRequestHandler<UpsertWorkspace, Workspace> with
      member this.Handle(request, token) =
        task {

          let id =
            if request.Id = Guid.Empty then
              Guid.NewGuid()
            else
              request.Id

          let entity =
            { Id = id
              DisplayName = request.DisplayName
              ProjectId = request.ProjectId.Value
              AreaPath = request.AreaPath
              ApiKeys = request.ApiKeys }

          session.Store(entity)

          let! result = session.SaveChangesAsync()
          return entity
        }

  [<Action(Route = "api/get-projects", AllowAnonymous = true)>]
  type GetProjects() =
    interface IRequest<IPagedList<TeamProjectReference>>

  type GetProjectsHandler(clients: AzdoClients) =
    interface IRequestHandler<GetProjects, IPagedList<TeamProjectReference>> with
      member this.Handle(request, token) =
        task {
          let! client = clients.GetAppClient<ProjectHttpClient>()
          let! projects = client.GetProjects()
          return projects
        }

  [<Action(Route = "api/get-task", AllowAnonymous = true)>]
  type GetTask() =
    interface IRequest<WorkItem>
    member val TaskId = 0 with get, set

  type GetTaskHandler(clients: AzdoClients) =
    interface IRequestHandler<GetTask, WorkItem> with
      member this.Handle(request, token) =
        task {
          let! client = clients.GetAppClient<WorkItemTrackingHttpClient>()

          let! data =
            client.GetWorkItemAsync(
              id = request.TaskId,
              expand = WorkItemExpand.All
            //              fields =
//                [| "System.Id"
//                   "System.Title"
//                   "System.State"
//                   "System.BoardColumn
//"System.Description"
//"System.BoardColumnDon"
//"Microsoft.VSTS.Common.Priority"
//"Microsoft.VSTS.Common.StateChangeDate"
//"Microsoft.VSTS.Common.AcceptanceCriteria
//                   "System.AreaPath"
//                   "System.TeamProject"
//                   "Microsoft.VSTS.Common.Priority"
//                   "System.Tags"
//                   "System.Reason"
//                   "System.ChangedDate"
//                   "System.ChangedBy"
//"System.CommentCount"
//                   "System.CreatedDate"
//                   "System.AssignedTo"
//                   "System.WorkItemType" |]
            )

          return data
        }

  [<Action(Route = "api/create-task", AllowAnonymous = true)>]
  type CreateTask() =
    interface IRequest<WorkItem>
    member val Title = String.Empty with get, set
    member val Description = String.Empty with get, set
    member val WorkItemType = String.Empty with get, set
    member val WorkspaceId = Guid.Empty with get, set
    member val CreatedBy = String.Empty with get,set

  [<Action(Route = "api/update-task", AllowAnonymous = true)>]
  type UpdateTask() =
    interface IRequest<WorkItem>
    member val TaskId = 0 with get, set
    member val Title = String.Empty with get, set
    member val Description = String.Empty with get, set

  type UpdateTaskHandler(clients: AzdoClients, session: IDocumentSession, logger: ILogger<UpdateTaskHandler>) =
    interface IRequestHandler<CreateTask, WorkItem> with
      member this.Handle(request, token) =
        task {
          let workspace =
            session
              .Query<Workspace>()
              .Single(fun v -> v.Id = request.WorkspaceId)

          let! client = clients.GetAppClient<WorkItemTrackingHttpClient>()
          let doc = JsonPatchDocument()
          doc.Add(JsonPatchOperation(Operation = Operation.Add, Path = "/fields/System.Title", Value = request.Title))
          doc.Add(JsonPatchOperation(Operation = Operation.Add, Path = "/fields/System.Description", Value = request.Description))
          doc.Add(JsonPatchOperation(Operation = Operation.Add, Path = "/fields/x_CreatedBy", Value = request.CreatedBy))

          let areaPath =
            workspace
              .AreaPath
              .Replace("\\Area\\", "\\")
              .Substring(1)

          doc.Add(JsonPatchOperation(Operation = Operation.Add, Path = "/fields/System.AreaPath", Value = areaPath))
          let! entity = client.CreateWorkItemAsync(doc, workspace.ProjectId, request.WorkItemType)
          return entity
        }

    interface IRequestHandler<UpdateTask, WorkItem> with
      member this.Handle(request, token) =
        task {
          let! client = clients.GetAppClient<WorkItemTrackingHttpClient>()
          let doc = JsonPatchDocument()

          doc.Add(JsonPatchOperation(Operation = Operation.Add, Path = "/fields/System.Title", Value = request.Title))
          doc.Add(JsonPatchOperation(Operation = Operation.Add, Path = "/fields/System.Description", Value = request.Description))

          let! result = client.UpdateWorkItemAsync(doc, request.TaskId, false, false, null, token)
          return result
        }

  type CreateTaskViewmodel() =
    member val Id = Guid.Empty with get, set
    member val WorkItemTypes: List<WorkItemType> = List<WorkItemType>() with get, set

  [<Action(Route = "api/get-create-task-viewmodel", AllowAnonymous = true)>]
  type GetCreateTaskViewmodel() =
    interface IRequest<CreateTaskViewmodel>
    member val WorkspaceId = Guid.Empty with get, set

  type GetCreateTaskViewmodelHandler(clients: AzdoClients, session: IDocumentSession) =
    interface IRequestHandler<GetCreateTaskViewmodel, CreateTaskViewmodel> with
      member this.Handle(request, token) =
        task {
          let! client = clients.GetAppClient<WorkItemTrackingHttpClient>()

          let workspace =
            session
              .Query<Workspace>()
              .Single(fun v -> v.Id = request.WorkspaceId)

          let! types = client.GetWorkItemTypesAsync(workspace.ProjectId)

          let result =
            types
              .Where(fun v ->
                v.Name = "Bug"
                || v.Name = "Task"
                || v.Name = "User Story"
                || v.Name = "Meeting")
              .ToList()

          return CreateTaskViewmodel(WorkItemTypes = result)
        }

  [<Action(Route = "api/get-comments", AllowAnonymous = true)>]
  type GetComments() =
    interface IRequest<WorkItemComments>
    member val TaskId = 0 with get, set

  type GetCommentsHandler(clients: AzdoClients) =
    interface IRequestHandler<GetComments, WorkItemComments> with
      member this.Handle(request, token) =
        task {
          let! client = clients.GetAppClient<WorkItemTrackingHttpClient>()
          let! comments = client.GetCommentsAsync(id = request.TaskId)
          return comments
        }

  [<Action(Route = "api/get-tasks", AllowAnonymous = true)>]
  type GetTasks() =
    interface IRequest<List<WorkItem>>
    member val WorkspaceId = Guid.Empty with get, set
    member val ApiKey = String.Empty with get, set

  type WorkspaceViewmodel() =
    member val WorkspaceId = Guid.Empty with get, set
    member val ProjectName = String.Empty with get, set
    member val AreaPath = String.Empty with get, set

  [<Action(Route = "api/get-workspace-viewmodel", AllowAnonymous = true)>]
  type GetWorkspaceViewmodel() =
    interface IRequest<WorkspaceViewmodel>
    member val WorkspaceId = Guid.Empty with get, set
    member val ApiKey = String.Empty with get, set

  type GetTasksHandler(clients: AzdoClients, session: IDocumentSession) =
    interface IRequestHandler<GetWorkspaceViewmodel, WorkspaceViewmodel> with
      member this.Handle(request, token) =
        task {
          let! client3 = clients.GetAppClient<ProjectHttpClient>()

          let workspace =
            session
              .Query<Workspace>()
              .Single(fun v -> v.Id = request.WorkspaceId)

          let! project = client3.GetProject(workspace.ProjectId.ToString())
          let! client = clients.GetAppClient<WorkItemTrackingHttpClient>()

          return WorkspaceViewmodel(WorkspaceId = workspace.Id, ProjectName = project.Name, AreaPath = workspace.AreaPath)
        }

    interface IRequestHandler<GetTasks, List<WorkItem>> with
      member this.Handle(request, token) =
        task {
          let workspace =
            session
              .Query<Workspace>()
              .Single(fun v -> v.Id = request.WorkspaceId)

          let! client = clients.GetAppClient<WorkItemTrackingHttpClient>()

          let areaPath =
            workspace
              .AreaPath
              .Replace("\\Area\\", "\\")
              .Substring(1)

          let query =
            $"SELECT [System.Id], [System.Title], [System.State], [Id], [Title], [State], [Microsoft.VSTS.Common.Priority] FROM workitems WHERE [System.AreaPath] = '{areaPath}' ORDER BY [System.CreatedDate] asc"

          let workspace =
            session
              .Query<Workspace>()
              .Single(fun v -> v.Id = request.WorkspaceId)

          let! queryResults = client.QueryByWiqlAsync(Wiql(Query = query), workspace.ProjectId)
          // only query specific area
          let! data =
            client.GetWorkItemsBatchAsync(
              WorkItemBatchGetRequest(
                Ids = queryResults.WorkItems.Select(fun v -> v.Id),
                Fields =
                  [| "System.Id"
                     "System.Title"
                     "System.State"
                     "System.ChangedDate"
                     "Microsoft.VSTS.Common.Priority" |],
                AsOf = queryResults.AsOf
              )
            )

          return data
        }
