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
                foreach (var library in dependencies.Where(x => Filter(x.Name)))
                {
                    assemblyNames = assemblyNames.Append(new AssemblyName(library.Name));
                }

                // load from base directory
                var directoryInfo = new DirectoryInfo("./".FullPath());
                foreach (var fileInfo in directoryInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly).Where(x => Filter(x.Name)))
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

        private bool Filter(string assemblyName)
        {
            return !assemblyName.StartsWith("runtime") 
                && !assemblyName.StartsWith("System") 
                && !assemblyName.StartsWith("Microsoft") 
                && !assemblyName.StartsWith("NETStandard") 
                && assemblyName != "Libuv" 
                && !assemblyName.StartsWith("Newtonsoft");
        }
    }
}