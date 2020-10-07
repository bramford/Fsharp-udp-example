# FSharp UDP Example

## Environment

1. [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Build

```
dotnet build
```

## Run

### Client

To test the UDP client, start a _netcat_ server before running the program:

```sh
cd Client
nc -lu 127.0.0.1 3000 &
dotnet run
```

### Server

To test the UDP server, start the server and then use a _netcat_ udp client to send valid JSON:

```sh
cd Server
dotnet run &
echo -n '{"type": "connect", "id": "$(uuidgen)"}' | nc -u 127.0.0.1 3001
```
