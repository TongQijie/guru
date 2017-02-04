using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyModel;

using Guru.ExtensionMethod;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    public class DefaultAssemblyLoader : IAssemblyLoader
    {
        public Assembly[] GetAssemblies()
        {
            var assemblies = new Assembly[0];

            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies.Where(x => !x.Name.StartsWith("runtime") && !x.Name.StartsWith("System") && !x.Name.StartsWith("Microsoft") && !x.Name.StartsWith("NETStandard") && x.Name != "Libuv" && !x.Name.StartsWith("Newtonsoft")))
            {
                assemblies = assemblies.Append(Assembly.Load(new AssemblyName(library.Name)));
            }

            return assemblies;
        }
    }
}