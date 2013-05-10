namespace IssueTracker.Web

open System
open ServiceStack.ServiceHost
open IssueTracker

[<Route("/user/{User}/issues/reported")>]
[<CLIMutable>]
type ReportedIssuesRequest = { User: string }
[<CLIMutable>] 
type ReportedIssue = { Number: int; Summary: string }

[<Route("/user/{User}/issues/report")>]
[<CLIMutable>]
type ReportRequest = { user: string; summary: string }
[<CLIMutable>]
type ReportResponse = { id: string }

[<Route("/user/{User}/issues/taken")>]
[<CLIMutable>]
type TakenIssuesRequest = { User: string }

[<Route("/issue/{Id}")>]
type IssueRequest = { id: string }

type IssueService(issues: IssueRepository) =
    let save id evt = issues.Save(id, evt)

    interface IService

    member this.Get (req:IssueRequest) =
        let issue = issues.GetIssue(req.id) // normally we'd just use readmodel
        issue

    (*member this.Get (req:ReportedIssuesRequest) =
        loadEvents (fun e -> e.Event.EventType = "Reported") ("IssuesBy-"+req.User)
        |> Seq.map (function 
                    | Issue.Reported({Summary=summary; Number=number}) -> { Number=number; Summary=summary }
                    | _ -> failwith "invalid event type")*)
    member this.Post (req:ReportRequest) =
        let id = "Issue-" + Guid.NewGuid().ToString("N")
        Issue.Report(1, req.user, req.summary) 
        |> Issue.exec Issue.empty
        |> save id
        id

