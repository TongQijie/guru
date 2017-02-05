using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using Microsoft.Extensions.DependencyModel;

using Guru.ExtensionMethod;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    public class DefaultAssemblyLoader : IAssemblyLoader
    {
        private static Assembly[] _Assemblies;

        public Assembly[] GetAssemblies()
        {
            if (_Assemblies == null)
            {
                var assemblyNames = new AssemblyName[0];

                // load from runtime libraries
                var dependencies = DependencyContext.Default.RuntimeLibraries;
                foreach (var library in dependencies.Where(x => !x.Name.StartsWith("runtime") && !x.Name.StartsWith("System") && !x.Name.StartsWith("Microsoft") && !x.Name.StartsWith("NETStandard") && x.Name != "Libuv" && !x.Name.StartsWith("Newtonsoft")))
                {
                    assemblyNames = assemblyNames.Append(new AssemblyName(library.Name));
                }

                // load from base directory
                var directoryInfo = new DirectoryInfo("./".FullPath());
                foreach (var fileInfo in directoryInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
                {
                    var assemblyName = AssemblyLoadContext.GetAssemblyName(fileInfo.FullName);

                    if (!assemblyNames.Exists(x => x.Name == assemblyName.Name))
                    {
                        assemblyNames = assemblyNames.Append(assemblyName);
                    }
                }

                _Assemblies = assemblyNames.Select(x => Assembly.Load(x));
            }

            return _Assemblies;
        }
    }
}