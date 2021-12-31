module MartenTest

open System
open System.Threading.Tasks
open Marten
open System.Linq
open System.Collections
open System.Linq.Expressions
open MediatR
open Microsoft.EntityFrameworkCore
open Glow.Core.Actions

[<CLIMutable>]
type MartenUser =
  { PersonId: Guid
    FirstName: string
    LastName: string
    Internal: bool
    UserName: string
    Department: string
    Id: Guid }

type ArrivedAtLocation = { Day: int; Location: string }

type MemberJoined = { Name: string }

type Quest() =
  member val Id = Guid.Empty with get, set
  member val Name = "" with get, set
  member val Location = "" with get, set
  //  member val Members = (upcast IEnumerable<string> List<string>.Empty) with get, set
  member this.Apply(event: ArrivedAtLocation) = this.Location <- event.Location

//let Test () =
//  let store =
//    DocumentStore.For("User ID=postgres;Password=postgreS123asd123;Host=localhost;Port=5432;Database=marten-test;Pooling=true;Connection Lifetime=0;")
//
//  use session = store.LightweightSession()
//
//  let x =
//    { FirstName = ""
//      PersonId = Guid.NewGuid()
//      LastName = "asd"
//      UserName = "asd"
//      Internal = false
//      Department = ""
//      Id = Guid.Parse("00000000-0000-0000-0000-000000000001") }
//
//  session.Store(x)
//  session.SaveChanges()
//
//  use session1 = store.QuerySession()
//
//  let users1 = session1.Query<MartenUser>().ToList()
//  let questId = Guid.Parse("00000000-0000-0000-0000-000000000001")
//  let questId2 = Guid.Parse("00000000-0000-0000-0000-000000000002")
//
//  let insertEvents () =
//    use session0 = store.OpenSession()
//
//    let started: MemberJoined = { Name = "Destroy the One Ring" }
//    let joined1: ArrivedAtLocation = { Day = 5; Location = "Hamburg" }
//
//    let objects: obj [] = [| started; joined1 |]
//
//    let state1 = session0.Events.FetchStreamState(questId)
//    let state2 = session0.Events.FetchStreamState(questId2)
//
//    if state2 = null then
//      let action = session0.Events.StartStream<Quest>(questId2, objects)
//      action
//    else
//      let action = session0.Events.Append(questId2, objects)
//      action
//
//    session0.SaveChanges()
//    None
//
//  insertEvents () |> ignore
//  use session0 = store.OpenSession()
//
//  let party = session0.Events.AggregateStream<Quest>(questId)
//
//  Console.WriteLine(party)
//
//  let version: int64 = 3L
//
//  let party_at_version_3 = session.Events.AggregateStream<Quest>(questId, 3L)
//
//  let party_yesterday = session.Events.AggregateStream<Quest>(questId, 0L, DateTime.UtcNow.AddDays(-1.0))
//
//
//  let x = 5
//  x
