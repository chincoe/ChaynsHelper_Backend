using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChaynsHelper.InternalServices.Websocket
{
    public static class WebsocketHelperExtension
    {
        public static void AddWebsocketHelper(this IServiceCollection services, string keyId, string key)
        {
            services.AddSingleton<IWebsocketHelper>(x =>
                new WebsocketHelper(x.GetService<ILogger<WebsocketHelper>>(), keyId, key));
        }
    }
}