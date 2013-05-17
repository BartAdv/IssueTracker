module IssueTracker.ReadModels.IssueReadModel

open System
open IssueTracker
open IssueTracker.Issue

type IssueReadModel = 
  { Reporter: string;
    Number: int;
    Summary: string;
    Status: string;
    TakenBy: string;
    TakenOn: Nullable<DateTime>;
    ClosedOn:Nullable<DateTime>;
    CancellationReason: string; }

let apply state = function
    | Reported({Reporter=reporter; Summary=summary}) -> 
        { state with Reporter=reporter; Summary=summary; Status="Reported"}
    | Logged(number) ->
        { state with Number=number }
    | Taken(user, time) ->
        { state with TakenBy=user; TakenOn=Nullable(time); Status="Active" }
    | Closed(time) ->
        { state with Status="Closed"; }
    | Cancelled(reason) ->
        { state with Status="Cancelled"; }

