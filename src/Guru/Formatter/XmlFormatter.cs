using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Formatter
{
    [Injectable(typeof(IXmlFormatter), Lifetime.Transient)]
    public class XmlFormatter : FormatterBase, IXmlFormatter
    {
        public bool OmitNamespaces { get; set; }

        public XmlFormatter()
        {
            OmitNamespaces = true;
        }

        private static XmlSerializerNamespaces _EmptyNamespaces = null;

        public static XmlSerializerNamespaces EmptyNamespaces
        {
            get
            {
                if (_EmptyNamespaces == null)
                {
                    _EmptyNamespaces = new XmlSerializerNamespaces();
                    _EmptyNamespaces.Add("", "");
                }
                return _EmptyNamespaces;
            }
        }

        public override object ReadObject(Type targetType, Stream stream)
        {
            return new XmlSerializer(targetType).Deserialize(stream);
        }

        public override void WriteObject(object instance, Stream stream)
        {
            if (OmitNamespaces)
            {
                new XmlSerializer(instance.GetType()).Serialize(stream, instance, EmptyNamespaces);
            }
            else
            {
                new XmlSerializer(instance.GetType()).Serialize(stream, instance);
            }
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