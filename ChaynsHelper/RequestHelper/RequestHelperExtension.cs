using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChaynsHelper.RequestHelper
{
    public static class RequestHelperExtension
    {
        public static void AddRequestHelper(this IServiceCollection services)
        {
            services.AddHttpClient<IHttpClientFactory>("RequestHelper", client =>
                client.AddRequestHelperDefaults());
            services.AddSingleton<IRequestHelper, RequestHelper>();
        }

        public static void AddRequestHelperDefaults(this HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", Assembly.GetEntryAssembly()?.GetName().Name);
        }
    }
}