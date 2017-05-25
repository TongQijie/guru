using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Guru.Formatter.Xml;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Formatter
{
    [Injectable(typeof(IXmlFormatter), Lifetime.Transient)]
    public class XmlFormatter : FormatterBase, IXmlFormatter
    {
        public bool OmitDefaultValue { get; set; }

        public Encoding DefaultEncoding { get; set; }

        public XmlFormatter()
        {
            DefaultEncoding = Encoding.UTF8;
        }

        public override object ReadObject(Type targetType, Stream stream)
        {
            return XmlSerializer.GetSerializer(targetType, DefaultEncoding, OmitDefaultValue).Deserialize(stream);
        }

        public override void WriteObject(object instance, Stream stream)
        {
            Xml.XmlSerializer.GetSerializer(instance.GetType(), DefaultEncoding, OmitDefaultValue).Serialize(instance, stream);
        }

        public override async Task WriteObjectAsync(object instance, Stream stream)
        {
            await Xml.XmlSerializer.GetSerializer(instance.GetType(), Encoding.UTF8, false).SerializeAsync(instance, stream);
        }

        public override async Task<object> ReadObjectAsync(Type targetType, Stream stream)
        {
            return await Xml.XmlSerializer.GetSerializer(targetType, Encoding.UTF8, false).DeserializeAsync(stream);
        }
    }
}