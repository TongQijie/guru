using System;

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

        void Enable();

        void Disable();

        void Run(string[] args);
    }
}