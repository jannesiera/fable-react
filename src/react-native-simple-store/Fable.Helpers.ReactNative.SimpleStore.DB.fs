module Fable.Helpers.ReactNativeSimpleStore.DB

open System
open Fable.Import.ReactNative
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.PowerPack

[<Literal>]
let private modelsKey = "models/"
type Table<'a> = 'a[]

let inline private setItem(key, s): JS.Promise<unit> =
    unbox(Globals.AsyncStorage.setItem(key,s))

/// Removes all rows from the model.
let clear<'a>() =
    let key = modelsKey + typeof<'a>.FullName
    let s:string = [||] |> toJson
    setItem(key,s)

/// Gets or creates a new model.
let [<PassGenerics>] private getModel<'a> (key) : JS.Promise<Table<'a>> =
    Globals.AsyncStorage.getItem (key)
    |> Promise.map (function
        | null -> [||]
        | v -> ofJson v)

/// Adds a row to a model
let [<PassGenerics>] add<'a>(data:'a) =
    let key = modelsKey + typeof<'a>.FullName
    getModel<'a> key
    |> Promise.bind (fun model ->
        let newModel : string = Array.append [|unbox data|] model |> toJson
        setItem(key,newModel))

/// Updates a row in a model
let [<PassGenerics>] update<'a>(index, data:'a) =
    let key = modelsKey + typeof<'a>.FullName
    getModel<'a> key
    |> Promise.bind (fun model ->
        model.[index] <- unbox data
        let newModel : string =  toJson model
        setItem(key,newModel))

/// Deletes a row from a model
let [<PassGenerics>] delete<'a>(index) =
    let key = modelsKey + typeof<'a>.FullName
    getModel<'a> key
    |> Promise.bind (fun model ->
        let model : 'a[] =
            model
            |> Array.mapi (fun i x -> i,x)
            |> Array.filter (fun (i,_) -> i <> index)
            |> Array.map snd
        let newModel : string =  toJson model
        setItem(key,newModel))

/// Updates multiple rows in a model
let [<PassGenerics>] updateMultiple<'a>(values) =
    let key = modelsKey + typeof<'a>.FullName
    getModel<'a> key
    |> Promise.bind (fun model ->
        for index, data:'a in values do
            model.[index] <- unbox data
        let newModel : string =  toJson model
        setItem(key,newModel))

///  Update data by an update function.
let [<PassGenerics>] updateWithFunction<'a>(updateF: 'a[] -> 'a[]) =
    let key = modelsKey + typeof<'a>.FullName
    getModel<'a> key
    |> Promise.bind (fun model ->
        let updated = updateF model
        let newModel : string = toJson updated
        setItem(key,newModel))

///  Update data by an update function.
let [<PassGenerics>] updateWithFunctionAndKey<'a>(updateF: 'a[] -> 'a[],key) =
    let key = modelsKey + typeof<'a>.FullName + "/" + key
    getModel<'a> key
    |> Promise.bind (fun model ->
        let updated = updateF model
        let newModel : string = toJson updated
        setItem(key,newModel))

/// Adds multiple rows to a model
let [<PassGenerics>] addMultiple<'a>(data:'a []) =
    let key = modelsKey + typeof<'a>.FullName
    getModel<'a> key
    |> Promise.bind (fun model ->
        let newModel : string = Array.append data model |> toJson
        setItem(key,newModel))

/// Replaces all rows of a model
let replaceWithKey<'a>(key,data:'a []) =
    let modelKey = modelsKey + typeof<'a>.FullName + "/" + key
    let newModel : string = data |> toJson
    setItem(modelKey,newModel)

/// Replaces all rows of a model
let replace<'a>(data:'a []) =
    let modelKey = modelsKey + typeof<'a>.FullName
    let newModel : string = data |> toJson
    setItem(modelKey,newModel)

/// Gets a row from the model
let [<PassGenerics>] get<'a>(index:int) =
    let key = modelsKey + typeof<'a>.FullName
    getModel<'a> key
    |> Promise.map (fun model -> model.[index])

/// Gets all rows from the model
let [<PassGenerics>] getAll<'a>() =
    let key = modelsKey + typeof<'a>.FullName
    getModel<'a> key

/// Gets all rows from the model
let [<PassGenerics>] getAllWithKey<'a>(key) =
    let key = modelsKey + typeof<'a>.FullName + "/" + key
    getModel<'a> key

/// Gets the row count from the model
let [<PassGenerics>] count<'a>() =
    let key = modelsKey + typeof<'a>.FullName
    getModel<'a> key
    |> Promise.map (fun model -> model.Length)