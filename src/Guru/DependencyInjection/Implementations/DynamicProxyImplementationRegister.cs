using System;
using System.Reflection;

using Guru.DynamicProxy;
using Guru.ExtensionMethod;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementations
{
    internal class DynamicProxyImplementationRegister : IImplementationRegister
    {
        private readonly IDynamicProxyGenerator _DynamicProxyGenerator;

        public DynamicProxyImplementationRegister()
        {
            _DynamicProxyGenerator = ContainerManager.Default.Resolve<IDynamicProxyGenerator>();
        }

        public void Register(IContainerInstance instance)
        {
            var assemblies = AssemblyLoader.Instance.GetAssemblies();

            if (!assemblies.HasLength())
            {
                return;
            }

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Subset(x => x.GetTypeInfo().IsClass))
                {
                    var attribute = type.GetTypeInfo().GetCustomAttribute<DynamicProxyAttribute>();
                    if (attribute != null)
                    {
                        Type proxyType;
                        try
                        {
                            proxyType = _DynamicProxyGenerator.CreateProxyType(type);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"failed to create proxy type '{type.FullName}'.");
                            throw;
                        }

                        instance.Add(attribute.Abstraction, new ImplementationResolver(new ImplementationDecorator(proxyType, attribute.Lifetime, attribute.Priority)));
                    }
                }
            }
        }
    }
}