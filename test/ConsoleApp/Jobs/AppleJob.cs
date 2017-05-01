using System;
using System.Threading.Tasks;
using Guru.Jobs;

namespace ConsoleApp.Jobs
{
    public class AppleJob : AbstractJob
    {
        public AppleJob(string name) : base(name)
        {
        }

        private int Count = 1;

        protected override async Task OnRunAsync(string[] args)
        {
            Console.WriteLine($"run: {Count++} time(s)");

            await Task.Delay(1000);
        }
    }
}