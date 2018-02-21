namespace Guru.Logging.Abstractions
{
    public interface ILogger
    {
        void LogEvent(string category, Severity severity, params object[] parameters);
    }
}