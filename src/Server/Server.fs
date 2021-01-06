module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared
open System.IO
open Thoth.Json.Net
type Storage() =
    let linksConfig = Shared.LinkConfig.create ()


    member __.GetLinks() =
        if File.Exists("./links.json") then
            let json = File.ReadAllText("./links.json")
            printfn "read config from file"
            match Decode.Auto.fromString<LinkConfig> (json) with
            | Ok (x) -> x
            | Error (x) ->
                printfn "decoding  json error:%s" x
                LinkConfig.create ()
        else
            File.Create("./links.json").Dispose()
            LinkConfig.create()


    member __.SetConfig(links: LinkConfig) =
        try
            let json =
                Encode.Auto.toString (4, links)
            printfn "writing config to file"
            File.WriteAllText("./links.json", json)
        with| e-> printfn"writing config to file failed with :%A" e

let storage = Storage()


let linksApi =
    { getLinks = fun () -> async { return storage.GetLinks() }
      setLinks = fun linkConf -> async { storage.SetConfig linkConf } }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue linksApi
    |> Remoting.withDiagnosticsLogger (printfn "%s")
    |> Remoting.buildHttpHandler
let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
