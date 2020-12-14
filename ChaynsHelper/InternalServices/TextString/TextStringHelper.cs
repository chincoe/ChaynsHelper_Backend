using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TobitLogger.Core;
using TobitLogger.Core.Models;
using TobitTextLib;

namespace ChaynsHelper.InternalServices.TextString
{
    /// <summary>
    /// helper to get textstrings
    /// </summary>
    public class TextStringHelper : ITextStringHelper
    {
        private readonly ILogger<TextStringHelper> _logger;
        private readonly IDictionary<string, TextstringHandler> _handlers;

        public TextStringHelper(ILogger<TextStringHelper> logger, string libName, string prefix)
        {
            _logger = logger;
            _handlers = new Dictionary<string, TextstringHandler>
            {
                {libName, new TextstringHandler(libName, "Ger", prefix ?? "", _logger)}
            };
        }

        public TextStringHelper(ILogger<TextStringHelper> logger, IDictionary<string, TextLibOptions> libs)
        {
            _logger = logger;
            _handlers = new Dictionary<string, TextstringHandler>();
            foreach (var (key, value) in libs)
            {
                _handlers.TryAdd(key, new TextstringHandler(value.LibName, value.Language ?? "Ger", value.Prefix ?? "", _logger));
            }
        }
        
        public TextStringHelper(ILogger<TextStringHelper> logger, IDictionary<int, TextLibOptions> libs)
        {
            _logger = logger;
            _handlers = new Dictionary<string, TextstringHandler>();
            foreach (var (key, value) in libs)
            {
                _handlers.TryAdd(key.ToString(), new TextstringHandler(value.LibName, value.Language ?? "Ger", value.Prefix ?? "", _logger));
            }
        }
        
        public string GetTextString(
            string textString,
            string fallback = "",
            IDictionary<string, string> replacements = null,
            string libName = null,
            bool overridePrefix = false)
        {
            string result = fallback;
            TextstringHandler handler = null;
            try
            {
                handler = libName == null
                    ? _handlers.First().Value
                    : _handlers[libName];
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionData(ex, $"Failed to get textStringHandler for textString {textString}")
                {
                    {"handlerKey", libName},
                    {"handlers", _handlers.Keys},
                    {"stringName", textString},
                    {"overridePrefix", overridePrefix},
                    {"fallback", fallback}
                });
            }
            if (handler != null)
            {
                try
                {
                    result = handler.GetText(textString, overridePrefix);
                }
                catch (Exception ex)
                {
                    _logger.Error(new ExceptionData(ex, $"Failed to load textString {textString}")
                    {
                        {"libKey", libName},
                        {"stringName", textString},
                        {"overridePrefix", overridePrefix},
                        {"fallback", fallback}
                    });
                    result = fallback;
                }
            }

            if (replacements == null) return result;
            foreach (var (search, value) in replacements)
            {
                result = result.Replace(search, value);
            }

            return result;
        }
        
        public string GetTextString(
            string textString,
            string fallback = "",
            IDictionary<string, string> replacements = null,
            int libName = -1,
            bool overridePrefix = false)
        {
            string result = fallback;
            TextstringHandler handler = null;
            try
            {
                handler = libName >= 0
                    ? _handlers.First().Value
                    : _handlers[libName.ToString()];
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionData(ex, $"Failed to get textStringHandler for textString {textString}")
                {
                    {"handlerKey", libName},
                    {"handlers", _handlers.Keys},
                    {"stringName", textString},
                    {"overridePrefix", overridePrefix},
                    {"fallback", fallback}
                });
            }
            if (handler != null)
            {
                try
                {
                    result = handler.GetText(textString, overridePrefix);
                }
                catch (Exception ex)
                {
                    _logger.Error(new ExceptionData(ex, $"Failed to load textString {textString}")
                    {
                        {"libKey", libName},
                        {"stringName", textString},
                        {"overridePrefix", overridePrefix},
                        {"fallback", fallback}
                    });
                    result = fallback;
                }
            }

            if (replacements == null) return result;
            foreach (var (search, value) in replacements)
            {
                result = result.Replace(search, value);
            }

            return result;
        }
    }
}