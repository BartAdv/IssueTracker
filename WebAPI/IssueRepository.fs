namespace IssueTracker.Web

open System
open EventStore.ClientAPI
open IssueTracker

type IssueRepository() =
    let conn = 
        let c = EventStoreConnection.Create()
        c.Connect(Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113))
        c

    let load streamId =
        let slice = conn.ReadStreamEventsForward(streamId, 1, Int32.MaxValue, true)
        slice.Events

    let commit (conn:EventStoreConnection) serialize streamId e =
        let eventType,data = serialize e
        let metaData = [||] : byte array
        let eventData = new EventData(Guid.NewGuid(), eventType, true, data, metaData)
        conn.AppendToStream(streamId, ExpectedVersion.Any, eventData)


    let commit id = 
        commit conn Serialization.serialize id

    let loadEvents streamId = 
        load streamId
        |> Seq.map (fun e -> Serialization.deserialize(e.Event.EventType, e.Event.Data))
    
    member __.GetIssue (id) = 
        loadEvents id
        |> Seq.fold Issue.apply { Issue.State = [] }  
    
    member __.Save(id, evt) =
        commit id evt
