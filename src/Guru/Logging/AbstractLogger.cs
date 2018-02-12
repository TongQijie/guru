using Guru.Executable.Abstractions;
using Guru.Logging.Abstractions;

namespace Guru.Logging
{
    public abstract class AbstractLogger : ILogger
    {
        public AbstractLogger(IZooKeeper zooKeeper)
        {
            zooKeeper.Add(this);
        }

        public abstract void LogEvent(string category, Severity severity, params object[] parameters);

        public virtual void Dispose() { }
    }
}