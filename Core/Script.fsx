#load "Issue.fs"
#load "Aggregate.fs"
#r @"D:\sources\IssueTracker\packages\Newtonsoft.Json.5.0.4\lib\net45\Newtonsoft.Json.dll"
#load "Serialization.fs"
#r @"D:\sources\IssueTracker\packages\EventStore.Client.1.1.0\lib\net40\EventStore.ClientAPI.dll"
#load "EventStore.fs"

open System
open IssueTracker

let aggregate: Aggregate.Aggregate<_,_,_> =
 {zero = { Issue.State = [] };
  apply = Issue.apply;
  exec = Issue.exec }

let connection = 
  System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
  |> EventStore.conn

let handle = Aggregate.handle (EventStore.commit connection "Issue" Serialization.serialize) aggregate

let id1 = Guid.NewGuid().ToString("N")
let issue1 = Issue.Report(1, "user2", "brelam brelam") |> handle id1 aggregate.zero
let issue1' = Issue.Cancel("feee") |> handle id1 issue1
  
let id2 = Guid.NewGuid().ToString("N")
let issue2 = Issue.Report(2, "user2", "good desc") |> handle id2 aggregate.zero
let issue2'= Issue.Take("analyst", DateTime.Now) |> handle id2 issue2
let issue2'' = Issue.Close(DateTime.Now) |> handle id2 issue2'

let id3 = Guid.NewGuid().ToString("N")
let issue3 = Issue.Report(3, "user", "woosh") |> handle id3 aggregate.zero