using System;
using System.Threading;
using System.Collections.Concurrent;

using Guru.Jobs.Abstractions;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Jobs
{
    [DI(typeof(IJobDispatcher), Lifetime = Lifetime.Singleton)]
    internal class DefaultJobDispatcher : IJobDispatcher
    {
        private ConcurrentDictionary<IJob, string[]> _Jobs = new ConcurrentDictionary<IJob, string[]>();

        private readonly IFileLogger _FileLogger;

        public DefaultJobDispatcher(IFileLogger fileLogger)
        {
            _FileLogger = fileLogger;
        }

        public void Add(IJob job, string[] args)
        {
            if (args == null)
            {
                args = new string[0];
            }

            _Jobs.AddOrUpdate(job, args, (j, a) => a);
        }

        public void Remove(IJob job)
        {
            if (!_Jobs.ContainsKey(job))
            {
                return;
            }

            var retry = 1;
            while (job.IsRunning && retry < 10)
            {
                Thread.Sleep(1000);
            }

            string[] args;
            _Jobs.TryRemove(job, out args);
        }

        public void Enable(IJob job)
        {
            if (!_Jobs.ContainsKey(job))
            {
                return;
            }

            job.Enable();
        }

        public void Disable(IJob job)
        {
            if (!_Jobs.ContainsKey(job))
            {
                return;
            }

            job.Disable();
        }

        private bool _IsAlive = false;

        private object _Sync = new object();

        public void Run()
        {
            if (!_IsAlive)
            {
                lock (_Sync)
                {
                    if (!_IsAlive)
                    {
                        _IsAlive = true;

                        new Thread(() =>
                        {
                            try
                            {
                                while (_IsAlive)
                                {
                                    foreach (var job in _Jobs)
                                    {
                                        if (WillExec(job.Key))
                                        {
                                            CreateJobThread(job.Key, job.Value);
                                        }
                                    }

                                    Thread.Sleep(1000);
                                }
                            }
                            catch(Exception e)
                            {
                                _FileLogger.LogEvent("DefaultJobDispatcher", Severity.Fatal, e);
                            }
                        })
                        {
                            IsBackground = true,
                            Name = "JobDispatcher",
                        }.Start();
                    }
                }
            }
        }

        private bool WillExec(IJob job)
        {
            if (job.Schedule == null)
            {
                return false;
            }

            if (!job.Enabled)
            {
                return false;
            }

            if (job.IsRunning)
            {
                return false;
            }

            var now = DateTime.Now;

            if (job.Schedule.StartTime != null && now < (DateTime)job.Schedule.StartTime)
            {
                return false;
            }

            if (job.Schedule.EndTime != null && now > (DateTime)job.Schedule.EndTime)
            {
                return false;
            }

            switch (job.Schedule.Cycle)
            {
                case ExecutionCycle.Always:
                    {
                        return true;
                    }
                case ExecutionCycle.Periodic:
                    {
                        if (job.Schedule.Point == null)
                        {
                            return false;
                        }

                        if (job.PrevExecTime == null)
                        {
                            return true;
                        }
                        else
                        {
                            return now > job.NextExecTime;
                        }
                    }
                case ExecutionCycle.Hourly:
                    {
                        if (job.Schedule.Point == null)
                        {
                            return false;
                        }

                        if (job.PrevExecTime == null)
                        {
                            var nextExecTime = new DateTime(
                                now.Year,
                                now.Month,
                                now.Day,
                                now.Hour,
                                job.Schedule.Point.Minute,
                                job.Schedule.Point.Second);

                            return now > nextExecTime;
                        }
                        else
                        {
                            return DateTime.Now > job.NextExecTime;
                        }
                    }
                case ExecutionCycle.Daily:
                    {
                        if (job.Schedule.Point == null)
                        {
                            return false;
                        }

                        if (job.PrevExecTime == null)
                        {
                            var nextExecTime = new DateTime(
                                now.Year,
                                now.Month,
                                now.Day,
                                job.Schedule.Point.Hour,
                                job.Schedule.Point.Minute,
                                job.Schedule.Point.Second);

                            return now > nextExecTime;
                        }
                        else
                        {
                            return DateTime.Now > job.NextExecTime;
                        }
                    }
                case ExecutionCycle.Monthly:
                    {
                        if (job.Schedule.Point == null)
                        {
                            return false;
                        }

                        if (job.PrevExecTime == null)
                        {
                            var nextExecTime = new DateTime(
                                now.Year,
                                now.Month,
                                job.Schedule.Point.Day,
                                job.Schedule.Point.Hour,
                                job.Schedule.Point.Minute,
                                job.Schedule.Point.Second);

                            return now > nextExecTime;
                        }
                        else
                        {
                            return DateTime.Now > job.NextExecTime;
                        }
                    }
                case ExecutionCycle.Yearly:
                    {
                        if (job.Schedule.Point == null)
                        {
                            return false;
                        }

                        if (job.PrevExecTime == null)
                        {
                            var nextExecTime = new DateTime(
                                now.Year,
                                job.Schedule.Point.Month,
                                job.Schedule.Point.Day,
                                job.Schedule.Point.Hour,
                                job.Schedule.Point.Minute,
                                job.Schedule.Point.Second);

                            return now > nextExecTime;
                        }
                        else
                        {
                            return DateTime.Now > job.NextExecTime;
                        }
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public bool Async { get; set; }

        private void CreateJobThread(IJob job, string[] args)
        {
            if (Async)
            {
                job.RunAsync(args);
            }
            else
            {
                new Thread(() =>
                {
                    job.Run(args);
                })
                {
                    IsBackground = true,
                    Name = job.Name,
                }.Start();
            }
        }
    }
}