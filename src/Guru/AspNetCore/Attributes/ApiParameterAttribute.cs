using System;

namespace Guru.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ApiParameterAttribute : Attribute
    {
        public string ParameterName { get; set; }

        public string Formatter { get; set; }
    }
}