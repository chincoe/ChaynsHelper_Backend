using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ChaynsHelper.RequestHelper
{
    public class Response<T>
    {
        public Response(){}

        public Response(Response<string> response, T body)
        {
            Url = response.Url;
            Method = response.Method;
            StatusCode = response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;
            Headers = response.Headers;
            IsSuccessStatusCode = response.IsSuccessStatusCode;
            Body = body;
        }
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public T Body { get; set; }
        public HttpResponseHeaders Headers { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }
}