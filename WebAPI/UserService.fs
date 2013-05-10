namespace IssueTracker.Web
(*
open System
open ServiceStack.ServiceHost

[<Route("/user/{User}/issues/reported")>]
[<CLIMutable>]
type ReportedIssues = { User: string }

[<Route("/user/{User}/issues/taken")>]
[<CLIMutable>]
type TakenIssues = { User: string }

type UserService() =
    interface IService

    member this.Get (req:ReportedIssues) = "Here be opened issues for user: " + req.User
    member this.Get (req:TakenIssues) = "Here be issues taken by user: " + req.User
    *)