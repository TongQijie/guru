using System;

using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    public static class Container
    {
        private static readonly IContainer _Instance = new DefaultContainer();
        
        public static IContainer Instance { get { return _Instance; } }

        public static IContainer Init()
        {
            return Init(new DefaultAssemblyLoader());
        }

        public static IContainer Init(IAssemblyLoader loader)
        {
            return _Instance.Use(new DefaultRegister(loader))
                .Use(new FileRegister(loader))
                .Use(new DynamicProxyRegister(loader));
        }

        public static IContainer Use(IRegister register)
        {
            return _Instance.Use(register);
        }

        public static object Resolve(Type type)
        {
            return _Instance.GetImplementation(type);
        }

        public static T Resolve<T>()
        {
            return _Instance.GetImplementation<T>();
        }

        public static IContainer Register(IResolver resolver)
        {
            _Instance.Register(resolver);
            return _Instance;
        }
    }
}