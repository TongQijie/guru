using System.Reflection;

using Guru.ExtensionMethod;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    public class DefaultRegister : IRegister
    {
        private readonly IAssemblyLoader _Loader;

        public DefaultRegister(IAssemblyLoader loader)
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
                    var attribute = type.GetTypeInfo().GetCustomAttribute<DIAttribute>();
                    if (attribute != null)
                    {
                        if (attribute.Lifetime == Lifetime.Transient)
                        {
                            container.RegisterTransient(attribute.Abstraction, type, attribute.Priority);
                        }
                        else if (attribute.Lifetime == Lifetime.Singleton)
                        {
                            container.RegisterSingleton(attribute.Abstraction, type, attribute.Priority);
                        }
                    }
                }
            }
        }
    }
}