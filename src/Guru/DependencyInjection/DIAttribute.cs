using System;

using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DIAttribute : Attribute
    {
        public DIAttribute() { }
        
        public DIAttribute(Type abstraction)
        {
            Abstraction = abstraction;
        }

        public Type Abstraction { get; set; }

        public Lifetime Lifetime { get; set; }

        public int Priority { get; set; }
    }
}