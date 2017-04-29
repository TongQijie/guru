using System;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.EntityFramework.Abstractions;
using Guru.EntityFramework.Configuration;
using Guru.DependencyInjection.Abstractions;

namespace Guru.EntityFramework
{
    [DI(typeof(ICommandProvider), Lifetime = Lifetime.Singleton)]
    public class CommandProvider : ICommandProvider
    {
        private readonly IDatabaseProvider _DatabaseProvider;

        public CommandProvider(IDatabaseProvider databaseProvider)
        {
            _DatabaseProvider = databaseProvider;
        }

        public ICommand GetCommand(string name)
        {
            var item = GetCommandItem(name);
            if (item == null)
            {
                throw new Exception($"command '{name}' doest not exist in commands_*.xml.");
            }

            try
            {
                var database = _DatabaseProvider.GetDatabase(item.Database);

                var command = new Command(database, item.CommandType, item.CommandText);
                if (item.Parameters.HasLength())
                {
                    foreach (var parameter in item.Parameters)
                    {
                        command.AddParameter(parameter.Name, parameter.DbType, parameter.Direction, parameter.Size);
                    }
                }

                return command;
            }
            catch (Exception e)
            {
                throw new Exception($"some error has occur when getting command '{name}'.", e);
            }
        }

        private CommandItemConfiguration GetCommandItem(string name)
        {
            var configurations = (Container.Resolve(typeof(ICommandConfiguration)) as object[]).Select(x => x as ICommandConfiguration);
            if (!configurations.HasLength())
            {
                return null;
            }

            foreach (var configuration in configurations.Subset(x => x.Items.HasLength()))
            {
                foreach (var command in configuration.Items)
                {
                    if (command.Name.EqualsWith(name))
                    {
                        return command;
                    }
                }
            }

            return null;
        }
    }
}