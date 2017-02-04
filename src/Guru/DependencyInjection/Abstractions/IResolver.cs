using System;

namespace Guru.DependencyInjection.Abstractions
{
    public interface IResolver
    {
         Type Abstraction { get; }

         Type Implementation { get; }

         Lifetime Lifetime { get; }

         int Priority { get; }

         object Resolve();
    }
}