using System;
using System.Reflection;
using System.Data.Common;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.EntityFramework.Abstractions;
using Guru.EntityFramework.Configuration;
using Guru.DependencyInjection.Attributes;
using Guru.Logging.Abstractions;
using Guru.Logging;

namespace Guru.EntityFramework
{
    [Injectable(typeof(IDatabaseProvider), Lifetime.Singleton)]
    public class DatabaseProvider : IDatabaseProvider
    {
        private ConcurrentDictionary<string, DbProviderFactory> _Caches = new ConcurrentDictionary<string, DbProviderFactory>();

        private readonly ILogger _Logger;

        public DatabaseProvider(IFileLogger fileLogger)
        {
            _Logger = fileLogger;
        }

        public IDatabase GetDatabase(string name)
        {
            var database = GetDatabaseItem(name);
            if (database == null)
            {
                _Logger.LogEvent(nameof(DatabaseProvider), Severity.Error, $"database '{name}' does not exist in databases.xml.");
                return null;
            }

            DbProviderFactory factory;
            if (!_Caches.ContainsKey(database.Provider))
            {
                var factoryType = Type.GetType(database.Provider, false, true);
                if (factoryType == null)
                {
                    _Logger.LogEvent(nameof(DatabaseProvider), Severity.Error, $"databse factory type cannot be reached by '{database.Provider}'");
                    return null;
                }

                var instance = factoryType.GetField("Instance", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
                if (instance == null)
                {
                    _Logger.LogEvent(nameof(DatabaseProvider), Severity.Error, $"database factory instance cannot be found from type '{factoryType.FullName}'.");
                    return null;
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
            var configuration = DependencyContainer.Resolve<IDatabaseConfiguration>();
            if (!configuration.Items.HasLength())
            {
                return null;
            }

            return configuration.Items.FirstOrDefault(x => x.Name.EqualsWith(name));
        }
    }
}