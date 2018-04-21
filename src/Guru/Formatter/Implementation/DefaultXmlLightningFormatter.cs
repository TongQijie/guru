using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection;
using Guru.Formatter.Xml;

namespace Guru.Formatter.Implementation
{
    [Injectable(typeof(IXmlLightningFormatter), Lifetime.Transient)]
    internal class DefaultXmlLightningFormatter : DefaultAbstractLightningFormatter, IXmlLightningFormatter
    {
        public override string Tag => "XML";

        public override async Task<object> ReadObjectAsync(Type targetType, Stream stream)
        {
            return await XmlSerializer.GetSerializer(targetType, Encoding.UTF8, false).DeserializeAsync(stream);
        }

        public override async Task WriteObjectAsync(object instance, Stream stream)
        {
            await XmlSerializer.GetSerializer(instance.GetType(), Encoding.UTF8, false).SerializeAsync(instance, stream);
        }
    }
}