namespace IssueTracker.Web

open System
open ServiceStack.ServiceHost

[<Route("/test")>]
type Request() = class end

[<CLIMutable>]
type Response = { Result: string; }

type UserService() =
    interface IService

    member this.Get (req:Request) = { Result = "Testtt" }
