using System;
using Guru.Executable.Abstractions;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection;
using Guru.ExtensionMethod;

namespace Watchman
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    public class Startup : IConsoleExecutable
    {
        public int Run(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("arguments is not valid. [source] [target]");
                return 1;
            }

            var context = ContainerManager.Default.Resolve<IContext>();
            
            context.Source = args[0].FullPath();
            context.Target = args[1].FullPath();

            new WatchFolder("", null);

            return 0;
        }
    }
}
