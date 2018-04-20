using Guru.Executable;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConsoleAppInstance.Default.RunAsync(args, true);
        }
    }
}