using Guru.DependencyInjection.Abstractions;
using System;
using System.Collections.Concurrent;

namespace Guru.DependencyInjection.Implementation
{
    internal class DefaultContainerInstance : IContainerInstance
    {
        private ConcurrentDictionary<object, IDependencyResolver> _DependencyResolvers = new ConcurrentDictionary<object, IDependencyResolver>();

        public void Add(object key, IDependencyResolver resolver)
        {
            _DependencyResolvers.AddOrUpdate(key, resolver, (k, v) => v.Descriptor.Priority >= resolver.Descriptor.Priority ? v : resolver);
        }

        public bool Exists(object key)
        {
            return _DependencyResolvers.ContainsKey(key);
        }

        public object GetImplementation(object key)
        {
            if (!_DependencyResolvers.TryGetValue(key, out var resolver))
            {
                Console.WriteLine($"Implementation '{key}' cannot be found.");
                return null;
            }

            return resolver.Resolve();
        }

        public IContainerInstance Register(IDependencyRegister register)
        {
            return register.Register(this);
        }
    }
}