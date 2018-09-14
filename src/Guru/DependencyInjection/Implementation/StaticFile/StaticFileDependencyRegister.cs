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
                var attribute = type.GetTypeInfo().GetCustomAttribute<StaticFileAttribute>();
                if (attribute != null)
                {
                    instance.Add(attribute.Abstraction, new StaticFileDependencyResolver(new StaticFileDependencyDescriptor(type, attribute.Path, attribute.Format, attribute.MultiFiles)));
                }
                var singleFileAttr = type.GetTypeInfo().GetCustomAttribute<SingleFileAttribute>();
                if (singleFileAttr != null)
                {
                    instance.Add(singleFileAttr.Abstraction, new SingleFileDependencyResolver(new SingleFileDependencyDescriptor(type, singleFileAttr.Path, singleFileAttr.Format)));
                }
                var multiFileAttr = type.GetTypeInfo().GetCustomAttribute<MultiFileAttribute>();
                if (multiFileAttr != null)
                {
                    instance.Add(multiFileAttr.Abstraction, new MultiFileDependencyResolver(new MultiFileDependencyDescriptor(type, multiFileAttr.Path, multiFileAttr.Format)));
                }
            }
        }
    }
}
