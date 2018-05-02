using Guru.Logging.Abstractions;

namespace Guru.AspNetCore.Abstractions
{
    public interface IResourceLogger : IFileLogger
    {
        void LogEvent(CallingContext context);
    }
}