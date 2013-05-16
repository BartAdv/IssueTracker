module IssueTracker.EventStore.Integration.Events

open System
open IssueTracker
open EventStore.ClientAPI

let issueUpdated = Event<Issue.Event * string>()
let IssueUpdated = issueUpdated.Publish

let start (conn:EventStoreConnection) =
    let sub = conn.SubscribeToStreamFrom("$ce-Issue", Nullable<int>(), true,
                fun sub (re:ResolvedEvent) -> 
                    lock sub (fun () ->
                    if not (re.Event.EventType.StartsWith("$")) then
                            let evt = Serialization.deserialize (re.Event.EventType, re.Event.Data)
                            issueUpdated.Trigger(evt, re.Event.EventStreamId)))
    sub.Start()
    