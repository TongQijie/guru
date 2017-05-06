using System;

using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DynamicProxyAttribute : Attribute
    {
        public Type Abstraction { get; set; }

        public Lifetime Lifetime { get; set; }

        public int Priority { get; set; }
    }
}