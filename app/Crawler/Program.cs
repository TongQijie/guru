using Guru.Jobs.Abstractions;
using Guru.DependencyInjection;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            ContainerManager.Default.Resolve<IJobDispatcher>().Run();
        }
    }
}