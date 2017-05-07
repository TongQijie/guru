using System;

namespace Guru.Formatter.Xml
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XmlPropertyAttribute : Attribute
    {
        public XmlPropertyAttribute() { }

        public XmlPropertyAttribute(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; set; }
    }
}
