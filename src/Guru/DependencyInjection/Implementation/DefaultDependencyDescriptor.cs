using System;

using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementation
{
    internal class DefaultDependencyDescriptor : IDependencyDescriptor
    {
        public DefaultDependencyDescriptor(Type implementationType, Lifetime lifetime, int priority)
        {
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Priority = priority;
        }

        public Type ImplementationType { get; private set; }

        public Lifetime Lifetime { get; private set; }

        public int Priority { get; private set; }
    }
}
