namespace IssueTracker.Web

open System
open ServiceStack.ServiceHost
open IssueTracker
open IssueTracker.EventStore.Integration
open IssueTracker.ReadModels

type M = CLIMutableAttribute

[<Route("/user/{user}/issues/reported", "GET")>]
[<M>] type ReportedIssuesRequest = { User: string }
[<M>] type ReportedIssue = { Number: int; Summary: string }

[<Route("/user/{user}/issues/reported", "POST")>]
[<M>] type ReportRequest = { user: string; summary: string }
[<M>] type ReportResponse = { id: string }

[<Route("/user/{User}/issues/taken")>]
[<M>] type TakenIssuesRequest = { User: string }

[<Route("/issue/{id}/")>]
[<M>] type IssueRequest = { id: string }

[<Route("/issue/{id}/take")>]
[<M>] type TakeIssueRequest = { id: string; user: string }

type IssueService(handle, loadIssue, loadReportedIssues) =

    interface IService

    member this.Get (req:IssueRequest) =
        loadIssue req.id

    member this.Get (req:ReportedIssuesRequest) =
        loadReportedIssues req.User
    member this.Post (req:ReportRequest) =
        let id = "Issue-" + Guid.NewGuid().ToString("N")
        Issue.Report(req.user, req.summary) 
        |>  handle id
        id

    member this.Post (req:TakeIssueRequest) =
        Issue.Take(req.user, DateTime.Now)
        |> handle req.id 
        "trololo"

    // this helps in object configuration and also aids type inference
    // so that code above can be clean of annotations as much as possible
    new() =
        let conn = Common.connect ("localhost", 1113)
        let db = ReadModels.SQL.Schema.GetDataContext()
        IssueService(Issue.handle conn, 
            SQL.loadIssue db,
            SQL.loadReportedIssues db)

