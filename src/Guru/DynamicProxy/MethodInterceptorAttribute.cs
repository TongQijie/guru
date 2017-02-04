
using System;

namespace Guru.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodInterceptorAttribute : System.Attribute
    {
        public Type Type { get; set; }
    }
}