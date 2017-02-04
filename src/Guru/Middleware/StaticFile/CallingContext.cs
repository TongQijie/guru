using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Guru.Middleware.Abstractions;

namespace Guru.Middleware.StaticFile
{
    public class CallingContext : ICallingContext
    {
        public CallingContext(HttpContext context, string path, string resourceType)
        {
            Context = context;
            Path = path;
            ResourceType = resourceType;
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