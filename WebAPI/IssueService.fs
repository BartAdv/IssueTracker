namespace IssueTracker.Web

open System
open ServiceStack.ServiceHost
open IssueTracker
open IssueTracker.Web
open IssueTracker.ReadModels

type M = CLIMutableAttribute

[<Route("/user/{User}/issues/reported")>]
[<M>] type ReportedIssuesRequest = { User: string }
[<M>] type ReportedIssue = { Number: int; Summary: string }

[<Route("/user/{User}/issues/report")>]
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
        handle id (fun issue ->
            Issue.Report(1, req.user, req.summary) 
            |> Issue.exec issue)
        id
    member this.Post (req:TakeIssueRequest) =
        handle req.id (fun issue ->
            Issue.Take(req.user, DateTime.Now)
            |> Issue.exec issue)
        "trololo"

    // this helps in object configuration and also aids type inference
    // so that code above can be clean of annotations as much as possible
    new() =
        let conn = EventStore.connect ("localhost", 1113)
        let load = EventStore.load conn Serialization.deserialize
        let save = EventStore.save conn Serialization.serialize
        let db = SQL.Schema.GetDataContext()
        let handleIssue id f =
            let issue = load id |> Seq.fold Issue.apply Issue.zero
            f issue |> save id

        IssueService(handleIssue, 
            SQL.loadIssue db,
            SQL.loadReportedIssues db)

