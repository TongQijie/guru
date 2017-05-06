using Guru.DependencyInjection;

namespace GitDiff
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ContainerManager.Default.Resolve<IDiff>().Execute();
        }
    }
}