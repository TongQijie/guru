using System;

namespace Guru.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiAttribute : Attribute
    {
        public ApiAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }

        public string ServiceName { get; set; }
    }
}