using System.Threading.Tasks;

namespace ChaynsHelper.InternalServices.Websocket
{
    public interface IWebsocketHelper
    {
        Task SendWebsocketMessage(string topic, object data, object conditions);
    }
}