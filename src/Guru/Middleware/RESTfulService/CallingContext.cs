using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Guru.ExtensionMethod;
using Guru.Middleware.Abstractions;

namespace Guru.Middleware.RESTfulService
{
    public class CallingContext : ICallingContext
    {
        public CallingContext(HttpContext context, string uri)
        {
            Context = context;

            var fields = uri.SplitByChar('/');

            // 1) uri: A => A = service name, method is default
            // 2) uri: A/B => A = service name, B = method name
            // 3) uri: A/B/C => A = service prefix, B = service name, C = method name

            if (fields.Length == 1)
            {
                ServiceName = fields[0];
            }
            else if (fields.Length == 2)
            {
                ServiceName = fields[0];
                MethodName = fields[1];
            }
            else if (fields.Length > 2)
            {
                ServicePrefix = fields[0];
                ServiceName = fields[1];
                MethodName = fields[2];
            }
        }

        public HttpContext Context { get; private set; }

        public string ServicePrefix { get; private set; }

        public string ServiceName { get; private set; }

        public string MethodName { get; private set; }

        public HttpVerb HttpVerb => Context.Request.Method.EqualsIgnoreCase("GET") ? HttpVerb.GET : HttpVerb.POST;

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

        public string GetQueryString(string name) => QueryString.ContainsKey(name.ToLower()) ? QueryString[name.ToLower()] : string.Empty;

        private Dictionary<string, string> _Headers;

        public Dictionary<string, string> Headers
        {
            get
            {
                if (_Headers == null)
                {
                    _Headers = Context.Request.Headers.ToDictionary(x => x.Key.ToLower(), x => string.Join(";", x.Value));
                }

                return _Headers;
            }
        }

        public string GetHeader(string name) => Headers.ContainsKey(name.ToLower()) ? Headers[name.ToLower()] : string.Empty;

        public string ContentType => GetHeader("Content-Type");

        public string Accept => GetHeader("Accept");
    }
}