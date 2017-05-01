using Guru.Jobs.Abstractions;
using Guru.DependencyInjection;

namespace Crawler
{
    class Program
    {
        static Program()
        {
            Container.Init();
        }

        static void Main(string[] args)
        {
            Container.Resolve<IJobDispatcher>().Run();
        }
    }
}