using System;

namespace Guru.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiServiceAttribute : Attribute
    {
        public ApiServiceAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }

        public string ServiceName { get; set; }
    }
}