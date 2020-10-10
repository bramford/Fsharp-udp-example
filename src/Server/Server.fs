module Server

open System.Json
open System.Net
open System.Net.Sockets
open System.Text
open Fleece
open Lib

let getServerMsg(inSocket: UdpClient) = async {
  let! asyncData = inSocket.ReceiveAsync() |> Async.AwaitTask
  return asyncData
}

let run port = async {
  let inEndpoint = IPEndPoint(IPAddress.Any, port)
  let inSocket = new UdpClient(inEndpoint)

  let rec loop(inSocket: UdpClient) : Async<unit> = async {
      let! msg = getServerMsg inSocket

      let parseResult =
          parseJson (Encoding.ASCII.GetString(msg.Buffer))
      match parseResult with
      | Error e ->
          printf "SERVER: Failed parsing: %s\n" e
          ()
      | Ok r ->
          printfn "SERVER: Received message: id = %s, state = %s" r.Id r.State
          let responseMsg = Encoding.ASCII.GetBytes((toJson {
                        Connect.Id = r.Id
                        State = "received"
                      }).ToString())
          try
            printfn "SERVER: Sending response: id = %s, state = %s" r.Id r.State
            let! result = inSocket.SendAsync(responseMsg, responseMsg.Length, msg.RemoteEndPoint) |> Async.AwaitTask
            if (result = 1) then 
              printfn "SERVER: Successfully sent response"
            else
              printfn "SERVER: Failed to send response"
            ()
          with _ ->
            printfn "SERVER: No remote socket, can't send response"
            ()
      return! loop inSocket
  }

  return! loop inSocket
}

[<EntryPoint>]
let main(argv :string[]) =
  [
    run 3000;
  ]
  |> Async.Parallel
  |> Async.Ignore
  |> Async.RunSynchronously
  0
