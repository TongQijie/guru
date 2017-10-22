using Guru.Logging.Abstractions;

namespace Guru.Logging
{
    public abstract class AbstractLogger : ILogger
    {
        public AbstractLogger(ILoggerKeeper loggerKeeper)
        {
            _LoggerKeeper = loggerKeeper;
            _LoggerKeeper.Connect(this);
        }

        private readonly ILoggerKeeper _LoggerKeeper;

        public abstract void LogEvent(string category, Severity severity, params object[] parameters);

        public virtual void Dispose()
        {
            _LoggerKeeper.Disconnect(this);
        }
    }
}