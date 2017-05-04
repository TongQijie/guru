using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Guru.ExtensionMethod;
using Guru.Middleware.Abstractions;

namespace Guru.Middleware.StaticFile
{
    public class CallingContext : ICallingContext
    {
        public CallingContext(HttpContext context, string uri)
        {
            Context = context;

            var fields = uri.SplitByChar('/');
            var lastField = fields[fields.Length - 1];
            var dotIndex = lastField.LastIndexOf('.');

            Path = uri;
            ResourceType = ((dotIndex >= 0) && (dotIndex < lastField.Length - 1)) ? lastField.Substring(dotIndex + 1) : string.Empty;
        }

        public HttpContext Context { get; private set; }

        public string Path { get; set; }
        
        public string ResourceType { get; set; }

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