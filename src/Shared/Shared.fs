namespace Shared

open System

type Link ={
    Name:string
    Url:string
    Description:string
    }
type Category={
        Name:string
        Contents:Link list
        }
type LinkConfig = { PublicData: Category list; AdminData:Category list; }
module LinkConfig=

    let create () =
        { PublicData= []; AdminData=[]; }


module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ILinksAPI =
    { getLinks : unit -> Async<LinkConfig>
      setLinks : LinkConfig -> Async<unit> }