using Guru.Formatter.Abstractions;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Guru.AspNetCore.Implementation.Api.Formatter
{
    public class XmlApiFormatter : AbstractApiFormatter
    {
        private readonly ILightningFormatter _Formatter;

        public XmlApiFormatter(ILightningFormatter formatter)
        {
            _Formatter = formatter;
            ContentType = "application/xml";
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