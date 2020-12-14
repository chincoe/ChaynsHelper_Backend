using Newtonsoft.Json;

namespace ChaynsHelper.InternalServices.Websocket
{
    public class WebSocketBody
    {
        [JsonProperty("topic")] public string Topic { get; set; }
        [JsonProperty("data")] public object Data { get; set; }
        [JsonProperty("conditions")] public object Conditions { get; set; }
    }
}