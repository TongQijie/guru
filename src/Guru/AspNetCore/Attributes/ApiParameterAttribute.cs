using System;

namespace Guru.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ApiParameterAttribute : Attribute
    {
        public ApiParameterAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; set; }
    }
}