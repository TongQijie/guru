using System.Collections.Generic;
using System.Linq;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;
using Guru.ExtensionMethod;
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
            var kvs = new List<KeyValuePair<string, object>>();

            if (context != null && context.InputParameters != null)
            {
                kvs.AddRange(context.InputParameters.GetDictionary().Select(x =>
                    new KeyValuePair<string, object>(x.Value.Source.ToString(), $"{x.Value.Name}={x.Value.Value}")));
            }

            if (context != null && context.OutputParameters != null)
            {
                kvs.AddRange(context.OutputParameters.GetDictionary().Select(x =>
                    new KeyValuePair<string, object>(x.Value.Source.ToString(), $"{x.Value.Name}={x.Value.Value}")));
            }
        }
    }
}