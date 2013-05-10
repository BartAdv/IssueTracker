namespace IssueTracker.Web

open System
open ServiceStack.ServiceHost
open ServiceStack.WebHost.Endpoints
open Microsoft.FSharp.Reflection
  
open IssueTracker

//Define the Web Services AppHost
type AppHost =
    inherit AppHostBase
    new() = { inherit AppHostBase("Issue Tracker services", typeof<AppHost>.Assembly) }
    override this.Configure container =
        container.RegisterAutoWired<IssueRepository>() |> ignore