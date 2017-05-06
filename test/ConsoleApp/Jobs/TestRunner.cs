using Guru.Jobs.Abstractions;
using Guru.DependencyInjection;

namespace ConsoleApp.Jobs
{
    public class TestRunner
    {
        private readonly IJobDispatcher _JobDispatcher;

        public TestRunner()
        {
            _JobDispatcher = ContainerManager.Default.Resolve<IJobDispatcher>();
        }

        public void Run()
        {
            _JobDispatcher.Run();
        }
    }
}