using Guru.Executable;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppInstance.Default.RunAsync(args, true);
        }
    }
}