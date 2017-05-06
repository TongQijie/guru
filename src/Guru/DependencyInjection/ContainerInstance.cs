using System;
using System.Collections.Concurrent;

using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    internal class ContainerInstance : IContainerInstance
    {
        private ConcurrentDictionary<object, IImplementationResolver> _Resolvers = new ConcurrentDictionary<object, IImplementationResolver>();

        public void Add(object key, IImplementationResolver resolver)
        {
            _Resolvers.AddOrUpdate(key, resolver, (k, v) => v.Decorator.Priority >= resolver.Decorator.Priority ? v : resolver);
        }

        public bool Exists(object key)
        {
            return _Resolvers.ContainsKey(key);
        }

        public object GetImplementation(object key)
        {
            if (!_Resolvers.TryGetValue(key, out var resolver))
            {
                throw new Exception($"implementation of key '{key}' cannot be found.");
            }

            return resolver.Resolve();
        }

        public IContainerInstance Register(IImplementationRegister register)
        {
            register.Register(this);
            return this;
        }
    }
}