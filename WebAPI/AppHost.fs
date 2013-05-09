namespace IssueTracker.Web

open System
open ServiceStack.ServiceHost
open ServiceStack.WebHost.Endpoints
 
[<CLIMutable>]
type Hello = { Name: string; }
[<CLIMutable>]
type HelloResponse = { Result: string; }

type HelloService() =
    interface IService
    member this.Get (req:Hello) = { Result = "Hello, " + req.Name }
 
//Define the Web Services AppHost
type AppHost =
    inherit AppHostBase
    new() = { inherit AppHostBase("Hello F# Services", typeof<HelloService>.Assembly) }
    override this.Configure container =
        base.Routes
            .Add<Hello>("/hello")
            .Add<Hello>("/hello/{Name}") |> ignore
 