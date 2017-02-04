using System;
using System.IO;
using System.Reflection;
using System.Data.Common;
using System.Runtime.Loader;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.EntityFramework.Abstractions;
using Guru.EntityFramework.Configuration;
using Guru.DependencyInjection.Abstractions;

namespace Guru.EntityFramework
{
    [DI(typeof(IDatabaseProvider), Lifetime = Lifetime.Singleton)]
    public class DatabaseProvider : IDatabaseProvider
    {
        private ConcurrentDictionary<string, DbProviderFactory> _Caches = new ConcurrentDictionary<string, DbProviderFactory>();

        public IDatabase GetDatabase(string name)
        {
            var database = GetDatabaseItem(name);
            if (database == null)
            {
                throw new Exception($"database '{name}' does not exist in databases.xml.");
            }

            DbProviderFactory factory;
            if (!_Caches.ContainsKey(database.Provider))
            {
                //var factoryType = database.Provider.GetTypeByName();
                var factoryType = GetFactoryType(database.Provider);
                if (factoryType == null)
                {
                    throw new Exception($"databse factory type cannot be reached by '{database.Provider}'");
                }

                var instance = factoryType.GetField("Instance", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
                if (instance == null)
                {
                    throw new Exception($"database factory instance cannot be found from type '{factoryType.FullName}'.");
                }

                factory = _Caches.GetOrAdd(database.Provider, instance.GetValue(null) as DbProviderFactory);
            }
            else
            {
                _Caches.TryGetValue(database.Provider, out factory);
            }

            return new Database(factory, database.ConnectionString);
        }

        private DatabaseItemConfiguration GetDatabaseItem(string name)
        {
            var configuration = ContainerEntry.Resolve<IDatabaseConfiguration>();
            if (!configuration.Items.HasLength())
            {
                return null;
            }

            return configuration.Items.FirstOrDefault(x => x.Name.EqualsWith(name));
        }

        private Type GetFactoryType(string stringValue)
        {
            foreach (var a in GetAssemblies())
            {
                var type = a.GetType(stringValue, false, true);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private Assembly[] GetAssemblies()
        {
            var assemblies = new Assembly[0];

            var directoryInfo = new DirectoryInfo("./".FullPath());

            foreach (var fileInfo in directoryInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
            {
                assemblies = assemblies.Append(Assembly.Load(AssemblyLoadContext.GetAssemblyName(fileInfo.FullName)));
            }

            return assemblies;
        }
    }
}