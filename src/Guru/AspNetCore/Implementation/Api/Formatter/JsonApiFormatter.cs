using Guru.Formatter.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Guru.AspNetCore.Implementation.Api.Formatter
{
    public class JsonApiFormatter : AbstractApiFormatter
    {
        private readonly ILightningFormatter _Formatter;

        public JsonApiFormatter(ILightningFormatter formatter)
        {
            _Formatter = formatter;
            ContentType = "application/json";
        }

        public override async Task<object> Read(Type targetType, Stream stream)
        {
            return await _Formatter.ReadObjectAsync(targetType, stream);
        }

        public override async Task Write(object instance, Stream stream)
        {
            await _Formatter.WriteObjectAsync(instance, stream);
        }
    }
}