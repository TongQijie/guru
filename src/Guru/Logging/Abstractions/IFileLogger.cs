using System;

namespace Guru.Logging.Abstractions
{
    public interface IFileLogger : ILogger, IDisposable
    {
        string Folder { get; set; }

        int Interval { get; set; }
    }
}