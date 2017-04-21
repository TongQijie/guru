using Guru.Jobs.Abstractions;
using Guru.DependencyInjection;

namespace Crawler
{
    class Program
    {
        static Program()
        {
            ContainerEntry.Init(new DefaultAssemblyLoader());
        }

        static void Main(string[] args)
        {
            var dispatcher = ContainerEntry.Resolve<IJobDispatcher>();
            dispatcher.Async = true;

            var job = new PrisonBreakS05();
            dispatcher.Add(job, null);
            dispatcher.Enable(job);

            dispatcher.Run();
        }
    }
}