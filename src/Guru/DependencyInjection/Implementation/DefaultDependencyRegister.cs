using System.Reflection;

using Guru.ExtensionMethod;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementation
{
    internal class DefaultDependencyRegister : IDependencyRegister
    {
        public IContainerInstance Register(IContainerInstance instance)
        {
            var assemblies = AssemblyLoader.Instance.GetAssemblies();

            if (!assemblies.HasLength())
            {
                return instance;
            }

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Subset(x => x.GetTypeInfo().IsClass))
                {
                    var attribute = type.GetTypeInfo().GetCustomAttribute<InjectableAttribute>();
                    if (attribute != null)
                    {
                        instance.Add(attribute.Abstraction, new DefaultDependencyResolver(new DefaultDependencyDescriptor(type, attribute.Lifetime, attribute.Priority)));
                    }
                }
            }

            return instance;
        }
    }
}
