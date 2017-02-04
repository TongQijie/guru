using System;

using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    public static class ContainerEntry
    {
        private static readonly IContainer _Container = new DefaultContainer();
        
        public static IContainer Container { get { return _Container; } }

        public static IContainer Init(IAssemblyLoader loader)
        {
            return _Container.Use(new DefaultRegister(loader))
                .Use(new FileRegister(loader))
                .Use(new DynamicProxyRegister(loader));
        }

        public static IContainer Use(IRegister register)
        {
            return _Container.Use(register);
        }

        public static object Resolve(Type type)
        {
            return _Container.GetImplementation(type);
        }

        public static T Resolve<T>()
        {
            return _Container.GetImplementation<T>();
        }

        public static IContainer Register(IResolver resolver)
        {
            _Container.Register(resolver);
            return _Container;
        }
    }
}