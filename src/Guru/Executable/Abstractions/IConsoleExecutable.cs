using System.Threading.Tasks;

namespace Guru.Executable.Abstractions
{
    public interface IConsoleExecutable
    {
        Task<int> RunAsync(CommandLineArgs args);
    }
}