using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChaynsHelper.InternalServices.TextString
{
    public static class TextStringHelperExtension
    {
        public static void AddTextStringHelper(this IServiceCollection services, string libName, string prefix = "")
        {
            services.AddSingleton<ITextStringHelper>(x =>
                new TextStringHelper(x.GetService<ILogger<TextStringHelper>>(), libName, prefix));
        }
        
        public static void AddTextStringHelper(this IServiceCollection services, IDictionary<string, TextLibOptions> libs)
        {
            services.AddSingleton<ITextStringHelper>(x =>
                new TextStringHelper(x.GetService<ILogger<TextStringHelper>>(), libs));
        }
        
        public static void AddTextStringHelper(this IServiceCollection services, IDictionary<int, TextLibOptions> libs)
        {
            services.AddSingleton<ITextStringHelper>(x =>
                new TextStringHelper(x.GetService<ILogger<TextStringHelper>>(), libs));
        }
    }
}