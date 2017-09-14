using System;

namespace Guru.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class InjectableAttribute : Attribute
    {
        public InjectableAttribute() { }

        public InjectableAttribute(Type abstraction, Lifetime lifetime)
        {
            Abstraction = abstraction;
            Lifetime = lifetime;
        }

        public Type Abstraction { get; set; }

        public Lifetime Lifetime { get; set; }

        public int Priority { get; set; }
    }
}