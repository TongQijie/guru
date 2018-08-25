using Guru.Executable;

namespace Guru.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppInstance.Default.RunAsync(args, true);
        }
    }
}