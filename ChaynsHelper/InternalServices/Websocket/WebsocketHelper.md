## WebsocketHelper

### Usage
```c#
// Startup.cs

// get keyId and key from https://tobitdev.chayns.net/WebSocket-ServiceAdministration
services.AddWebsocketHelper("KeyIdjafdsljfasdjlkfa", "keylkjajdhdsoifgual923fh38");

// File

private readonly IWebsocketHelper _websocket;

public MyClass(IWebsocketHelper websocket)
{
    _websocket = websocket;
}

public void MyMethod()
{
    await _websocket.SendWebsocketMessage(
        "my_topic",
        myWebsocketData,
        myWebsocketConditions
    );
}

```