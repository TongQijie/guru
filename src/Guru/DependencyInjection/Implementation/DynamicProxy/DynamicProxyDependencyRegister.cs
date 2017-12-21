using System;
using System.Reflection;

using Guru.DynamicProxy;
using Guru.ExtensionMethod;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementation.DynamicProxy
{
    internal class DynamicProxyDependencyRegister : IDependencyRegister
    {
        private readonly IDynamicProxyGenerator _DynamicProxyGenerator;

        public DynamicProxyDependencyRegister()
        {
            try
            {
                _DynamicProxyGenerator = DependencyContainer.Resolve<IDynamicProxyGenerator>();
            }
            catch (Exception) { }
        }

        public IContainerInstance Register(IContainerInstance instance)
        {
            var assemblies = AssemblyLoader.Instance.GetAssemblies();

            if (!assemblies.HasLength())
            {
                return instance;
            }

            foreach (var assembly in assemblies)
            {
                InternalRegister(instance, assembly);
            }

            return instance;
        }

        public IContainerInstance Register(IContainerInstance instance, Assembly assembly)
        {
            if (assembly == null)
            {
                return instance;
            }

            InternalRegister(instance, assembly);

            return instance;
        }

        private void InternalRegister(IContainerInstance instance, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Subset(x => x.GetTypeInfo().IsClass))
            {
                var attribute = type.GetTypeInfo().GetCustomAttribute<DynamicProxyAttribute>();
                if (attribute != null)
                {
                    Type proxyType;
                    try
                    {
                        proxyType = _DynamicProxyGenerator?.CreateProxyType(type);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"failed to create proxy type '{type.FullName}'.");
                        throw;
                    }

                    if (proxyType != null)
                    {
                        instance.Add(attribute.Abstraction, new DefaultDependencyResolver(new DefaultDependencyDescriptor(proxyType, attribute.Lifetime, attribute.Priority)));
                    }
                }
            }
        }
    }
}