using System;
using System.Threading;
using System.Threading.Tasks;

using Guru.Jobs.Configuration;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;

namespace Guru.Jobs
{
    public abstract class AbstractJob : IJob
    {
        private readonly ILogger _Logger;

        public AbstractJob(string name)
        {
            Name = name;

            _Logger = Container.Resolve<IFileLogger>();
        }

        public AbstractJob(string name, Schedule schedule) : this(name)
        {
            Schedule = schedule;
        }

        public string Name { get; private set; }

        public bool Enabled { get; private set; }

        public bool IsRunning { get; private set; }

        public Schedule Schedule { get; private set; }

        public DateTime? PrevExecTime { get; private set; }

        public DateTime NextExecTime { get; private set; }

        public void Config(JobScheduleConfiguration schedule)
        {
            Schedule = new Schedule()
            {
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Cycle = schedule.Cycle,
                Point = new ExecutionPoint()
                {
                    Year = schedule.Year,
                    Month = schedule.Month,
                    Day = schedule.Day,
                    Hour = schedule.Hour,
                    Minute = schedule.Minute,
                    Second = schedule.Second,
                },
            };
        }

        public bool Disable()
        {
            Enabled = false;
            return SafeStop();
        }

        public bool Enable()
        {
            Enabled = true;
            return Enabled;
        }

        public async Task RunAsync(string[] args)
        {
            if (!IsRunning)
            {
                IsRunning = true;

                PrevExecTime = DateTime.Now;

                NextExecTime = GetNextExecTime((DateTime)PrevExecTime);

                try
                {
                    await OnRunAsync(args);
                }
                catch (Exception e)
                {
                    _Logger.LogEvent("Job", Severity.Error, $"failed to execute job '{Name}'", e);
                }

                _Logger.LogEvent("Job", Severity.Information, 
                    $"job '{Name}' done and cost {(DateTime.Now - (DateTime)PrevExecTime).TotalMilliseconds} milliseconds.");

                IsRunning = false;
            }
        }

        protected abstract Task OnRunAsync(string[] args);

        private DateTime GetNextExecTime(DateTime prevExecTime)
        {
            switch (Schedule.Cycle)
            {
                case ExecutionCycle.Always:
                    {
                        return new DateTime();
                    }
                case ExecutionCycle.Periodic:
                    {
                        var timespan = new TimeSpan(
                            Schedule.Point.Day,
                            Schedule.Point.Hour,
                            Schedule.Point.Minute,
                            Schedule.Point.Second);

                        return prevExecTime + timespan;
                    }
                case ExecutionCycle.Hourly:
                    {
                        return new DateTime(
                            prevExecTime.Year,
                            prevExecTime.Month,
                            prevExecTime.Day,
                            prevExecTime.Hour,
                            0,
                            0)
                            .AddHours(1)
                            .AddMinutes(Schedule.Point.Minute)
                            .AddSeconds(Schedule.Point.Second);
                    }
                case ExecutionCycle.Daily:
                    {
                        return new DateTime(
                            prevExecTime.Year,
                            prevExecTime.Month,
                            prevExecTime.Day,
                            0,
                            0,
                            0)
                            .AddDays(1)
                            .AddHours(Schedule.Point.Hour)
                            .AddMinutes(Schedule.Point.Minute)
                            .AddSeconds(Schedule.Point.Second);
                    }
                case ExecutionCycle.Monthly:
                    {
                        return new DateTime(
                            prevExecTime.Year,
                            prevExecTime.Month,
                            0,
                            0,
                            0,
                            0)
                            .AddMonths(1)
                            .AddDays(Schedule.Point.Day)
                            .AddHours(Schedule.Point.Hour)
                            .AddMinutes(Schedule.Point.Minute)
                            .AddSeconds(Schedule.Point.Second);
                    }
                case ExecutionCycle.Yearly:
                    {
                        return new DateTime(
                            prevExecTime.Year,
                            0,
                            0,
                            0,
                            0,
                            0)
                            .AddYears(1)
                            .AddMonths(Schedule.Point.Month)
                            .AddDays(Schedule.Point.Day)
                            .AddHours(Schedule.Point.Hour)
                            .AddMinutes(Schedule.Point.Minute)
                            .AddSeconds(Schedule.Point.Second);
                    }
                default:
                    {
                        throw new Exception("schedule cycle is not valid.");
                    }
            }
        }

        private bool SafeStop()
        {
            var retry = 1;
            while (IsRunning && retry < 10)
            {
                Thread.Sleep(1000);
                retry++;
            }

            return !IsRunning;
        }
    }
}