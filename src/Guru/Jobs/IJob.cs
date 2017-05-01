using System;
using System.Threading.Tasks;

using Guru.Jobs.Configuration;

namespace Guru.Jobs
{
    public interface IJob
    {
        string Name { get; }

        bool Enabled { get; }

        bool IsRunning { get; }

        Schedule Schedule { get; }

        DateTime? PrevExecTime { get; }

        DateTime NextExecTime { get; }

        void Config(JobScheduleConfiguration schedule);

        bool Enable();

        bool Disable();

        Task RunAsync(string [] args);
    }
}