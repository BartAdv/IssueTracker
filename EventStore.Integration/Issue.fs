module IssueTracker.EventStore.Integration.Issue

open IssueTracker

let load conn = Common.load conn Serialization.deserialize
let save conn = Common.save conn Serialization.serialize
  
let handleF conn id f =
    let issue = load conn id |> Seq.fold Issue.apply Issue.zero
    match f issue with
    | Some(evt) -> evt |> save conn id
    | None -> ()

let handle conn id comm = 
    handleF conn id (fun issue -> comm |> Issue.exec issue |> Some)