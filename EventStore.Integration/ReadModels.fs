namespace IssueTracker.ReadModels

open System
open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

open IssueTracker.ReadModels
open IssueTracker.ReadModels.IssueReadModel

type IReadModels =
    abstract member GetPosition : string -> int
    abstract member GetIssue : string -> IssueReadModel
    abstract member UpdateIssue : string*int*IssueReadModel -> unit
    abstract member Save : unit -> unit
    
type Schema = SqlDataConnection<"Data Source=localhost\SQLExpress;Initial Catalog=IssueTracker;Integrated Security=SSPI;">
type DataContext = Schema.ServiceTypes.SimpleDataContextTypes.IssueTracker

type SqlReadModels() =
    let db = Schema.GetDataContext()

    let store (db:DataContext) (id:string) (issue:IssueReadModel) =
        let id = Guid(id.Replace("Issue-",""))
        let mutable record = query { for issue in db.Issue do where (issue.Id = id); exactlyOneOrDefault }
        if record = null then 
            record <- Schema.ServiceTypes.Issue(Id=id)
            db.Issue.InsertOnSubmit(record)
        record.Reporter <- issue.Reporter
        record.Summary <- issue.Summary
    
    let storePosition (db:DataContext) name pos =
        let mutable record = query { for pos in db.Position do exactlyOneOrDefault }
        if record = null then 
            record <- Schema.ServiceTypes.Position(Type=name)
            db.Position.InsertOnSubmit(record)
        record.Position1 <- pos    

    interface IReadModels with
        member __.GetPosition(type':string) =
            let p = query {
                for pos in db.Position do
                where (pos.Type = type')
                exactlyOneOrDefault }
            if p = null then 0
            else p.Position1

        member __.GetIssue(id:string) =
            let id = Guid(id.Replace("Issue-", ""))
            let res = query {
                        for issue in db.Issue do
                        where (issue.Id = id)
                        select issue
                        exactlyOneOrDefault }
            // ugh
            if res = null then
              { Reporter = "";
                Summary = "";
                Status = "";
                TakenBy = "";
                TakenOn = DateTime.MinValue }
            else
              { Reporter = res.Reporter;
                Summary = res.Summary;
                Status = "";
                TakenBy = "";
                TakenOn = DateTime.MinValue }
 
        member this.UpdateIssue(id:string, position:int, issue:IssueReadModel) =
            printfn "Updating issue: %s" id
            store db id issue
            storePosition db "Issue" position

        member __.Save() =
            db.DataContext.SubmitChanges()
