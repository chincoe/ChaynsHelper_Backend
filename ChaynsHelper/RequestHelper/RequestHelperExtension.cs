using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Chayns.Auth.Shared.Extensions;
using Chayns.Auth.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace ChaynsHelper.RequestHelper
{
    public static class RequestHelperExtension
    {
        public static void AddRequestHelper(this IServiceCollection services)
        {
            services.AddHttpClient<RequestHelper>("RequestHelper", client =>
                client.AddRequestHelperDefaults());
            services.AddSingleton<IRequestHelper, RequestHelper>();
        }

        public static void AddRequestHelperDefaults(this HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", GetDefaultUserAgent());
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
                sb.Append($"/{Assembly.GetEntryAssembly().GetVersion()} ");
            }

            sb.Append(Assembly.GetExecutingAssembly().GetName().Name + "/");
            sb.Append(Assembly.GetExecutingAssembly().GetVersion());

            sb.Append(" (");
            sb.Append(EnvironmentHelper.GetPrettyOsName());
            sb.Append("; " + Environment.MachineName);
            sb.Append("; " + EnvironmentHelper.GetPrettyFrameworkName());
            sb.Append(")");
            return sb.ToString();
        }
    }
}