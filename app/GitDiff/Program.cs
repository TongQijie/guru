using Guru.DependencyInjection;

namespace GitDiff
{
    public class Program
    {
        static Program()
        {
            Container.Init(new DefaultAssemblyLoader());
        }

        public static void Main(string[] args)
        {
            var diff = Container.Resolve<IDiff>();
            diff.Execute();
        }
    }
}