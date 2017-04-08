using System;

namespace Guru.Jobs
{
    public class Schedule
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public ExecutionCycle Cycle { get; set; }

        public ExecutionPoint Point { get; set; }
    }
}