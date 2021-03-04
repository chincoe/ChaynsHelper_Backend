using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Chayns.Errors.Handling.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using AssemblyExtensions = Chayns.Auth.Shared.Extensions.AssemblyExtensions;
using EnvironmentHelper = Chayns.Auth.Shared.Helpers.EnvironmentHelper;

namespace ChaynsHelper.RequestHelper
{
    public static class RequestHelperExtension
    {
        public static void AddRequestHelper(this IServiceCollection services)
        {
            services.AddRequestHelperHttpClient();
        }
        
        /// <summary>
        /// Add a request helper using an http client
        /// </summary>
        /// <param name="services"></param>
        public static void AddRequestHelperHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<RequestHelper>("RequestHelper", client =>
                client.AddRequestHelperDefaults());
            services.AddSingleton<IRequestHelper>(x =>
                new RequestHelper(x.GetService<ILogger<RequestHelper>>(), x.GetService<IHttpClientFactory>()));
        }
        
        /// <summary>
        /// Add a request helper using a chayns errors web client
        /// </summary>
        /// <param name="services"></param>
        public static void AddRequestHelperWebClient(this IServiceCollection services)
        {
            services.TryAddSingleton<IWebClientFactory, WebClientFactory>();
            services.AddSingleton<IRequestHelper>(x =>
                new RequestHelper(x.GetService<ILogger<RequestHelper>>(), x.GetService<IHttpClientFactory>()));
        }

        public static void AddRequestHelperDefaults(this HttpClient client)
        {
            if (client.DefaultRequestHeaders.UserAgent == default)
            {
                client.DefaultRequestHeaders.Add("User-Agent", GetDefaultUserAgent());
            }
        }

        /// <summary>
        /// Create a default user-agent based on current assembly and version
        /// </summary>
        /// <returns>String in format "ApplicationName/1.0 (Microsoft Windows 10; SERVER-01; .NET Core 3.1)"</returns>
        private static string GetDefaultUserAgent()
        {
            var sb = new StringBuilder();

            var projectName = Assembly.GetEntryAssembly()?.GetName().Name;

            if (projectName != null)
            {
                sb.Append(projectName);
                sb.Append($"/{AssemblyExtensions.GetVersion(Assembly.GetEntryAssembly())} ");
            }

            sb.Append(Assembly.GetExecutingAssembly().GetName().Name + "/");
            sb.Append(AssemblyExtensions.GetVersion(Assembly.GetExecutingAssembly()));

            sb.Append(" (");
            sb.Append(EnvironmentHelper.GetPrettyOsName());
            sb.Append("; " + Environment.MachineName);
            sb.Append("; " + EnvironmentHelper.GetPrettyFrameworkName());
            sb.Append(")");
            return sb.ToString();
        }
    }
}