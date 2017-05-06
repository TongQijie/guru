using System;
using System.Text;
using System.Threading.Tasks;

using Guru.DependencyInjection;
using Guru.Logging.Abstractions;
using Guru.Middleware.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Middleware.Errors
{
    [Injectable(typeof(IErrorHandler), Lifetime.Singleton)]
    public class ErrorHandler : IErrorHandler
    {
        private readonly IFileLogger _FileLogger;

        private readonly byte[] _ErrorBytes;

        public ErrorHandler(IFileLogger fileLogger)
        {
            _FileLogger = fileLogger;

            _ErrorBytes = Encoding.UTF8.GetBytes("An error occurred while processing your request.");
        }

        public async Task ProcessRequest(ICallingContext context)
        {
            var callingContext = context as CallingContext;
            if (callingContext == null)
            {
                throw new Exception("calling context is null.");
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("An error occurred.");
            stringBuilder.AppendLine($"Uri: {callingContext.Context.Request.Path.Value}{callingContext.Context.Request.QueryString.Value}");

            _FileLogger.LogEvent("HttpHandlerComponent", Severity.Error, stringBuilder.ToString(), callingContext.Exception);

            callingContext.Context.Response.StatusCode = 500;
            await callingContext.Context.Response.Body.WriteAsync(_ErrorBytes, 0, _ErrorBytes.Length);
        }
    }
}