module IssueTracker.ReadModels.SQL.EventStore

open System
open EventStore.ClientAPI

open IssueTracker
open IssueTracker.ReadModels

[<EntryPoint>]
let main argv =   
    let db = Schema.GetDataContext()
    let conn = EventStoreConnection.Create()
    conn.Connect(Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113))

    let readModels = SqlReadModels() :> IReadModels
    let from = readModels.GetPosition("Issue")
    let sub = conn.SubscribeToStreamFrom("$ce-Issue", Nullable<int>(from), true,
                fun sub (re:ResolvedEvent) -> 
                    lock sub (fun () ->
                    if not (re.Event.EventType.StartsWith("$")) then
                            let evt = Serialization.deserialize (re.Event.EventType, re.Event.Data)
                            let issue = readModels.GetIssue(re.Event.EventStreamId) |> IssueReadModel.apply <| evt
                            readModels.UpdateIssue(re.Event.EventStreamId, re.OriginalEventNumber, issue)
                            readModels.Save()))
    sub.Start()
    Threading.Thread.Sleep(TimeSpan.FromHours(1.0))
    0