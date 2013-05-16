module IssueTracker.Issue

open System

type User = string
type Summary = string
type Reason = string
type IssueNumber = int
type ReportData = { Reporter: User; Summary: Summary }
type TakeData = { User: User; Time: DateTime }

type IssueState =
  | Reported of ReportData
  | Logged of IssueNumber
  | Active of User * DateTime
  | Closed of DateTime
  | Cancelled of Reason

type Issue = { State: IssueState list }

let zero = { State = [] }

type Event =
  | Reported of ReportData
  | Logged of IssueNumber
  | Taken of User * DateTime
  | Closed of DateTime
  | Cancelled of Reason

type Command =
  | Report of User * Summary
  | LogIssue of IssueNumber
  | Take of User * DateTime
  | Close of DateTime
  | Cancel of Reason


let apply issue =
  function
  | Reported(data) -> { issue with State = [IssueState.Reported(data)]}
  | Logged(num) -> { issue with State = IssueState.Logged(num)::issue.State }
  | Taken(user, time) -> { issue with State = Active(user, time)::issue.State }
  | Closed(time) -> { issue with State = IssueState.Closed(time)::issue.State }
  | Cancelled(reason) -> { issue with State = IssueState.Cancelled(reason)::issue.State }
   
let exec { State = state } =
  function
    | Report(user, summary) ->
      match state with
      | [] -> Reported({Reporter=user; Summary=summary})
      | _ -> invalidOp "state"
    | LogIssue(number) ->
      match state with
      | IssueState.Reported(_)::_ -> Logged(number)
      | _ -> invalidOp "state"
    | Take (user, time) -> 
      match state with 
      | IssueState.Logged(_)::_ -> Taken (user, time)
      | _ -> invalidOp "state"
    | Close time -> 
      match state with 
      | Active(_)::_ -> Closed time
      | _ -> invalidOp "state"
    | Cancel reason -> 
      match state with
      | IssueState.Logged(_)::[] | Active(_)::_ -> Cancelled reason
      | _ -> invalidOp "state"
