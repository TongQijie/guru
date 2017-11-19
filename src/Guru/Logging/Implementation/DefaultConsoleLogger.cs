using System;
using System.Text;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Logging.Abstractions;

namespace Guru.Logging.Implementation
{
    [Injectable(typeof(IConsoleLogger), Lifetime.Singleton)]
    public class DefaultConsoleLogger : IConsoleLogger
    {
        public void LogEvent(string category, Severity severity, params object[] parameters)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0}|{1,-12}|", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), severity);
            if (parameters != null)
            {
                foreach (var parameter in parameters.Subset(x => x != null))
                {
                    if (parameter is Exception)
                    {
                        stringBuilder.Append(new ExceptionWrapper(parameter as Exception).ToString());
                    }
                    else
                    {
                        stringBuilder.Append(parameter.ToString());
                    }

                    stringBuilder.AppendLine();
                }
            }

            Console.Write(stringBuilder.ToString());
        }

        public void LogEvent(Severity severity, params object[] parameters)
        {
            LogEvent(string.Empty, severity, parameters);
        }

        public void Dispose() { }
    }
}