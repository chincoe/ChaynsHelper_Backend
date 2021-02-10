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
        new {
            boardId = 3,
            tappId = 12345,
            userId = new MultipleCondition(ConditionType.OneOf, new[]{123, 456, 789})
        }
    );
    // or
    await _websocket.SendWebsocketMessage(
        "my_topic",
        myWebsocketData,
        new Condition("boardId", 3)
            .Add("tappId", 12345)
            .AddMultiple("userId", ConditionType.OneOf, new[]{123, 456, 789})
    );
}

```