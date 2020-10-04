# FSharp UDP Example

## Environment

1. [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Build & Run

```
dotnet run
```

## Test

### Client

To test the UDP client, have a _netcat_ server up and running before you start this program:

```sh
nc -lu 127.0.0.1 3000
```

### Server

To test the UDP server, use the commonly found _netcat_ udp client to send valid JSON:

```sh
echo -n '{"type": "msg", "msg": "Hello World"}' | nc -u 127.0.0.1 3001
```
