using System;
using System.Linq;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;
using Guru.ExtensionMethod;
using Guru.Foundation;
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
            var kvs = new IgnoreCaseKeyValues<object>();
            kvs.Add("RequestTime", requestTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            kvs.Add("ResponseTime", responseTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            kvs.Add("CostTime", $"{(responseTime - requestTime).TotalMilliseconds}ms");

            if (context != null)
            {
                if (context.RequestHttpParameters != null)
                {
                    kvs.AddRange(context.RequestHttpParameters.KeyValues.Select(x =>
                        new ReadOnlyKeyValue<string, object>("RequestHttp", $"{x.Key}={x.Value}")));
                }
                if (context.RequestHeaderParameters != null)
                {
                    kvs.AddRange(context.RequestHeaderParameters.KeyValues.Select(x =>
                        new ReadOnlyKeyValue<string, object>("RequestHeader", $"{x.Key}={x.Value}")));
                }
                if (context.InputParameters != null)
                {
                    kvs.AddRange(context.InputParameters.KeyValues.Select(x =>
                        new ReadOnlyKeyValue<string, object>(x.Value.Source.ToString(), $"{x.Key}={x.Value.Value}")));
                }
                if (context.ResponseHttpParameters != null)
                {
                    kvs.AddRange(context.ResponseHttpParameters.KeyValues.Select(x =>
                        new ReadOnlyKeyValue<string, object>("ResponseHttp", $"{x.Key}={x.Value}")));
                }
                if (context.ResponseHeaderParameters != null)
                {
                    kvs.AddRange(context.ResponseHeaderParameters.KeyValues.Select(x =>
                        new ReadOnlyKeyValue<string, object>("ResponseHeader", $"{x.Key}={x.Value}")));
                }
            }

            if (requestBodys != null && requestBodys.Length > 0)
            {
                kvs.AddRange(requestBodys.Select(x => new ReadOnlyKeyValue<string, object>("Request", x)));
            }

            if (responseBody != null)
            {
                kvs.Add("Response", responseBody);
            }

            LogEvent(nameof(DefaultApiLogger), Severity.Information, kvs);
        }
    }
}