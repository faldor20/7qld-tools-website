module App

open Elmish
open Elmish.React
open Browser.Dom
open Fable.Core.JsInterop
#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif
importAll "./styles/global.scss"

Program.mkProgram Index.init Index.update Index.view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
