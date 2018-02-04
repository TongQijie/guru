using System;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.EntityFramework.Abstractions;
using Guru.EntityFramework.Configuration;
using Guru.DependencyInjection.Attributes;
using Guru.Logging.Abstractions;
using Guru.Logging;

namespace Guru.EntityFramework
{
    [Injectable(typeof(ICommandProvider), Lifetime.Singleton)]
    public class CommandProvider : ICommandProvider
    {
        private readonly IDatabaseProvider _DatabaseProvider;

        private readonly ILogger _Logger;

        public CommandProvider(IDatabaseProvider databaseProvider, IFileLogger fileLogger)
        {
            _DatabaseProvider = databaseProvider;
            _Logger = fileLogger;
        }

        public ICommand GetCommand(string name)
        {
            var item = GetCommandItem(name);
            if (item == null)
            {
                _Logger.LogEvent(nameof(CommandProvider), Severity.Error, $"command '{name}' doest not exist in commands_*.xml.");
                return null;
            }

            var database = _DatabaseProvider.GetDatabase(item.Database);
            if (database == null)
            {
                return null;
            }

            try
            {
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
                _Logger.LogEvent(nameof(CommandProvider), Severity.Error, $"some error has occur when getting command '{name}'.", e);
            }

            return null;
        }

        private CommandItemConfiguration GetCommandItem(string name)
        {
            var configuration = DependencyContainer.Resolve<ICommandConfiguration>();
            if (!configuration.Items.HasLength())
            {
                return null;
            }

            foreach (var command in configuration.Items)
            {
                if (command.Name.EqualsWith(name))
                {
                    return command;
                }
            }

            return null;
        }
    }
}