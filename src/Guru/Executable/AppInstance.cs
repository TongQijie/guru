using System;
using Guru.DependencyInjection;
using Guru.Executable.Abstractions;
using Guru.Logging.Abstractions;
using System.Threading;
using Guru.Logging;

namespace Guru.Executable
{
    public class AppInstance
    {
        private static AppInstance _Default = null;

        public static AppInstance Default { get { return _Default ?? (_Default = new AppInstance()); } }

        private readonly ILogger _Logger;

        private readonly IZooKeeper _ZooKeeper;

        private readonly ICommandLineArgsParser _CommandLineArgsParser;

        private AppInstance()
        {
            _Logger = DependencyContainer.Resolve<IFileLogger>();
            _ZooKeeper = DependencyContainer.Resolve<IZooKeeper>();
            _CommandLineArgsParser = DependencyContainer.Resolve<ICommandLineArgsParser>();
        }

        private bool _InstanceAlive = false;

        public void RunAsync(string[] args, bool loop = false)
        {
            _Logger.LogEvent(nameof(AppInstance), Severity.Information, "Application started.");

            try
            {
                if (DependencyContainer.Resolve<IStartup>().RunAsync(_CommandLineArgsParser.Parse(args)).GetAwaiter().GetResult() == 0)
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
                _Logger.LogEvent(nameof(AppInstance), Severity.Information, "Application occurred an unhandled exception.", e);
                Console.WriteLine($"Application aborted. {e.Message}");
            }

            _Logger.LogEvent(nameof(AppInstance), Severity.Information, "Application stopped.");

            _ZooKeeper.RemoveAll();

            Console.WriteLine($"Application stopped.");
        }
    }
}