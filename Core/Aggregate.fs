/// Aggregate framework.
// https://github.com/eulerfx/DDDInventoryItemFSharp
[<RequireQualifiedAccess>]
module IssueTracker.Aggregate

/// Represents an aggregate.
type Aggregate<'TState, 'TCommand, 'TEvent> = {
    
    /// An initial state value.
    zero : 'TState;

    /// Applies an event to a state returning a new state.
    apply : 'TState -> 'TEvent -> 'TState;

    /// Executes a command on a state yielding an event.
    exec : 'TState -> 'TCommand -> 'TEvent;
}

let load events (aggregate:Aggregate<'TState, 'TCommand, 'TEvent>) =
  let events = events |> Seq.cast :> 'TEvent seq
  Seq.fold aggregate.apply aggregate.zero events
    
let handle commit aggregate id state command =
  let event = aggregate.exec state command
  event |> commit id
  aggregate.apply state event   