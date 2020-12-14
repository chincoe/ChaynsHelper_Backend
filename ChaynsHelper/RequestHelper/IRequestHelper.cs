using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ChaynsHelper.RequestHelper
{
    public interface IRequestHelper
    {
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
        /// <param name="logger">logger for the request. Default: Logger of RequestHelper</param>
        /// <typeparam name="T">Return Type the body string will be deserialized to</typeparam>
        /// <returns>Response with instance of T as body</returns>
        Task<Response<T>> Request<T>(
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
            ILogger logger = null);

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
        /// <param name="logger">logger for the request. Default: Logger of RequestHelper</param>
        /// <returns>Response of string</returns>
        Task<Response<string>> Request(
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
            ILogger logger = null);
    }
}