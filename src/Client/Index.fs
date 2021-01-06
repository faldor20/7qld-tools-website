module Index
open Shared
open Feliz
open Elmish
open Feliz.Bulma
open MyComponents.Link
open MyComponents
open MyComponents.FileUpload
open Fable
open Thoth.Json
open Fable.SimpleHttp
open Shared
open Fable.Remoting.Client
let getConfigJsonDownload (config:Shared.LinkConfig)=
    let prefix="data:text/json;charset=utf-8,"
    let json=Thoth.Json.Encode.Auto.toString(4, config)
    prefix+json
let getConfigURI config=
    let json= getConfigJsonDownload config
    Fable.Core.JS.encodeURI json
let triggerConfigDownload config=
    let uri=getConfigURI config
    Browser.Dom.window.``open`` uri|>ignore

let LinkConfApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ILinksAPI>

type State = { Count:int ;LinksData: Shared.LinkConfig;FileResult:string; CorrectPassword:bool }

type Msg =
    | Increment
    | Decrement
    | SetLinksData of LinkConfig
    | ConfigUpload of Browser.Types.File
    | ConfigRead of Browser.Types.FileReader
    | ConfgReadFail of Browser.Types.Event
    | DownloadConfig
    | PasswordChange of string
(*     | GetConfigResult of LinkConfig
    | GetConfigError *)
let init() =

    let getLinkConfig =
        async{
            printfn "requesting data from server"
            let! data= LinkConfApi.getLinks()
            return data |> SetLinksData}
    {Count=0; LinksData=LinkConfig.create(); FileResult=""; CorrectPassword=false},
    Cmd.OfAsync.result getLinkConfig

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none
    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none
    | SetLinksData data->{state with LinksData=data},Cmd.none
    |ConfigUpload file->
        let reader=Browser.Dom.FileReader.Create()

        let sub (dispatch:Msg->unit)=
            reader.onload <-(fun x->(ConfigRead reader) |>dispatch )
            reader.onerror<-ConfgReadFail >>dispatch
            reader.readAsText(file)
        state,(Cmd.ofSub sub)
    |ConfigRead reader->
        let file=reader.result :?> string
        let res= Thoth.Json.Decode.Auto.fromString<LinkConfig > (file)
        match res with
        |Ok(data)->
        //TODO: this can most likely be removed and the data can be returned as the new satte directly

            //this should call a method on the server
            LinkConfApi.setLinks data|>Async.Start

            { state with LinksData=data;FileResult="Read new config data" },Cmd.none
        |Error(x)->
            let err= sprintf "failed desrialising uploaded file. Reason: %s" x
            printfn "%s" err
            {state with FileResult= err },Cmd.none
    |DownloadConfig->
        triggerConfigDownload state.LinksData
        state,Cmd.none
    |PasswordChange  enteredPass->
        let correct= enteredPass="5t3w@rd355"
        {state with CorrectPassword=correct },Cmd.none

let main (children:ReactElement list)=
    Bulma.hero[

        prop.style[

            style.backgroundSize "cover"
        ]
        prop.children[
            Bulma.heroBody[
                Bulma.container[
                    Html.div[
                        prop.className "content" //this makes bulma styles apply by defualt
                        prop.style [style.padding (length.rem 2)]
                        prop.children children
                        ]
                ]
            ]
        ]
    ]


let view (state: State) (dispatch: Msg -> unit) =
    main [
        Html.h1 "Link list"
        LinkList.render state.LinksData.PublicData
        Bulma.input.password[
            prop.onTextChange (PasswordChange>>dispatch)
        ]
        if state.CorrectPassword then
            LinkList.render state.LinksData.AdminData
            Divider.divider"_____________________________________________________________________________________________________________"
            Bulma.levelLeft[
                prop.className "gaps"
                prop.children[
                    Bulma.button.a[

                        color.isDanger
                        prop.text "reset List"
                        prop.onClick(fun _-> LinkConfig.create()|>SetLinksData|>dispatch)
                        ]
                    Bulma.button.a[

                        color.isSuccess
                        prop.text "Dummy Data"
                        prop.onClick(fun _-> {state.LinksData with PublicData= LinkList.dummyData}|>SetLinksData|>dispatch)
                        ]
                    Bulma.button.a[
                        color.isInfo
                        prop.text "download config"
                        prop.custom( "download","config.json" )

                        prop.href (getConfigURI state.LinksData)
                        //prop.onClick(fun _ -> DownloadConfig|>dispatch )
                    ]
                    Html.div[
                    Html.div[prop.style[style.position.initial];prop.children[ (fileUpload "upload config file" (fun (x:Browser.Types.File)-> (ConfigUpload(x))|>dispatch))]]
                    Html.p[prop.style[style.position.absolute]; prop.text state.FileResult]
                    ]
                ]
            ]
    ]
