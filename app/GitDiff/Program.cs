using Guru.DependencyInjection;

namespace GitDiff
{
    public class Program
    {
        static Program()
        {
            ContainerEntry.Init(new DefaultAssemblyLoader());
        }

        public static void Main(string[] args)
        {
            var diff = ContainerEntry.Resolve<IDiff>();
            diff.Execute();
        }
    }
}