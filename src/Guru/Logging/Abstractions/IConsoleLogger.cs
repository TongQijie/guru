namespace Guru.Logging.Abstractions
{
    public interface IConsoleLogger : ILogger
    {
        void LogEvent(Severity severity, params object[] parameters);
    }
}