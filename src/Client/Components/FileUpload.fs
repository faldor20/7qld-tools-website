module MyComponents.FileUpload
open Feliz
open Feliz.Bulma



let fileUpload (text: string) (uploadAction:Browser.Types.File->unit) =
    Html.div[
        prop.children[
            Bulma.file[
                Bulma.fileLabel.label[
                    prop.name "hi"
                    prop.type' "file"
                    prop.children[
                        Bulma.fileInput [
                            prop.onChange uploadAction
                            prop.type' "file"
                            prop.name "upload"
                        ]
                        Bulma.fileCta[
                            Bulma.fileLabel.span text
                        ]
                    ]
                ]
            ]               
        ]
    ]