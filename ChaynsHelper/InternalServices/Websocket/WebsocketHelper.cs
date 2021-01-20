using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TobitLogger.Core;
using TobitLogger.Core.Models;

namespace ChaynsHelper.InternalServices.Websocket
{
    public class WebsocketHelper: IWebsocketHelper
    {
        private readonly ILogger<WebsocketHelper> _logger;
        private readonly string _auth;

        public WebsocketHelper(
            ILogger<WebsocketHelper> logger,
            string keyId,
            string key)
        {
            _logger = logger;
            _auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{keyId}:{key}"));
        }

        public async Task SendWebsocketMessage(string topic, object data, object conditions)
        {
            if (data is IEnumerable)
            {
                throw new Exception("[WebsocketHelper] WS data cannot be an array or list, but has to be an object");
            }

            var key = $"Basic {_auth}";
            const string url = "https://websocket.tobit.com/message";
            var response = await RequestHelper.RequestHelper.StaticRequest(
                url,
                _logger,
                HttpMethod.Post,
                key,
                new
                {
                    Topic = topic,
                    Data = data ?? new { },
                    Conditions = conditions
                },
                ignoreNullValues: false);
            if (response.IsSuccessStatusCode)
            {
                _logger.Info("[WebsocketHelper] Send Websocket success", new LogData
                {
                    {"topic", topic},
                    {"data", data},
                    {"conditions", conditions}
                });
            }
            else
            {
                _logger.Error("[WebsocketHelper] Send Websocket failure", new LogData
                {
                    {"topic", topic},
                    {"data", data},
                    {"conditions", conditions}
                });  
            }
        }

        public async Task SendWebsocketMessage(string topic, object data, Condition conditions)
        {
            await SendWebsocketMessage(topic, data, (object) conditions);
        }
    }
}