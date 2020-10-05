module Server

open System.Json
open System.Net
open System.Net.Sockets
open System.Text
open Fleece
open Messages

let getServerMsg(inSocket: UdpClient) = async {
  let! asyncData = inSocket.ReceiveAsync() |> Async.AwaitTask
  return asyncData.Buffer
}

let run port = async {
  let inEndpoint = IPEndPoint(IPAddress.Any, port)
  let inSocket = new UdpClient(inEndpoint)

  let rec loop(inSocket: UdpClient) (outSocket: UdpClient option) : Async<unit> = async {
      let! msg = getServerMsg inSocket

      let outSocket =
        match outSocket with
        | Some v -> v
        | None ->
            let outSocket = new UdpClient(inSocket.Client.RemoteEndPoint.AddressFamily)
            let outEndpoint = inSocket.Client.RemoteEndPoint :?> IPEndPoint
            outSocket.Connect(outEndpoint)
            outSocket

      let parseResult =
          parseJson (Encoding.ASCII.GetString(msg))
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
          let! _ = outSocket.SendAsync(responseMsg, responseMsg.Length) |> Async.AwaitTask
          printfn "SERVER: Sending response: id = %s, state = %s" r.Id r.State
          outSocket.Close()
          ()
      return! loop inSocket (Some outSocket)
  }

  return! loop inSocket None
}
