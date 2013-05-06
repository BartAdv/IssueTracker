/// Integration with EventStore.
// https://github.com/eulerfx/DDDInventoryItemFSharp
[<RequireQualifiedAccess>]
module IssueTracker.EventStore

open System
open System.Net
open EventStore.ClientAPI

/// Creates and opens an EventStore connection.
let conn endPoint =   
    let conn = EventStoreConnection.Create() 
    conn.Connect(endPoint)
    conn

let streamId category id = category + "-" + id

let load (conn:EventStoreConnection) category deserialize (t,id) =
  let streamId = streamId category id
  let slice = conn.ReadStreamEventsForward(streamId, 1, Int32.MaxValue, false)
  slice.Events |> Seq.map (fun e -> deserialize(t, e.Event.EventType, e.Event.Data))

let commit (conn:EventStoreConnection) category serialize id e =
  let streamId = streamId category id
  let eventType,data = serialize e
  let metaData = [||] : byte array
  let eventData = new EventData(Guid.NewGuid(), eventType, true, data, metaData)
  conn.AppendToStream(streamId, ExpectedVersion.Any, eventData)
