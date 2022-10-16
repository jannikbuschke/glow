module Test

open FsToolkit.ErrorHandling

type AuthError = { Msg: string }
type TokenError = { Msg: string }
type AuthToken = { Token: string }
type User = { Name: string }

let tryGetUser userName =
    async { return Some { Name = userName } }

let isPwdValid password user = true

let createAuthToken user = Result.Error { TokenError.Msg = "" }

let authorize user =
    async { return Result.Error { AuthError.Msg = "error" } }

type LoginError =
    | InvalidUser
    | InvalidPwd
    | Unauthorized of AuthError
    | TokenErr of TokenError

let login (username: string) (password: string) : Async<Result<AuthToken, LoginError>> =
    asyncResult {
        // requireSome unwraps a Some value or gives the specified error if None
        let! user =
            username
            |> tryGetUser
            |> AsyncResult.requireSome InvalidUser

        // requireTrue gives the specified error if false
        do!
            user
            |> isPwdValid password
            |> Result.requireTrue InvalidPwd

        // Error value is wrapped/transformed (Unauthorized has signature AuthError -> LoginError)
        do!
            user
            |> authorize
            |> AsyncResult.mapError Unauthorized

        // Same as above, but synchronous, so we use the built-in mapError
        return!
            user
            |> createAuthToken
            |> Result.mapError TokenErr
    }
