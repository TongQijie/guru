using System.Collections.Generic;
using System.Linq;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;
using Guru.ExtensionMethod;
using Guru.Foundation;
using Guru.Logging;
using Guru.Logging.Implementation;

namespace Guru.AspNetCore.Implementation.Resource
{
    [Injectable(typeof(IResourceLogger), Lifetime.Singleton)]
    internal class DefaultResourceLogger : DefaultFileLogger, IResourceLogger
    {
        public DefaultResourceLogger(IZooKeeper zooKeeper)
            : base(zooKeeper)
        {
            Folder = "./ResLog".FullPath();
            Interval = 5000;
        }

        public void LogEvent(CallingContext context)
        {
            var kvs = new IgnoreCaseKeyValues<object>();

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

            LogEvent(nameof(DefaultResourceLogger), Severity.Information, kvs);
        }
    }
}