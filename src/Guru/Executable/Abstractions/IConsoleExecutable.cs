using System.Threading.Tasks;

namespace Guru.Executable.Abstractions
{
    public interface IConsoleExecutable
    {
        int Run(string[] args);

        Task<int> RunAsync(string[] args);
    }
}