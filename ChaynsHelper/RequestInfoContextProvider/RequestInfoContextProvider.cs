using System;
using Auth.Models;
using Microsoft.AspNetCore.Http;
using TobitLogger.Core;
using TobitLogger.Middleware;
using TobitWebApiExtensions.Extensions;

namespace ChaynsHelper.RequestInfoContextProvider
{
    public class RequestInfoContextProvider : ILogContextProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public RequestInfoContextProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public Guid? GetContextUid()
        {
            IHttpContextAccessor contextAccessor = this._contextAccessor;
            if (contextAccessor == null)
                return new Guid?();
            HttpContext httpContext = contextAccessor.HttpContext;
            return httpContext == null ? new Guid?() : new Guid?(httpContext.GetContextGuid());
        }

        public Guid? GetRequestUid()
        {
            IHttpContextAccessor contextAccessor = this._contextAccessor;
            Guid? nullable1;
            if (contextAccessor == null)
            {
                nullable1 = new Guid?();
            }
            else
            {
                HttpContext httpContext = contextAccessor.HttpContext;
                nullable1 = httpContext != null ? new Guid?(httpContext.GetRequestGuid()) : new Guid?();
            }

            Guid? nullable2 = nullable1;
            return !nullable2.HasValue ? this.GetContextUid() : nullable2;
        }

        public double? GetCustomNumber() => new double?();

        public int? GetLocationId()
        {
            IHttpContextAccessor contextAccessor = this._contextAccessor;
            int? locationId = null;
            HttpContext httpContext = contextAccessor?.HttpContext;
            if (httpContext != null)
            {
                LocationUserTokenPayload payload = (LocationUserTokenPayload) httpContext.GetTokenPayload();
                locationId = payload?.LocationId;
            }
            return locationId;
        }

        public string GetSiteId()
        {
            return null;
            // throw new NotImplementedException();
        }

        public string GetCustomText()
        {
            IHttpContextAccessor contextAccessor = this._contextAccessor;
            string method = null;
            string path = null;
            HttpContext httpContext = contextAccessor?.HttpContext;
            if (httpContext != null)
            {
                HttpRequest request = httpContext.Request;
                method = request?.Method;
                path = request?.Path;
            }
            return $"{method} {path}";   
        }

        public string GetPersonId()
        {
            IHttpContextAccessor contextAccessor = this._contextAccessor;
            string personId = null;
            HttpContext httpContext = contextAccessor?.HttpContext;
            if (httpContext != null)
            {
                LocationUserTokenPayload payload = (LocationUserTokenPayload) httpContext.GetTokenPayload();
                personId = payload?.PersonId;
            }
            return personId;
        }

        public string GetOperatingSystem()
        {
            return null;
            // throw new NotImplementedException();
        }
    }
}