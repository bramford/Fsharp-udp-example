module Main

open Client
open System

let address = "127.0.0.1"
let port = 3000 

[<EntryPoint>]
let main (argv :string[]) =
  [
    Server.run port;
    Client.run address port;
  ]
  |> Async.Parallel
  |> Async.Ignore
  |> Async.RunSynchronously
  0
