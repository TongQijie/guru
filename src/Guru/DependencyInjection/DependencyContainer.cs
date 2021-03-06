﻿using Guru.DependencyInjection.Abstractions;
using Guru.DependencyInjection.Implementation;
using Guru.DependencyInjection.Implementation.Configurable;
using Guru.DependencyInjection.Implementation.DynamicProxy;
using Guru.DependencyInjection.Implementation.StaticFile;
using Guru.ExtensionMethod;
using System;

namespace Guru.DependencyInjection
{
    public static class DependencyContainer
    {
        private static IContainerInstance _ContainerInstance = null;

        internal static IContainerInstance ContainerInstance
        {
            get
            {
                if (_ContainerInstance == null)
                {
                    Initialize();
                }

                return _ContainerInstance;
            }
        }

        private static object _Locker = new object();

        private static void Initialize()
        {
            if (_ContainerInstance == null)
            {
                lock (_Locker)
                {
                    if (_ContainerInstance == null)
                    {
                        _ContainerInstance = new DefaultContainerInstance();

                        try
                        {
                            _ContainerInstance
                                .Register(new DefaultDependencyRegister())
                                .Register(new DynamicProxyDependencyRegister())
                                .Register(new StaticFileDependencyRegister())
                                .Register(new ConfigurableDependencyRegister());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"dependency register failed.{Environment.NewLine}{e.GetInfo()}");
                        }
                    }
                }
            }
        }

        public static object Resolve(object abstraction)
        {
            return ContainerInstance.GetImplementation(abstraction);
        }

        public static T Resolve<T>()
        {
            return (T)ContainerInstance.GetImplementation(typeof(T));
        }

        public static TAbstraction ResolveOrDefault<TAbstraction, TDefaultAbstraction>(object dependencyName)
        {
            if (ContainerInstance.Exists(dependencyName))
            {
                return (TAbstraction)ContainerInstance.GetImplementation(dependencyName);
            }
            else
            {
                return (TAbstraction)ContainerInstance.GetImplementation(typeof(TDefaultAbstraction));
            }
        }

        public static void RegisterSingleton(Type abstraction, Type implementationType)
        {
            ContainerInstance.Add(abstraction, new DefaultDependencyResolver(new DefaultDependencyDescriptor(implementationType, Lifetime.Singleton, 0)));
        }

        internal static bool Has(Type abstraction)
        {
            return ContainerInstance.Exists(abstraction);
        }

        public static bool Contains(object key)
        {
            return ContainerInstance.Exists(key);
        }
    }
}