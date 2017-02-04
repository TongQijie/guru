using System;

namespace Guru.DynamicProxy
{
    public class InterceptorNotFoundException : Exception
    {
        public InterceptorNotFoundException(string name) : base($"interceptor '{name}' can NOT be found.")
        {
        }
    }
}