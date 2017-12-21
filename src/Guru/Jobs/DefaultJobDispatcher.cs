using System;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.Jobs.Abstractions;
using Guru.Jobs.Configuration;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Jobs
{
    [Injectable(typeof(IJobDispatcher), Lifetime.Singleton)]
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

            _Jobs.GetOrAdd(job, args);
            _Jobs[job] = args;
        }

        public void Remove(IJob job)
        {
            if (!_Jobs.ContainsKey(job))
            {
                return;
            }

            if (!job.Disable() || !_Jobs.TryRemove(job, out var args))
            {
                _FileLogger.LogEvent("DefaultJobDispatcher", Severity.Error, $"failed to remove job '{job.Name}'. this job is still running.");
            }
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

            if (!job.Disable())
            {
                _FileLogger.LogEvent("DefaultJobDispatcher", Severity.Error, $"failed to disable job '{job.Name}'. this job is still running.");
            }
        }

        private bool _IsAlive = false;

        public bool IsAlive => _IsAlive;

        public void Run()
        {
            try
            {
                if (!_IsAlive)
                {
                    _IsAlive = true;
                }

                ReadConfig();

                while (_IsAlive)
                {
                    foreach (var job in _Jobs)
                    {
                        if (WillExec(job.Key))
                        {
                            job.Key.RunAsync(job.Value);
                        }
                    }

                    Thread.Sleep(10000);

                    ReadConfig();
                }

                SafeStop();
            }
            catch (Exception e)
            {
                _FileLogger.LogEvent("DefaultJobDispatcher", Severity.Fatal, e);
            }
            finally
            {
                _IsAlive = false;
            }
        }

        public void SafeStop()
        {
            var existsJobs = _Jobs.Keys.ToArray();

            var retry = 1;
            while (existsJobs.Exists(x => x.IsRunning) && retry < 10)
            {
                foreach (var job in existsJobs.Subset(x => x.IsRunning))
                {
                    job.Disable();
                }

                retry++;
            }
        }

        public void ReadConfig()
        {
            var config = DependencyContainer.Resolve<IApplicationConfiguration>();
            if (!config.Enabled)
            {
                _IsAlive = false;
                return;
            }

            if (config.Jobs.HasLength())
            {
                var existsJobs = _Jobs.Keys.ToArray();
                foreach (var job in existsJobs)
                {
                    var j = config.Jobs.FirstOrDefault(x => x.Name == job.Name);
                    if (j == null)
                    {
                        Remove(job);
                    }

                    if (j.Enabled != job.Enabled)
                    {
                        if (j.Enabled)
                        {
                            job.Config(j.Schedule);
                            Add(job, j.Args);
                            Enable(job);
                        }
                        else
                        {
                            Disable(job);
                        }
                    }
                }

                foreach (var job in config.Jobs.Where(x => !existsJobs.Exists(y => y.Name == x.Name)))
                {
                    var instance = Activator.CreateInstance(Type.GetType(job.Type), job.Name) as IJob;
                    if (instance != null)
                    {
                        if (job.Schedule != null)
                        {
                            instance.Config(job.Schedule);
                        }

                        Add(instance, job.Args);

                        if (job.Enabled)
                        {
                            Enable(instance);
                        }
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
    }
}