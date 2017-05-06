using System;

namespace Guru.DependencyInjection.Abstractions
{
    public interface IImplementationDecorator
    {
        Type ImplementationType { get; }

        Lifetime Lifetime { get; }

        int Priority { get; }
    }
}