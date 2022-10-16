
module TestMonads

module Async =
  let map a = 
    async{
      return a
    }


type UserId = | UserId of string
type User={Id:UserId;Name:string}

type ValidateError = | ValidateError of string
type LoadError = | LoadError of string

type Error = | Validation of ValidateError | Load of LoadError

type LoadUserWithError = UserId -> Async<Result<User,string>>

type LoadUser = UserId -> Async<User>

let loadUser (userId: UserId) =
  async {
    return { Id = userId; Name = "" }
  }

type LoadUserOptional = Option<UserId> -> Async<Option<User>>

let bindAsyncOption f v =
  async {
    match v with 
    | Some v ->
      let! result = f v
      return Some result
    | None -> return None
  }

let loadUser1: LoadUserOptional = bindAsyncOption loadUser

let loadUserO: LoadUserOptional = fun (userId) ->
    async {
        match userId with 
        | Some id -> return Some { Id = id; Name = "" }
        | None -> return None
    }

type ValidateUserId = string -> Option<UserId>

let loadUser5 (validate: ValidateUserId) (load: LoadUser) (id: string) =
  async{
    let! result = (validate id) |> bindAsyncOption load
    let! result = id |> validate |> bindAsyncOption load
    return result
  }

// let bindAsyncResult f v =
//   async {
//     match v with 
//     | Result.Ok v ->
//       let! result = f v
//       return Some result
//     | Result.Error e -> return None
//   }

// let mapValidationError e = Error.Validation e
let mapValidationError = Error.Validation
let mapLoadError = Error.Load

let mapAsyncError map f =
  async {
    let! result = f
    let result = result |> Result.mapError map
    return result
  }

let bindAsyncResult2 f a =
  match a with
     | Result.Ok a -> 
        async{
         let! result = f a
         let result = match result with | Ok result -> Ok result | Error e -> Result.Error( Error.Load e)
         return result
      }
     | Result.Error e -> async{return Result.Error e}

let loadUser6 (validate: string -> Result<UserId, ValidateError>) (load: UserId -> Async<Result<User, LoadError>>) (id: string) =
  async {
    let! result = id 
                    |> validate
                    |> Result.mapError Error.Validation
                    |> bindAsyncResult2 load

    return result
  }
    