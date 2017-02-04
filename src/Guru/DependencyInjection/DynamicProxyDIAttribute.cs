using System;

using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DynamicProxyDIAttribute : Attribute
    {
        public Type Abstraction { get; set; }

        public Lifetime Lifetime { get; set; }

        public int Priority { get; set; }
    }
}