using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TobitLogger.Core;
using TobitLogger.Core.Models;

namespace ChaynsHelper.RequestHelper
{
    public class RequestHelper : IRequestHelper
    {
        private readonly ILogger<RequestHelper> _logger;
        private static HttpClient _httpClient = new HttpClient();

        static RequestHelper()
        {
            _httpClient.AddRequestHelperDefaults();
        }

        public RequestHelper(ILogger<RequestHelper> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient("RequestHelper");
        }

        public async Task<Response<string>> Request(
            string url,
            HttpMethod method = null,
            string authorization = null,
            object body = null,
            Dictionary<HttpRequestHeader, string> headers = null,
            bool ignoreNullValues = true,
            bool camelCase = true,
            string mediaType = "application/json",
            IEnumerable<HttpStatusCode> expectedStatusCodes = null,
            string processName = null,
            ILogger logger = null)
        {
            logger = logger ?? _logger;
            processName = processName ?? new StackTrace().GetFrame(7)?.GetMethod()?.Name ?? "HttpRequest";
            return await StaticRequest(
                url,
                method: method,
                authorization: authorization,
                body: body,
                headers: headers,
                ignoreNullValues: ignoreNullValues,
                camelCase: camelCase,
                mediaType: mediaType,
                expectedStatusCodes: expectedStatusCodes,
                processName: processName,
                logger: logger);
        }

        public async Task<Response<T>> Request<T>(
            string url,
            HttpMethod method = null,
            string authorization = null,
            object body = null,
            Dictionary<HttpRequestHeader, string> headers = null,
            bool ignoreNullValues = true,
            bool camelCase = true,
            string mediaType = "application/json",
            IEnumerable<HttpStatusCode> expectedStatusCodes = null,
            string processName = null,
            ILogger logger = null)
        {
            logger = logger ?? _logger;
            processName = processName ?? new StackTrace().GetFrame(7)?.GetMethod()?.Name ?? "HttpRequest";
            return await StaticRequest<T>(
                url,
                method: method,
                authorization: authorization,
                body: body,
                headers: headers,
                ignoreNullValues: ignoreNullValues,
                camelCase: camelCase,
                mediaType: mediaType,
                expectedStatusCodes: expectedStatusCodes,
                processName: processName,
                logger: logger);
        }

        /// <summary>
        /// Send http request. Returns Response Object with body and statusCode
        /// </summary>
        /// <param name="url">The request url</param>
        /// <param name="method">HttpMethod, default GET</param>
        /// <param name="authorization">Authorization string</param>
        /// <param name="body">Body Object. HttpContent: Add as Content. String: Add as StringContent. Other: Json Serialize to StringContent</param>
        /// <param name="headers">Header dictionary</param>
        /// <param name="ignoreNullValues">Ignore null values when JSON serializing, default true</param>
        /// <param name="camelCase">Serialize objects with camelCase names</param>
        /// <param name="mediaType">MediaType for StringContent</param>
        /// <param name="expectedStatusCodes">Only affects logging. Not expected status codes are logged as errors. Default: all success status codes</param>
        /// <param name="processName">Name of the request. Default: calling method name</param>
        /// <param name="logger">logger for the request. Default: null logger that doesn't log anything</param>
        /// <typeparam name="T">Return Type the body string will be deserialized to</typeparam>
        /// <returns>Response with instance of T as body</returns>
        public static async Task<Response<T>> StaticRequest<T>(
            string url,
            ILogger logger,
            HttpMethod method = null,
            string authorization = null,
            object body = null,
            Dictionary<HttpRequestHeader, string> headers = null,
            bool ignoreNullValues = true,
            bool camelCase = true,
            string mediaType = "application/json",
            IEnumerable<HttpStatusCode> expectedStatusCodes = null,
            string processName = null)
        {
            logger = logger ?? new Logger<RequestHelper>(new NullLoggerFactory());
            processName = processName ?? new StackTrace().GetFrame(7)?.GetMethod()?.Name ?? "HttpRequest";
            var response = await StaticRequest(
                url,
                method: method,
                authorization: authorization,
                body: body,
                headers: headers,
                ignoreNullValues: ignoreNullValues,
                camelCase: camelCase,
                mediaType: mediaType,
                expectedStatusCodes: expectedStatusCodes,
                processName: processName,
                logger: logger);
            var result = response?.Body != null
                ? JsonConvert.DeserializeObject<T>(response.Body)
                : default(T);
            return new Response<T>(response, result);
        }

        /// <summary>
        /// Send http request. Returns Response Object with body and statusCode
        /// </summary>
        /// <param name="url">The request url</param>
        /// <param name="method">HttpMethod, default GET</param>
        /// <param name="authorization">Authorization string</param>
        /// <param name="body">Body Object. HttpContent: Add as Content. String: Add as StringContent. Other: Json Serialize to StringContent</param>
        /// <param name="headers">Header dictionary</param>
        /// <param name="ignoreNullValues">Ignore null values when JSON serializing, default true</param>
        /// <param name="camelCase">Serialize objects with camelCase names</param>
        /// <param name="mediaType">MediaType for StringContent</param>
        /// <param name="expectedStatusCodes">Only affects logging. Not expected status codes are logged as errors. Default: all success status codes</param>
        /// <param name="processName">Name of the request. Default: calling method name</param>
        /// <param name="logger">logger for the request. Default: null logger that doesn't log anything</param>
        /// <returns>Response of string</returns>
        public static async Task<Response<string>> StaticRequest(
            string url,
            ILogger logger,
            HttpMethod method = null,
            string authorization = null,
            object body = null,
            Dictionary<HttpRequestHeader, string> headers = null,
            bool ignoreNullValues = true,
            bool camelCase = true,
            string mediaType = "application/json",
            IEnumerable<HttpStatusCode> expectedStatusCodes = null,
            string processName = null)
        {
            logger = logger ?? new Logger<RequestHelper>(new NullLoggerFactory());
            // get process name from stacktrace
            processName = processName ?? new StackTrace().GetFrame(7)?.GetMethod()?.Name ?? "HttpRequest";

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver =
                    camelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver(),
                NullValueHandling = ignoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            var request = new HttpRequestMessage(method ?? new HttpMethod("GET"), url);
            if (authorization != null)
            {
                request.Headers.TryAddWithoutValidation("Authorization", authorization);
            }

            if (headers != null)
            {
                foreach (var (header, value) in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.ToString(), value);
                }
            }

            if (body != null)
            {
                if (body is HttpContent content)
                {
                    request.Content = content;
                }
                else if (body is string contentString)
                {
                    request.Content = new StringContent(contentString, Encoding.UTF8, mediaType);
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(body, serializerSettings),
                        Encoding.UTF8, mediaType);
                }
            }

            logger.Info(new LogData($"[RequestHelper] Starting request for {processName}")
            {
                {"Url", url},
                {"Method", method},
                {"Autorization", authorization != null},
                {"Body", body},
                {"Headers", headers},
                {"IgnoreNullValues", ignoreNullValues},
                {"MediaType", mediaType},
                {"ExpectedStatusCodes", expectedStatusCodes}
            });

            var response = await _httpClient.SendAsync(request);

            string bodyString = null;
            try
            {
                bodyString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                logger.Warning(
                    new ExceptionData(ex,
                        $"[RequestHelper] Failed to get body on Status {response.StatusCode} on {processName}")
                    {
                        {"url", url},
                        {"method", method},
                        {"responseHeaders", response?.Headers},
                        {"status", response?.StatusCode},
                        {"requestBody", body}
                    });
            }

            var result = new Response<string>
            {
                Body = bodyString,
                Method = method,
                Url = url,
                StatusCode = response.StatusCode,
                Headers = response.Headers,
                IsSuccessStatusCode = response.IsSuccessStatusCode,
                ReasonPhrase = response.ReasonPhrase
            };


            if (response != null && response.IsSuccessStatusCode)
            {
                logger.Info(new LogData($"[RequestHelper] Request successful for {processName}")
                {
                    {"url", url},
                    {"method", method},
                    {"responseHeaders", response?.Headers},
                    {"status", response?.StatusCode},
                    {"responseBody", result},
                    {"requestBody", body}
                });
            }
            else if (expectedStatusCodes?.Any(status => (status == response?.StatusCode)) ?? false)
            {
                logger.Warning(
                    new LogData($"[RequestHelper] HttpRequest failed: Status {response?.StatusCode} on {processName}")
                    {
                        {"url", url},
                        {"auth", authorization != null},
                        {"requestBody", body},
                        {"responseHeaders", response?.Headers},
                        {"method", method},
                        {"statusCode", response?.StatusCode},
                        {"responseBody", bodyString}
                    });
            }
            else
            {
                logger.Error(
                    new LogData(
                        $"[RequestHelper] HttpRequest failed unexpectedly: Status {response?.StatusCode} on {processName}")
                    {
                        {"url", url},
                        {"auth", authorization != null},
                        {"requestBody", body},
                        {"responseHeaders", response?.Headers},
                        {"method", method},
                        {"statusCode", response?.StatusCode},
                        {"responseBody", bodyString}
                    });
            }

            return result;
        }
    }
}