using System;
using Guru.DependencyInjection;
using Guru.Executive.Abstractions;
using Guru.Logging.Abstractions;

namespace Guru.Executable
{
    public class ConsoleInstance
    {
        private static ConsoleInstance _Default = null;

        public static ConsoleInstance Default { get { return _Default ?? (_Default = new ConsoleInstance()); } }

        private ConsoleInstance()
        {
            _Logger = ContainerManager.Default.Resolve<IFileLogger>();
        }

        private readonly ILogger _Logger = null; 

        public void Run(string[] args)
        {
            _Logger.LogEvent("ConsoleInstance", Severity.Information, "console app started.");

            try
            {
                ContainerManager.Default.Resolve<IConsoleExecutable>().Run(args);
            }
            catch (Exception e)
            {
                _Logger.LogEvent("ConsoleInstance", Severity.Information, "console app occurred an unhandled exception.", e);
            }

            _Logger.LogEvent("ConsoleInstance", Severity.Information, "console app stopped.");

            _Logger.Dispose();
        }
    }
}