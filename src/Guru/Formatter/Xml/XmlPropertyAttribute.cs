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

        public bool Attribute { get; set; }

        public bool Array { get; set; }

        public bool ArrayItem { get; set; }

        public string ArrayItemAlias { get; set; }
    }
}