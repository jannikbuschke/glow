namespace Glow.Debug

open Glow.Core.Actions
open MediatR
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Npgsql

module PgsqlHelper =

  type Activity =
    { Username: obj
      Db: obj
      // Address:string
      State: obj
      Query: obj
      Pid: obj }

  [<Action(Route = "api/glow/pgsql/get-activity", Policy = "AuthenticatedUser")>]
  type GetPgsqlActivities() =
    interface IRequest<ResizeArray<Activity>>

  //    [<Action(Route = "api/pgsql/exec", Policy = Policies.Planner)>]
//    type ExecuteSql() =
//        interface IRequest<ResizeArray<Activity>>

  type ExecuteSqlHandler
    (
      config: IConfiguration,
      logger: ILogger<GetPgsqlActivities>
    ) =
    interface IRequestHandler<GetPgsqlActivities, ResizeArray<Activity>> with
      member this.Handle(request, cancellationToken) =
        task {
          let cs =
            config.GetValue<string>("PostgresConnectionString")

          use conn = new Npgsql.NpgsqlConnection(cs)
          let! x = conn.OpenAsync()

          use cmd =
            new NpgsqlCommand("SELECT * FROM pg_stat_activity", conn)

          logger.LogInformation("read with npgsql")
          use! reader = cmd.ExecuteReaderAsync()
          let mutable finished = false
          let r = ResizeArray<Activity>()

          while not finished do
            let! result = reader.ReadAsync()

            if not result then
              finished <- true
            else
              let username =
                reader.GetValue(reader.GetOrdinal("usename"))

              let query =
                reader.GetValue(reader.GetOrdinal("query"))

              let db =
                reader.GetValue(reader.GetOrdinal("datname"))

              let pid =
                reader.GetValue(reader.GetOrdinal("pid"))

              let address =
                reader.GetValue(reader.GetOrdinal("client_addr"))

              let state =
                reader.GetValue(reader.GetOrdinal("state"))

              r.Add(
                { Username = username
                  Db = db
                  State = state
                  Query = query
                  Pid = pid }
              )

          logger.LogInformation("finished")
          return r
        }
