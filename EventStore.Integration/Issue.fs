﻿module IssueTracker.EventStore.Integration.Issue

open IssueTracker

let load conn = Common.load conn Serialization.deserialize
let save conn = Common.save conn Serialization.serialize
  
let handle conn id f =
    let issue = load conn id |> Seq.fold Issue.apply Issue.zero
    f issue |> save conn id

