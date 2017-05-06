using System.Reflection;

using Guru.ExtensionMethod;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementations
{
    internal class ImplementationRegister : IImplementationRegister
    {
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
                    var attribute = type.GetTypeInfo().GetCustomAttribute<InjectableAttribute>();
                    if (attribute != null)
                    {
                        instance.Add(attribute.Abstraction, new ImplementationResolver(new ImplementationDecorator(type, attribute.Lifetime, attribute.Priority)));
                    }
                }
            }
        }
    }
}