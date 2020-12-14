## [RequestHelper](./RequestHelper.cs)

A request Helper. Interesting features include:
 * Automatically deserialize the response into a model
 * Highly customizable
 * Automatically log the calling function's name in error logs
 * Adds your assembly name as user agent
 
### Add Service
```c#
// Startup.cs
services.AddRequestHelper();
```

### Request()
```c#
public async Task<Response<string>> Request(
    string url,
    HttpMethod method = HttpMethod.Get,
    string authorization = null,
    object body = null,
    Dictionary<HttpRequestHeader, string> headers = null,
    bool ignoreNullValues = true,
    bool catchErrors = true,
    string mediaType = "application/json",
    string processName = "HttpRequest",
    ILogger logger = null
)
```
| Parameter | Description | Type | Default/required |
|------|--------------|-----------|-------------|
| url | Request url | string | required |
| method | The request method | `HttpMethod?` | `HttpMethod.Get`|
| authorization | A Bearer token to set as Authorization header | `string` | `null` | 
| body | The request body.<br>`object`: Serialize to JSON body<br>`HttpContent`: Set as request content<br>`string`: Set as `StringContent` | `object`/`HttpContent`/`string` | `null` |
| headers | Headers to add to the request | `Dictionary<HttpRequestHeader, string>` | `null` |
| ignoreNullValues | Ignore null properties on JSON serialize | `bool` | `true` |
| camelCase | Use camelCase names on JSON serialize | `bool` | `true` |
| catchErrors | Always return a response object and fail silently. Throws an error on error status code if `false` | `bool` | `true` |
| mediaType | MediaType to use for JSON serialize | `string` | `"application/json"` |
| expectedStatusCodes | Set expected status codes. Unexpected status codes will be logged as error. Only affects logging | `IEnumerable<HttpStatusCode>` | Success status codes |
| processName | Name of the request for logging. Will try to get the calling method's name via Stacktrace if not specified | `string` | The calling method's name or `"HttpRequest"`|
| logger | pass a custom logger to the request | `ILogger` | `ILogger<RequestHelper>` |
| **@returns** | A Response object (see below) | `Response<string>` |

### Request\<T\>()
Identical parameters to Request().

This method automatically tries to deserialize the response body string into an object of type `T`.

Returns a `Response<T>`

### Response\<T\>
```c#
public class Response<T> 
{
    public string Url { get; set; }
    public HttpMethod Method { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string ReasonPhrase { get; set; }
    public T Body { get; set; }
    public HttpResponseHeaders Headers { get; set; }
    public bool IsSuccessStatusCode { get; set; }
}
```

### RequestHelper.StaticRequest(), RequestHelper.StaticRequest\<T\>()
Both request methods above can be called as static methods without the need to inject a dependency. 

Both static methods require a Logger of type `ILogger` to be passed as second argument. 
`null` is a valid value for the logger and will result in a NullLogger that doesn't log anything.

It is recommended to avoid the static methods unless service lifetimes or similar issues make the regular version unavailable.