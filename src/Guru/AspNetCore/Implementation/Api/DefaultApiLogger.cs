using System;
using System.Collections.Generic;
using System.Linq;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using Guru.Logging;
using Guru.Logging.Implementation;

namespace Guru.AspNetCore.Implementation.Api
{
    [Injectable(typeof(IApiLogger), Lifetime.Singleton)]
    internal class DefaultApiLogger : DefaultFileLogger, IApiLogger
    {
        public DefaultApiLogger(IZooKeeper zooKeeper)
            : base(zooKeeper)
        {
            Folder = "./ApiLog".FullPath();
            Interval = 5000;
        }

        public void LogEvent(CallingContext context,
            DateTime requestTime, DateTime responseTime, 
            object[] requestBodys, object responseBody)
        {
            var kvs = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("RequestTime", requestTime.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                new KeyValuePair<string, object>("ResponseTime", responseTime.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                new KeyValuePair<string, object>("CostTime", $"{(responseTime - requestTime).TotalMilliseconds}ms"),
            };

            if (context != null && context.InputParameters != null)
            {
                kvs.AddRange(context.InputParameters.GetDictionary().Select(x =>
                    new KeyValuePair<string, object>(x.Value.Source.ToString(), $"{x.Key}={x.Value.Value}")));
            }

            if (requestBodys != null && requestBodys.Length > 0)
            {
                kvs.AddRange(requestBodys.Select(x => new KeyValuePair<string, object>("Request", x)));
            }

            if (responseBody != null)
            {
                kvs.Add(new KeyValuePair<string, object>("Response", responseBody));
            }

            LogEvent(nameof(DefaultApiLogger), Severity.Information, kvs);
        }
    }
}