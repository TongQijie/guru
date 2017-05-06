using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using Microsoft.Extensions.DependencyModel;

using Guru.ExtensionMethod;

namespace Guru.DependencyInjection
{
    internal class AssemblyLoader
    {
        public static AssemblyLoader _Instance = null;

        public static AssemblyLoader Instance { get { return _Instance ?? (_Instance = new AssemblyLoader()); } }

        private AssemblyLoader() { }

        private Assembly[] _Assemblies;

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

                foreach (var assemblyName in assemblyNames)
                {
                    try
                    {
                        _Assemblies = _Assemblies.Append(Assembly.Load(assemblyName));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            return _Assemblies;
        }

        private bool Filter(string assemblyName)
        {
            return !assemblyName.StartsWith("runtime", StringComparison.OrdinalIgnoreCase)
                && !assemblyName.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                && !assemblyName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)
                && !assemblyName.StartsWith("NETStandard", StringComparison.OrdinalIgnoreCase)
                && !assemblyName.EqualsIgnoreCase("Libuv")
                && !assemblyName.StartsWith("Newtonsoft", StringComparison.OrdinalIgnoreCase);
        }
    }
}
