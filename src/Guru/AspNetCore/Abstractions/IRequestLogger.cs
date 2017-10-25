using System;
using Guru.Logging.Abstractions;

namespace Guru.AspNetCore.Abstractions
{
    public interface IRequestLogger : IFileLogger
    {
        void LogEvent(string category, CallingContext context, 
            DateTime requestTime, DateTime responseTime, object[] requestBodys, object response);
    }
}
