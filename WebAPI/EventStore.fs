module IssueTracker.Web.EventStore

open System
open EventStore.ClientAPI

// TODO: config version
let connect (ip, port) =
    let c = EventStoreConnection.Create()
    c.Connect(Net.IPEndPoint(Net.IPAddress.Parse("127.0.0.1"), 1113))
    c

let save (conn:EventStoreConnection) serialize id e =
    let eventType,data = serialize e
    let metaData = [||] : byte array
    let eventData = new EventData(Guid.NewGuid(), eventType, true, data, metaData)
    conn.AppendToStream(id, ExpectedVersion.Any, eventData)

let load (conn:EventStoreConnection) deserialize id =
    let slice = conn.ReadStreamEventsForward(id, 1, Int32.MaxValue, true)
    slice.Events
    |> Seq.map (fun e -> deserialize (e.Event.EventType, e.Event.Data))
    