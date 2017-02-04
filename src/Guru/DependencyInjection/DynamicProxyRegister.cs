using System.Reflection;

using Guru.ExtensionMethod;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    public class DynamicProxyRegister : IRegister
    {
        private readonly IAssemblyLoader _Loader;

        public DynamicProxyRegister(IAssemblyLoader loader)
        {
            _Loader = loader;
        }

        public void RegisterTo(IContainer container)
        {
            var assemblies = _Loader.GetAssemblies();
            
            if (!assemblies.HasLength())
            {
                return;
            }

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Subset(x => x.GetTypeInfo().IsClass))
                {
                    var attribute = type.GetTypeInfo().GetCustomAttribute<DynamicProxyDIAttribute>();
                    if (attribute != null)
                    {
                        container.Register(new DefaultResolver(container, attribute.Abstraction, type, attribute.Lifetime, attribute.Priority));
                    }
                }
            }
        }
    }
}