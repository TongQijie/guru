using System;

namespace Guru.Logging.Abstractions
{
    public interface ILogger : IDisposable
    {
        void LogEvent(string category, Severity severity, params object[] parameters);
    }
}