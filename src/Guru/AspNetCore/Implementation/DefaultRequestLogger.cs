using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Logging;
using Guru.Logging.Abstractions;
using System.Text;
using System.Reflection;
using Guru.Formatter.Abstractions;
using System;

namespace Guru.AspNetCore.Implementation
{
    [Injectable(typeof(IRequestLogger), Lifetime.Singleton)]
    internal class DefaultRequestLogger : FileLogger, IRequestLogger
    {
        private readonly IFormatter _Formatter;

        public DefaultRequestLogger(ILoggerKeeper loggerKeeper, IJsonFormatter formatter)
            : base(loggerKeeper)
        {
            Folder = "./requests".FullPath();
            Interval = 5000;
            _Formatter = formatter;
        }

        public void LogEvent(string category, CallingContext context,
            DateTime requestTime, DateTime responseTime, object[] requestBodys, object responseBody)
        {
            var stringBuilder = new StringBuilder();
            foreach (var parameter in context.InputParameters.GetDictionary())
            {
                stringBuilder.AppendLine($"[{parameter.Value.Source.ToString()}] {parameter.Key}={parameter.Value.Value}");
            }

            stringBuilder.AppendLine($"[RequestTime] {requestTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            stringBuilder.AppendLine($"[ResponseTime] {responseTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            stringBuilder.AppendLine($"[CostTime] {(responseTime - requestTime).TotalMilliseconds}ms");

            if (requestBodys != null && requestBodys.Length > 0)
            {
                foreach (var requestBody in requestBodys)
                {
                    if (requestBody == null)
                    {
                        stringBuilder.AppendLine($"[Request] null");
                    }
                    else if (requestBody.GetType() == typeof(string) ||
                        requestBody.GetType().GetTypeInfo().IsValueType)
                    {
                        stringBuilder.AppendLine($"[Request] {requestBody.ToString()}");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"[Request] {_Formatter.WriteString(requestBody, Encoding.UTF8)}");
                    }
                }
            }

            if (responseBody == null)
            {
                stringBuilder.AppendLine($"[Response] null");
            }
            else if (responseBody.GetType() == typeof(string) ||
                responseBody.GetType().GetTypeInfo().IsValueType)
            {
                stringBuilder.AppendLine($"[Response] {responseBody.ToString()}");
            }
            else
            {
                stringBuilder.AppendLine($"[Response] {_Formatter.WriteString(responseBody, Encoding.UTF8)}");
            }

            LogEvent(category, Severity.Information, stringBuilder.ToString());
        }
    }
}