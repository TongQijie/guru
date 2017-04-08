using System;
using Guru.Jobs;

namespace ConsoleApp.Jobs
{
    public class AppleJob : AbstractJob
    {
        public AppleJob() 
            : base("Apple", new Schedule()
            {
                Cycle = ExecutionCycle.Periodic,
                Point = new ExecutionPoint()
                {
                    Second = 5,
                },
            })
        {
        }

        private int Count = 1;

        protected override void OnRun(string[] args)
        {
            Console.WriteLine($"run: {Count++} time(s)");
        }
    }
}