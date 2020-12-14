﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TobitLogger.Core;

namespace ChaynsHelper.RequestInfoContextProvider
{
    public static class RequestInfoContextProviderExtension
    {
        public static void UseRequestInfoContext(this IServiceCollection services)
        {
            services.RemoveAll<ILogContextProvider>();
            services.AddSingleton<ILogContextProvider, RequestInfoContextProvider>();
        }
    }
}