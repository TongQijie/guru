using System;
using System.Collections.Concurrent;

using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    public class DefaultContainer : IContainer
    {
        private readonly ConcurrentDictionary<Type, IResolver> _ImplementationResolvers;
        
        private readonly IConstructorInjectionFactory _ConstructorInjectionFactory;
        
        public DefaultContainer()
        {
            _ImplementationResolvers = new ConcurrentDictionary<Type, IResolver>();
            _ConstructorInjectionFactory = new ConstructorInjectionFactory(this);
        }
        
        public IConstructorInjectionFactory ConstructorInjectionFactory { get { return _ConstructorInjectionFactory; } }
        
        public IContainer Use(IRegister register)
        {
            register.RegisterTo(this);
            return this;
        }
        
        public bool CanInject(Type type)
        {
            return _ImplementationResolvers.ContainsKey(type);
        }
        
        public object GetImplementation(Type type)
        {
            IResolver resolver;
            if (!_ImplementationResolvers.TryGetValue(type, out resolver))
            {
                throw new Exception($"i cannot find an implementment of type '{type.FullName}'.");
            }
            
            return resolver.Resolve();
        }
        
        public T GetImplementation<T>()
        {
            return (T)GetImplementation(typeof(T));
        }
        
        public void RegisterTransient<TAbstraction, TImplementation>()
        {
            var resolver = new DefaultResolver(
                this,
                typeof(TAbstraction),
                typeof(TImplementation),
                Lifetime.Transient,
                0);
            Register(resolver);
        }

        public void RegisterTransient<TAbstraction, TImplementation>(int priority)
        {
            var resolver = new DefaultResolver(
                this,
                typeof(TAbstraction),
                typeof(TImplementation),
                Lifetime.Transient,
                priority);
            Register(resolver);
        }

        public void RegisterSingleton<TAbstraction, TImplementation>()
        {
            var resolver = new DefaultResolver(
                this,
                typeof(TAbstraction),
                typeof(TImplementation),
                Lifetime.Singleton,
                0);
            Register(resolver);
        }

        public void RegisterSingleton<TAbstraction, TImplementation>(int priority)
        {
            var resolver = new DefaultResolver(
                this,
                typeof(TAbstraction),
                typeof(TImplementation),
                Lifetime.Singleton,
                priority);
            Register(resolver);
        }

        public void RegisterTransient(Type abstraction, Type implementation, int priority)
        {
            var resolver = new DefaultResolver(
                this,
                abstraction,
                implementation,
                Lifetime.Transient,
                priority);
            Register(resolver);
        }

        public void RegisterSingleton(Type abstraction, Type implementation, int priority)
        {
            var resolver = new DefaultResolver(
                this,
                abstraction,
                implementation,
                Lifetime.Singleton,
                priority);
            Register(resolver);
        }

        public void Register(IResolver resolver)
        {
            _ImplementationResolvers.AddOrUpdate(resolver.Abstraction, resolver, (t, r) => resolver.Priority > r.Priority ? resolver : r);
        }
    }
}