using System;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;

namespace Guru.Jobs
{
    public abstract class AbstractJob : IJob
    {
        private readonly ILogger _Logger;

        public AbstractJob(string name, Schedule schedule)
        {
            Name = name;
            Schedule = schedule;

            _Logger = Container.Resolve<IFileLogger>();
        }

        public string Name { get; private set; }

        public bool Enabled { get; private set; }

        public bool IsRunning { get; private set; }

        public Schedule Schedule { get; private set; }

        public DateTime? PrevExecTime { get; private set; }

        public DateTime NextExecTime { get; private set; }

        public void Disable()
        {
            IsRunning = false;
            Enabled = false;
        }

        public void Enable()
        {
            Enabled = true;
        }

        private object _Sync = new object();

        public void Run(string[] args)
        {
            if (!IsRunning)
            {
                IsRunning = true;

                PrevExecTime = DateTime.Now;

                NextExecTime = GetNextExecTime((DateTime)PrevExecTime);

                try
                {
                    OnRun(args);
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

        protected abstract void OnRun(string[] args);

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
    }
}