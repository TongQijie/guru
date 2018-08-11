using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;
using Guru.Logging.Abstractions;
using System.Threading.Tasks;
using Guru.Executable;

namespace ConsoleApp
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    public class Executable : IConsoleExecutable
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