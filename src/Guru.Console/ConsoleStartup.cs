using Guru.Executable.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guru.Console
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    public class ConsoleStartup : IConsoleExecutable
    {
        private readonly ILogger _Logger;

        public Executable(IFileLogger fileLogger)
        {
            _Logger = fileLogger;
        }

        public async Task<int> RunAsync(CommandLineArgs args)
        {
            await new ElasticSearch.TestRunner().Run();
            return 0;
        }
    }
}
