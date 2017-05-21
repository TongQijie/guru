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

        public bool IsAttr { get; set; }

        public bool IsArrayElement { get; set; }

        public string ArrayElementName { get; set; }
    }
}