using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ChaynsHelper.RequestHelper
{
    public static class RequestHelperExtension
    {
        public static void AddRequestHelper(this IServiceCollection services)
        {
            var client = new HttpClient();
            client.AddRequestHelperDefaults();
            services.AddSingleton<IRequestHelper, RequestHelper>();
        }

        public static void AddRequestHelperDefaults(this HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", Assembly.GetEntryAssembly()?.GetName().Name);
        }
    }
}