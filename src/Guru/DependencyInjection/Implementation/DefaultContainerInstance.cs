using Guru.DependencyInjection.Abstractions;
using Guru.DependencyInjection.Implementation.DynamicProxy;
using Guru.DependencyInjection.Implementation.StaticFile;
using System;
using System.Collections.Concurrent;
using System.Reflection;

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
                if (key is Type)
                {
                    var assembly = Assembly.GetAssembly(key as Type);
                    new DefaultDependencyRegister().Register(this, assembly);
                    new DynamicProxyDependencyRegister().Register(this, assembly);
                    new StaticFileDependencyRegister().Register(this, assembly);
                }

                if (!_DependencyResolvers.TryGetValue(key, out resolver))
                {
                    //Console.WriteLine($"Implementation '{key}' cannot be found.");
                    return null;
                }
            }

            return resolver.Resolve();
        }

        public IContainerInstance Register(IDependencyRegister register)
        {
            return register.Register(this);
        }

        private IContainerInstance Register(IDependencyRegister register, Assembly assembly)
        {
            return register.Register(this, assembly);
        }
    }
}