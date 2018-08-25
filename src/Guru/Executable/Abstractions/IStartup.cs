using System.Threading.Tasks;

namespace Guru.Executable.Abstractions
{
    public interface IStartup
    {
        Task<int> RunAsync(CommandLineArgs args);
    }
}