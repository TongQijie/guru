using System;

using Guru.DynamicProxy;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    /*********************************
     * IDynamicProxyGenerator
     *********************************/
    public class DynamicProxyResolver : IResolver
    {
        private readonly IContainer _Container;
        
        private readonly Type _Abstraction;

        private readonly Type _Implementation;

        private readonly Lifetime _Lifetime;
        
        private readonly int _Priority;
        
        public DynamicProxyResolver(IContainer container, Type abstraction, Type implementation, Lifetime lifetime, int priority)
        {
            _Container = container;
            _Abstraction = abstraction;
            _Implementation = implementation;
            _Lifetime = lifetime;
            _Priority = priority;
        }

        public Type Abstraction { get { return _Abstraction; } }

        public Type Implementation { get { return _Implementation; } }

        public int Priority { get { return _Priority; } }

        public Lifetime Lifetime { get { return _Lifetime; } }

        private object _SingletonObject;
        
        private Type _ProxyType;

        public object Resolve()
        {
            if (Lifetime == Lifetime.Singleton && _SingletonObject != null)
            {
                return _SingletonObject;
            }
            
            if (_ProxyType == null)
            {
                _ProxyType = _Container.GetImplementation<IDynamicProxyGenerator>().CreateProxyType(Implementation);
            }

            var obj = _Container.ConstructorInjectionFactory.GetInstance(_ProxyType);
            
            if (Lifetime == Lifetime.Singleton)
            {
                _SingletonObject = obj;
            }
            
            return obj;
        }
    }
}