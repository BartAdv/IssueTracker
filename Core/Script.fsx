#load "Issue.fs"
#r @"D:\sources\IssueTracker\packages\Newtonsoft.Json.5.0.4\lib\net45\Newtonsoft.Json.dll"
#load "Serialization.fs"
#r @"D:\sources\IssueTracker\packages\EventStore.Client.1.1.0\lib\net40\EventStore.ClientAPI.dll"

open System
open IssueTracker
open Microsoft.FSharp.Reflection

let aggregate: Aggregate.Aggregate<_,_,_> =
 {zero = { Issue.State = [] };
  apply = Issue.apply;
  exec = Issue.exec }

let connection = 
  System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
  |> EventStore.conn

let handle id state =
  Aggregate.handle (EventStore.commit connection Serialization.serialize) aggregate ("Issue-"+id) state

let loadEvents pred streamId = EventStore.load connection Serialization.deserialize pred streamId

let loadIssue id = 
  let events : seq<Issue.Event> = (loadEvents (fun _ -> true) ("Issue-"+id))
  Aggregate.load events aggregate

let loadReportedIssues user =
  let events : seq<Issue.Event> = loadEvents (fun e -> e.Event.EventType = "Reported") ("IssuesBy-"+user)
  events |> Seq.map (function Issue.Reported({Summary=summary; Number=number}) -> number, summary | _ -> failwith "invalid event type")

let (|>>) a b = (b,a) 

let report (num, user, summary) =
  let id = Guid.NewGuid().ToString("N")
  Issue.Report(num, user, summary)
  |> handle id aggregate.zero
  |>> id

let take user (id, state) =
  Issue.Take(user, DateTime.Now)
  |> handle id state
  |>> id

let close (id, state) =
  Issue.Close(DateTime.Now)
  |> handle id state
  |>> id

let cancel reason (id, state) =
  Issue.Cancel(reason)
  |> handle id state
  |>> id


report (1, "user2", "brelam brelam")
|> take "dev"
|> cancel("feee")

//let sub = connection.SubscribeToAll(true, fun sub (evt:EventStore.ClientAPI.ResolvedEvent) -> printfn "%s" evt.Event.EventType)
//sub.RunSynchronously()