using Guru.Jobs.Abstractions;
using Guru.DependencyInjection;

namespace ConsoleApp.Jobs
{
    public class TestRunner
    {
        private readonly IJobDispatcher _JobDispatcher;

        public TestRunner()
        {
            _JobDispatcher = ContainerEntry.Resolve<IJobDispatcher>();
            var job = new AppleJob();
            _JobDispatcher.Add(job, null);
            _JobDispatcher.Enable(job);
        }

        public void Run()
        {
            _JobDispatcher.Run();

            System.Threading.Thread.Sleep(100000);
        }
    }
}