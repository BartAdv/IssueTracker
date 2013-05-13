module IssueTracker.ReadModels.IssueReadModel

open System
open IssueTracker
open IssueTracker.Issue

type IssueReadModel = 
  { Reporter: string;
    Summary: string;
    TakenBy: string;
    TakenOn: DateTime;
    Status: string; }

let apply state = function
    | Reported({Reporter=reporter; Summary=summary}) -> 
        { state with Reporter=reporter; Summary=summary; Status="Reported"}
    | Taken(user, time) ->
        { state with TakenBy=user; TakenOn=time; Status="Active" }
    | Closed(time) ->
        { state with Status="Closed"; }
    | Cancelled(reason) ->
        { state with Status="Cancelled"; }

