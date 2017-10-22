using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Logging;
using Guru.Logging.Abstractions;
using System.Text;

namespace Guru.AspNetCore.Implementation
{
    [Injectable(typeof(IRequestLogger), Lifetime.Singleton)]
    internal class DefaultRequestLogger : FileLogger, IRequestLogger
    {
        public DefaultRequestLogger(ILoggerKeeper loggerKeeper)
            : base(loggerKeeper)
        {
            Folder = "./requests".FullPath();
            Interval = 5000;
        }

        public void LogEvent(string category, CallingContext context)
        {
            var stringBuilder = new StringBuilder();
            foreach (var parameter in context.InputParameters.GetDictionary())
            {
                stringBuilder.AppendLine($"[{parameter.Value.Source.ToString()}] {parameter.Key}={parameter.Value.Value}");
            }
            LogEvent(category, Severity.Information, stringBuilder.ToString());
        }
    }
}