using System;

namespace Guru.Middleware.RESTfulService
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodAttribute : Attribute
    {
        public string Name { get; set; }

        public bool Default { get; set; }

        public HttpVerb HttpVerb { get; set; }

        public ContentType Request { get; set; }

        public ContentType Response { get; set; }
    }
}
