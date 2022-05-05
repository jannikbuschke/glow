namespace Sample.Fs.Agenda

open System.Collections.Generic
open System.Threading.Tasks
open Marten.Events.Projections
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open System
open Marten
open MediatR
open Glow.Core.Actions

type Person = { FirstName: string; LastName: string }

type WrappListOfOptions = { Persons: Person option list }

type GetListController()=
    inherit ControllerBase()

    [<HttpGet("test")>]
    member this.Get()=
      { Persons = [ Some({ FirstName = ""; LastName = "" }); None;None;Some({FirstName="Hello";LastName="World"}) ] }
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

  type MeetingCreated = { Id: Guid; Name: string }

  type Meeting() =
    member val Id = Guid.Empty with get, set
    member val Name = String.Empty with get, set
    member val Items: List<MeetingItem> = List() with get, set

    member this.Apply(event: MeetingCreated) =
      this.Id <- event.Id
      this.Name <- event.Name

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



  [<Action(Route="get-wrapped-option-list", AllowAnonymous=true)>]
  type GetListOfOptions() =
    interface IRequest<WrappListOfOptions>

  type GetListOfOptionsHandler() =
    interface IRequestHandler<GetListOfOptions, WrappListOfOptions> with
      member this.Handle(request, token) =
        Task.FromResult { Persons = [ Some({ FirstName = ""; LastName = "" }); None;None;Some({FirstName="Hello";LastName="World"}) ] }

  [<Action(Route = "api/get-meeting", AllowAnonymous = true)>]
  type GetMeeting() =
    interface IRequest<Meeting>
    member val MeetingId: Guid = Guid.Empty with get, set
    member val Version: int64 = 0 with get, set

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
        task {
          let session = store.OpenSession()

          let event: ReorderedAgenda =
            { OldIndex = request.OldIndex
              NewIndex = request.NewIndex }

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
          let meetingCreated: MeetingCreated = { Id = id; Name = "Meeting xy" }
          //          let meetingCreatedEvent = ()
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

          session.Events.Append(request.MeetingId, event)
          |> ignore

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

  type MeetingView() =
    member val Id: Guid = Guid.Empty with get, set
    member val Name = String.Empty with get, set

  type MeetingReorderedView() =
    member val Id: Guid = Guid.Empty with get, set
    member val Name = String.Empty with get, set

  type MonsterDefeatedTransform() =
    inherit EventProjection()
    with
      member this.Create(input: Marten.Events.IEvent<MeetingCreated>) =
        MeetingView(Id = input.Id, Name = input.Data.Name)

      member this.Create(input: Marten.Events.IEvent<ReorderedAgenda>) =
        MeetingReorderedView(Id = input.Id, Name = "Reordered")
