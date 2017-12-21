using System.Reflection;

using Guru.ExtensionMethod;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementation.StaticFile
{
    internal class StaticFileDependencyRegister : IDependencyRegister
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
                    var attribute = type.GetTypeInfo().GetCustomAttribute<StaticFileAttribute>();
                    if (attribute != null)
                    {
                        instance.Add(attribute.Abstraction, new StaticFileDependencyResolver(new StaticFileDependencyDescriptor(type, attribute.Path, attribute.Format, attribute.MultiFiles)));
                    }
                }
            }

            return instance;
        }
    }
}
