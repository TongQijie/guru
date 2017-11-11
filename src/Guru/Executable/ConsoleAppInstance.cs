using System;
using Guru.DependencyInjection;
using Guru.Executable.Abstractions;
using Guru.Logging.Abstractions;
using System.Threading;

namespace Guru.Executable
{
    public class ConsoleAppInstance
    {
        private static ConsoleAppInstance _Default = null;

        public static ConsoleAppInstance Default { get { return _Default ?? (_Default = new ConsoleAppInstance()); } }

        private ConsoleAppInstance()
        {
            _Logger = ContainerManager.Default.Resolve<IFileLogger>();
            _LoggerKeeper = ContainerManager.Default.Resolve<ILoggerKeeper>();
        }

        private readonly ILogger _Logger;

        private readonly ILoggerKeeper _LoggerKeeper;

        private bool _InstanceAlive = false;

        public void Run(string[] args, bool loop = false)
        {
            _Logger.LogEvent(nameof(ConsoleAppInstance), Severity.Information, "Application started.");

            try
            {
                if (ContainerManager.Default.Resolve<IConsoleExecutable>().Run(args) == 0)
                {
                    if (loop)
                    {
                        _InstanceAlive = true;

                        Console.CancelKeyPress += (sender, eventArgs) =>
                        {
                            _InstanceAlive = false;
                            eventArgs.Cancel = true;
                        };

                        Console.WriteLine("Application started. Press Ctrl+C to shut down.");
                        while (_InstanceAlive)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.LogEvent(nameof(ConsoleAppInstance), Severity.Information, "Application occurred an unhandled exception.", e);
                Console.WriteLine($"Application aborted. {e.Message}");
            }

            _Logger.LogEvent(nameof(ConsoleAppInstance), Severity.Information, "Application stopped.");

            _LoggerKeeper.DisposeAll();
        }
    }
}