using System.Collections.Generic;
using System.Linq;
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
        
        public static void AddTextStringHelper<T>(this IServiceCollection services, IDictionary<T, TextLibOptions> libs)
        {
            var libParam = new Dictionary<string, TextLibOptions>();
            foreach (var (key, value) in libs)
            {
                libParam.TryAdd(key.ToString(), value);
            }
            services.AddSingleton<ITextStringHelper>(x =>
                new TextStringHelper(x.GetService<ILogger<TextStringHelper>>(), libParam));
        }
    }
}