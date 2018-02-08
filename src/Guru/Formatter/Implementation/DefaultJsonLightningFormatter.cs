using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Abstractions;
using Guru.Formatter.Json;

namespace Guru.Formatter.Implementation
{
    [Injectable(typeof(IJsonLightningFormatter), Lifetime.Transient)]
    internal class DefaultJsonLightningFormatter : DefaultAbstractLightningFormatter, IJsonLightningFormatter
    {
        public bool OmitDefaultValue { get; set; }

        public string DateTimeFormat { get; set; }

        public bool ElementKeyIgnoreCase { get; set; }

        public bool OmitNullValue { get; set; }

        public override async Task<object> ReadObjectAsync(Type targetType, Stream stream)
        {
            return await JsonSerializer.GetSerializer(targetType, new JsonSettings(Encoding.UTF8, OmitDefaultValue, DateTimeFormat ?? "", ElementKeyIgnoreCase, OmitNullValue)).DeserializeAsync(stream);
        }

        public override async Task WriteObjectAsync(object instance, Stream stream)
        {
            await JsonSerializer.GetSerializer(instance.GetType(), new JsonSettings(Encoding.UTF8, OmitDefaultValue, DateTimeFormat ?? "", ElementKeyIgnoreCase, OmitNullValue)).SerializeAsync(instance, stream);
        }
    }
}