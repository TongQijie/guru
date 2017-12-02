using System;
using Guru.Logging.Abstractions;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApiLogger : IFileLogger
    {
        void LogEvent(CallingContext context, 
            DateTime requestTime, DateTime responseTime, 
            object[] requestBodys, object response);
    }
}