using System;
using System.IO;
using System.Xml.Serialization;

using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Formatter
{
    [DI(typeof(IXmlFormatter), Lifetime = Lifetime.Singleton)]
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
    }
}