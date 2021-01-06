
module MyComponents.LinkList
open Shared
open Feliz
open System
open Elmish
open MyComponents.Link
open Feliz.Bulma

type LLState = Category list

type LLMsg =
    | ChangeData of Category list

//===utility===
let dummyData=
    [ { Name = "Quantel"
        Contents =
            [ { Name = "File Flow Queue"
                Url = "http://10.41.57.30/fileflowqueue/"
                Description="For monitoring of file trasnfer between offices and export and import jobs to and from Quantel" } ] };
       {Name="Empty1";Contents=[]};
       {Name="Empty2";Contents=[]}
       ]



//=====Elmish==
let init() = List.empty


let render (state: LLState)  =
    Html.div [
        prop.style[
            style.maxWidth (length.rem 60)
        ]
        prop.children [

            Html.unorderedList
                (state
                |>List.map(
                    fun {Name=name;Contents=contents}->
                    Html.li[
                    Html.h3 name
                    Html.unorderedList (contents|>List.map(fun {Name= linkName; Url=url; Description=desc;}->
                        Html.li[
                            Bulma.Bulma.box[
                                Bulma.levelLeft[
                                    link linkName url
                                    Html.p[prop.className "ml-3"; prop.text desc]
                                ]
                            ]
                            ]))
                    ]

                    ))
        ]

    ]