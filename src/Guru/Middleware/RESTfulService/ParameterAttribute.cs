using System;

namespace Guru.Middleware.RESTfulService
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterAttribute : Attribute
    {
        public string Alias { get; set; }

        public ParameterSource Source { get; set; }
    }
}