using Guru.Jobs.Abstractions;
using Guru.DependencyInjection;

namespace Crawler
{
    class Program
    {
        static Program()
        {
            Container.Init(new DefaultAssemblyLoader());
        }

        static void Main(string[] args)
        {
            var dispatcher = Container.Resolve<IJobDispatcher>();
            dispatcher.Async = true;

            //var job = new PrisonBreakS05();
            //dispatcher.Add(job, null);
            //dispatcher.Enable(job);

            dispatcher.Run();
        }
    }
}