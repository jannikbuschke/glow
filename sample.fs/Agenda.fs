namespace Sample.Fs.Agenda

open System.Collections.Generic
open System.Threading.Tasks
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

module Agenda =

  [<CLIMutable>]
  type Group =
    { Id: Guid
      DisplayName: string
      Duration: int }

  [<CLIMutable>]
  type GeneralBlock =
    { Id: Guid
      DisplayName: string
      Duration: int }

  [<CLIMutable>]
  type MeetingItem =
    { Id: Guid
      DisplayName: string
      //      Group: Group
//      Parent: Nullable<Guid>
//      Minutes: string
      Duration: int }

  [<CLIMutable>]
  type MeetingItemUpserted =
    { Id: Guid
      DisplayName: string
      Index: int }

  [<CLIMutable>]
  type Guest = { AttendsRemotely: bool }

  [<CLIMutable>]
  type AgendaEntry =
    { MeetingItem: MeetingItem
      Parent: AgendaEntry
      DisplayIndex: string }

  type ReorderedAgenda = { OldIndex: int; NewIndex: int }
  type CreatedMeeting = unit

  type MeetingCreated = { Id: Guid }

  type Meeting() =
    member val Id = Guid.Empty with get, set
    member val Items: List<MeetingItem> = List() with get, set
    member this.Apply(event: MeetingCreated) =
      this.Id <- event.Id

      //      let item = this.Items[event.OldIndex]
//      this.Items.Insert(event.NewIndex, item)
    member this.Apply(event: MeetingItemUpserted) =
      Console.WriteLine("meeting item upserted")
      this.Items.Add(
        { Id = event.Id
          DisplayName = event.DisplayName
          Duration = 0 }
      )

    member this.Apply(event: ReorderedAgenda) =
      let item = this.Items[event.OldIndex]
      this.Items.RemoveAt(event.OldIndex)
      this.Items.Insert(event.NewIndex, item)

  [<Action(Route = "api/meeting/create", AllowAnonymous = true)>]
  type CreateMeeting() =
    interface IRequest<Meeting>


  //  [<CLIMutable>]
  type Agenda() =
    member val Items: List<AgendaEntry> = List<AgendaEntry>() with get, set

    member this.Apply(event: ReorderedAgenda) =
      let item = this.Items[event.OldIndex]
      this.Items.Insert(event.NewIndex, item)
//      this.Items.RemoveAt(event.OldIndex)

  [<Action(Route = "api/get-meeting", AllowAnonymous = true)>]
  type GetMeeting() =
    interface IRequest<Meeting>
    member val MeetingId: Guid = Guid.Empty with get, set
    member val Version: int64 = 0 with get,set

  [<Action(Route = "api/reorder-agenda-items", AllowAnonymous = true)>]
  type ReorderAgendaItems() =
    interface IRequest<Unit>
    member val MeetingId: Guid = Guid.Empty with get, set
    member val OldIndex: int = 0 with get, set
    member val NewIndex: int = 0 with get, set

  [<Action(Route = "api/upsert-meeting-item", AllowAnonymous = true)>]
  type UpsertMeetingItem() =
    interface IRequest
    member val MeetingId: Guid = Guid.Empty with get, set

    member val MeetingItem: MeetingItem =
      { Id = Guid.Empty
        DisplayName = ""
        Duration = 0 } with get, set

  type GetAreaPathsHandler(store: IDocumentStore, session: IQuerySession) =
    interface IRequestHandler<ReorderAgendaItems, Unit> with
      member this.Handle(request, token) =
          task{

            let session = store.OpenSession()

            let event: ReorderedAgenda =
              { OldIndex = request.OldIndex; NewIndex = request.NewIndex }

            let state1 =
              session.Events.Append(request.MeetingId, event)

            let! result = session.SaveChangesAsync()
            return Unit.Value
          }

    interface IRequestHandler<CreateMeeting, Meeting> with
      member this.Handle(request, token) =
        task {
          let id = Guid.NewGuid()
          let session = store.OpenSession()
          // session.Events.StartStream(typeof(Quest), questId, started, joined1);
          let meetingCreated: MeetingCreated = { Id = id }
          let meetingCreatedEvent = ()
          let objects: obj [] = [| meetingCreated |]

          let state1 =
            session.Events.StartStream<Meeting>(id, objects)

          let! result = session.SaveChangesAsync()

          let meeting = Meeting(Id = id, Items = List())
          return meeting
        }

    interface IRequestHandler<UpsertMeetingItem, Unit> with
      member this.Handle(request, token) =
        task {
          let session = store.OpenSession()
          let event: MeetingItemUpserted =
            { Id = request.MeetingItem.Id
              DisplayName = request.MeetingItem.DisplayName
              Index = 0 }

          session.Events.Append(request.MeetingId, event) |> ignore

          let! result = session.SaveChangesAsync()
          return Unit.Value
        }

    interface IRequestHandler<GetMeeting, Meeting> with
      member this.Handle(request, token) =
        task {
          let session = store.OpenSession()
          let! meeting = session.Events.AggregateStreamAsync<Meeting>(request.MeetingId, request.Version)
          return meeting
        }
