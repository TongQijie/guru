using Guru.Executable;

namespace Guru.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConsoleAppInstance.Default.RunAsync(args, true);
        }
    }
}