using System;

namespace Guru.Middleware.RESTfulService
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; set; }

        public string Prefix { get; set; }
    }
}