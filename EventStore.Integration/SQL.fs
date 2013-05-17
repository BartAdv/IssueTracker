module IssueTracker.ReadModels.SQL

open System
open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

open IssueTracker.ReadModels
open IssueTracker.ReadModels.IssueReadModel
    
type Schema = SqlDataConnection<"Data Source=localhost\SQLExpress;Initial Catalog=IssueTracker;Integrated Security=SSPI;">
type DataContext = Schema.ServiceTypes.SimpleDataContextTypes.IssueTracker

let n2o (v:Nullable<'a>) = if v.HasValue then Some(v.Value) else None
let o2n o = match o with Some(v) -> Nullable(v) | None -> Nullable()

let mapIssue (res:Schema.ServiceTypes.Issue) =
    // ugh
    if res = null then
      { Reporter = ""
        Number = 0
        Summary = ""
        Status = ""
        TakenBy = ""
        TakenOn = Nullable()
        ClosedOn = Nullable()
        CancellationReason = "" }
    else
      { Reporter = res.Reporter
        Number = res.Number.GetValueOrDefault()
        Summary = res.Summary
        Status = res.Status
        TakenBy = res.TakenBy
        TakenOn = res.TakenOn
        ClosedOn = res.ClosedOn
        CancellationReason = res.CancellationReason }

let loadIssue (db:DataContext) (id:string) =
    let id = Guid(id.Replace("Issue-", ""))
    let res = query {
                for issue in db.Issue do
                where (issue.Id = id)
                select issue
                exactlyOneOrDefault }
    mapIssue res

let loadReportedIssues (db:DataContext) (user:string) =
    query {
        for issue in db.Issue do
        where (issue.Reporter = user)
    } |> Seq.map mapIssue

let storeIssue (db:DataContext) (id:string) (issue:IssueReadModel) =
    let id = Guid(id.Replace("Issue-",""))
    let mutable record = query { for issue in db.Issue do where (issue.Id = id); exactlyOneOrDefault }
    if record = null then 
        record <- Schema.ServiceTypes.Issue(Id=id)
        db.Issue.InsertOnSubmit(record)
    record.Reporter <- issue.Reporter
    record.Number <- Nullable(issue.Number)
    record.Summary <- issue.Summary
    record.Status <- issue.Status
    record.TakenBy <- issue.TakenBy
    record.TakenOn <- issue.TakenOn
    record.ClosedOn <- issue.ClosedOn
    record.CancellationReason <- issue.CancellationReason

let storePosition (db:DataContext) name pos =
    let mutable record = query { for pos in db.Position do exactlyOneOrDefault }
    if record = null then 
        record <- Schema.ServiceTypes.Position(Type=name)
        db.Position.InsertOnSubmit(record)
    record.Position1 <- pos    

let loadPosition (db:DataContext) type' =
    let p = query {
        for pos in db.Position do
        where (pos.Type = type')
        exactlyOneOrDefault }
    if p = null then 0
    else p.Position1

     