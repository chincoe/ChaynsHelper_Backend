using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Auth.Abstractions;
using ChaynsHelper.RequestHelper;
using Microsoft.Extensions.Logging;
using TobitLogger.Core;
using TobitLogger.Core.Models;

namespace ChaynsHelper.InternalServices.Push
{
    public class PushHelper : IPushHelper
    {
        private readonly ILogger<PushHelper> _logger;
        private readonly IApiTokenProvider _apiTokenProvider;
        private readonly IRequestHelper _requestHelper;
        
        public PushHelper(
            ILogger<PushHelper> logger,
            IApiTokenProvider apiTokenProvider,
            IRequestHelper requestHelper)
        {
            _logger = logger;
            _apiTokenProvider = apiTokenProvider;
            _requestHelper = requestHelper;
        }

        public async Task SendPushMessageToUser(PushMessage message)
        {
            var token = await _apiTokenProvider.GetToken();
            var result = await _requestHelper.Request(
                "https://webapi.tobit.com/MessageService/message/send",
                HttpMethod.Post,
                $"Bearer {token}",
                message);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                _logger.Info("[PushHelper] Send Push Success", new LogData
                {
                    {"message", message},
                    {"result", result},
                });
            }
            else
            {
                _logger.Error("[PushHelper] Send Push Failed", new LogData
                {
                    {"message", message},
                    {"result", result},
                });  
            }
        }
    }
}