using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Guru.ExtensionMethod;
using Guru.Middleware.Abstractions;
using Microsoft.Extensions.Primitives;

namespace Guru.Middleware.RESTfulService
{
    public class CallingContext : ICallingContext
    {
        public CallingContext(HttpContext context, string serviceName, string methodName)
        {
            Context = context;
            ServiceName = serviceName;
            MethodName = methodName;
        }

        public HttpContext Context { get; private set; }

        public string ServiceName { get; private set; }

        public string MethodName { get; private set; }

        public HttpVerb HttpVerb
        {
            get
            {
                return Context.Request.Method.EqualsWith("GET") ? HttpVerb.GET : HttpVerb.POST;
            }
        }

        public string ContentType
        {
            get
            {
                return Context.Request.Headers["Content-Type"] == StringValues.Empty ? string.Empty : Context.Request.Headers["Content-Type"][0];
            }
        }

        public string Accept
        {
            get
            {
                return Context.Request.Headers["Accept"] == StringValues.Empty ? string.Empty : Context.Request.Headers["Accept"][0];
            }
        }

        private Dictionary<string, string> _QueryString;

        public Dictionary<string, string> QueryString
        {
            get
            {
                if (_QueryString == null)
                {
                    _QueryString = Context.Request.Query.ToDictionary(x => x.Key.ToLower(), x => x.Value[0]);
                }

                return _QueryString;
            }
        }
    }
}