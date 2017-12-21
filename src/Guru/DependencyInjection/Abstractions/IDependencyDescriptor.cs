using System;

namespace Guru.DependencyInjection.Abstractions
{
    internal interface IDependencyDescriptor
    {
        Type ImplementationType { get; }

        Lifetime Lifetime { get; }

        int Priority { get; }
    }
}