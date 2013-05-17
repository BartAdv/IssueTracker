module IssueTracker.EventStore.Integration.Program

open System
open EventStore.ClientAPI

open IssueTracker
open IssueTracker.ReadModels

[<EntryPoint>]
let main argv =   
    let db = SQL.Schema.GetDataContext()

    let conn = EventStoreConnection.Create()
    conn.Connect(Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113))

    let db = SQL.Schema.GetDataContext()

    let issueCounter = ref 0
    Events.IssueUpdated
    |> Event.add (function 
        | Issue.Event.Reported(_), id -> 
            Issue.handleF conn id (fun issue ->
                match issue.State with
                | Issue.IssueState.Reported(_)::[] -> 
                    Issue.LogIssue(!issueCounter + 1)
                    |> Issue.exec issue
                    |> Some
                | _ -> None)
            issueCounter := !issueCounter + 1
        | _,_-> ())

    let from = ReadModels.SQL.loadPosition db "Issue"
    let sub = conn.SubscribeToStreamFrom("$ce-Issue", Nullable<int>(from), true,
                fun sub (re:ResolvedEvent) -> 
                    lock sub (fun () ->
                        if not (re.Event.EventType.StartsWith("$")) then
                            let evt = Serialization.deserialize (re.Event.EventType, re.Event.Data)
                            let issue = ReadModels.SQL.loadIssue db re.Event.EventStreamId |> IssueReadModel.apply <| evt
                            ReadModels.SQL.storeIssue db re.Event.EventStreamId issue
                            ReadModels.SQL.storePosition db "Issue" re.OriginalEventNumber
                            db.DataContext.SubmitChanges()))

    
    sub.Start()
    Events.start conn
    Threading.Thread.Sleep(TimeSpan.FromHours(1.0))
    0