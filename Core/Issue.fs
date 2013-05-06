module IssueTracker.Issue

open System

type User = string
type Summary = string
type Reason = string

type ReportData = { Number: int; Reporter: User; Summary: Summary }

type IssueState =
  | Logged of ReportData
  | Active of User * DateTime
  | Closed of DateTime
  | Cancelled of Reason

type Issue = { State: IssueState list }

type Event =
  | Reported of ReportData
  | Taken of User * DateTime
  | Closed of DateTime
  | Cancelled of Reason

type Command =
  | Report of int * User * Summary
  | Take of User * DateTime
  | Close of DateTime
  | Cancel of Reason


let apply issue =
  function
  | Reported(data) -> { issue with State = [Logged(data)]}
  | Taken(user, time) -> { issue with State = Active(user, time)::issue.State }
  | Closed(time) -> { issue with State = IssueState.Closed(time)::issue.State }
  | Cancelled(reason) -> { issue with State = IssueState.Cancelled(reason)::issue.State }
   
let exec { State = state } =
  function
    | Report(number, user, summary) ->
      match state with
      | [] -> Reported({Number=number; Reporter=user; Summary=summary})
      | _ -> invalidOp "state"
    | Take (user, time) -> 
      match state with 
      | Logged(_)::_ -> Taken (user, time)
      | _ -> invalidOp "state"
    | Close time -> 
      match state with 
      | Active(_)::_ -> Closed time
      | _ -> invalidOp "state"
    | Cancel reason -> 
      match state with
      | Logged(_)::[] | Active(_,_)::_ -> Cancelled reason
      | _ -> invalidOp "state"
