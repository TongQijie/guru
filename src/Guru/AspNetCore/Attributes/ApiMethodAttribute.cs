using System;

namespace Guru.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiMethodAttribute : Attribute
    {
        public ApiMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; set; }

        public bool DefaultMethod { get; set; } 
    }
}