using System;
using System.IO;
using System.Threading.Tasks;

namespace Guru.AspNetCore.Implementation.Api.Formatter
{
    public abstract class AbstractApiFormatter
    {
        public string ContentType { get; set; }

        public abstract Task Write(object instance, Stream stream);

        public abstract Task<object> Read(Type targetType, Stream stream);
    }
}